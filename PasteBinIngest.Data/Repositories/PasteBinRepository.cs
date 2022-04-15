using Newtonsoft.Json;
using PasteBinIngest.Data.DataTransferObjects;
using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Domain.Models;
using PasteBinIngest.Shared;
using System.Security.Cryptography;
using System.Text;

namespace PasteBinIngest.Data.Repositories
{
    public class PasteBinRepository : IPasteBinRepository
    {
        private readonly string _fileLocation;
        private readonly Loggger _loggger;

        public PasteBinRepository(string fileLocation, Loggger loggger)
        {
            _fileLocation = fileLocation;
            _loggger = loggger;
        }

        // DEAD CODE (for right now...):
        public string[] GetUrlsFromFolderNames()
        {
            var entityDiskLocation = Path.Combine(_fileLocation, Constants.EntryDirectory[1..]);
            var dirs = Directory.GetDirectories(entityDiskLocation);

            // remove entityDataPath from each url
            var cleanedDirs = dirs.Select(dir => dir[entityDiskLocation.Length..]).ToArray();
            return cleanedDirs;
        }

        public async Task SaveRequestAsync(PasteBinRequest request)
        {
            // save each entry
            var entries = request.PasteBinEntries.ToList();
            foreach (var entry in entries)
            {
                await _loggger.Info($"checking entry [{entry.Uri}]...");

                // remove the / from the uri for the folder name
                var uri = entry.Uri[1..];

                // check if entry exists
                if (!await CheckEntryExistsAsync(uri, entry.RawData))
                {
                    // doesnt exists
                    await SaveEntryAsync(entry.Id, entry);
                    await _loggger.Info($"entry saved to [{entry.Uri}]");
                    continue;
                }

                // already exists
                request.PasteBinEntries.Remove(entry);
                await _loggger.Info($"entry [{entry.Id}] already exist and unchanged, removed from request");
            }
            await _loggger.Debug("finished saving entries");

            // save request
            var entryIds = request.PasteBinEntries.Select(entry => entry.Id).ToArray();
            var dto = new PasteBinRequestData
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                EntryIds = entryIds
            };
            var dtoJson = JsonConvert.SerializeObject(dto);
            await _loggger.Debug("created request save object");

            // create directory
            var requestDataPath = _fileLocation + Constants.RequestDirectory;
            Directory.CreateDirectory(requestDataPath);
            await _loggger.Debug("created request directory");

            // write file
            var requestFilename = Path.Combine(requestDataPath, request.Id.ToString());
            await File.WriteAllTextAsync(requestFilename, dtoJson);
            await _loggger.Debug("finished saving request");
        }

        private async Task SaveEntryAsync(Guid requestId, PasteBinEntry entry)
        {
            var hash = await GetSha512HashOfDataAsync(entry.RawData);
            var dto = new PasteBinData
            {
                Id = entry.Id,
                RequestId = requestId,
                Name = entry.Name,
                Uri = entry.Uri,
                RawDataHash = hash,
                Created = entry.Created,
            };
            await _loggger.Debug($"created entry object for [{entry.Uri}]");

            // create directory
            var uri = entry.Uri[1..];
            var entryPath = Path.Combine(_fileLocation + Constants.EntryDirectory, uri);
            Directory.CreateDirectory(entryPath);
            await _loggger.Debug($"created directory for [{entry.Uri}]");

            // write raw data
            var rawFilePath = Path.Combine(entryPath, Constants.Raw);
            await File.WriteAllTextAsync(rawFilePath, entry.RawData);
            await _loggger.Debug($"saved raw data file for [{entry.Uri}]");

            // write DTO object
            var dtoFilePath = Path.Combine(entryPath, Constants.Data);
            var dtoJson = JsonConvert.SerializeObject(dto);
            await File.WriteAllTextAsync(dtoFilePath, dtoJson);
            await _loggger.Debug($"saved DTO object file for [{entry.Uri}]");

            // write quick-hash data
            var hashFilePath = Path.Combine(entryPath, Constants.Hash);
            await File.WriteAllTextAsync(hashFilePath, hash);
            await _loggger.Debug($"saved quick-hash file for [{entry.Uri}]");
        }

        private async Task<bool> CheckEntryExistsAsync(string uri, string rawData)
        {
            // hash of data
            var hash = await GetSha512HashOfDataAsync(rawData);
            await _loggger.Debug($"got hash of raw data");

            // read quick-hash data
            var entryPath = Path.Combine(_fileLocation + Constants.EntryDirectory, uri);
            var hashFilePath = Path.Combine(entryPath, Constants.Hash);
            if (!File.Exists(hashFilePath)) return false;
            var readHash = await File.ReadAllTextAsync(hashFilePath);
            await _loggger.Debug($"check generated hash against quick-hash file");

            // check quick-hash data
            var isExists = readHash == hash;
            await _loggger.Info($"does entry exist at [{uri}]? [{isExists}]");
            return isExists;
        }

        private static async Task<string> GetSha512HashOfDataAsync(string rawData)
        {
            // get byte hash of rawdata
            var rawBytes = Encoding.ASCII.GetBytes(rawData);
            var rawByteStream = new MemoryStream(rawBytes);
            using var sha512 = SHA512.Create();
            var hash = await sha512.ComputeHashAsync(rawByteStream);

            // get string hash from byte hash
            var stringBuilder = new StringBuilder();
            foreach (var b in hash)
            { stringBuilder.Append(b.ToString("x2")); }
            var result = stringBuilder.ToString();
            stringBuilder.Clear();

            return result;
        }
    }
}