using Newtonsoft.Json;
using PasteBinIngest.Data.DataTransferObjects;
using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Domain.Models;
using PasteBinIngest.Services;
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

        public void SaveRequest(PasteBinRequest request)
        {
            _loggger.Debug("save entire request started");

            // save each entry
            foreach (var entry in request.PasteBinEntries.ToArray())
            {
                var uri = entry.Uri[1..];

                // check if entry exists
                if (!CheckEntryExists(uri, entry.RawData))
                {
                    // doesnt exists
                    SaveEntry(entry.Id, entry);
                    continue;
                }

                // already exists
                _loggger.Debug("entry already exists, hash matches");
                request.PasteBinEntries.Remove(entry);
            }

            // save request
            var entryIds = request.PasteBinEntries.Select(entry => entry.Id).ToArray();
            var dto = new PasteBinRequestData
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                EntryIds = entryIds
            };
            var dtoJson = JsonConvert.SerializeObject(dto);

            // create directory
            var requestDataPath = _fileLocation + Constants.RequestDirectory;
            Directory.CreateDirectory(requestDataPath);

            // write file
            var requestFilename = Path.Combine(requestDataPath, request.Id.ToString());
            File.WriteAllText(requestFilename, dtoJson);
            _loggger.Debug("wrote request file");

            _loggger.LogObject("request object", dto);
            _loggger.Debug("saving entire request ended");
        }

        public void SaveEntry(Guid requestId, PasteBinEntry entry)
        {
            _loggger.Debug("save entry started");

            var hash = GetSha512HashOfData(entry.RawData);
            var dto = new PasteBinData
            {
                Id = entry.Id,
                RequestId = requestId,
                Name = entry.Name,
                Uri = entry.Uri,
                RawDataHash = hash,
                Created = entry.Created,
            };

            // create directory
            var uri = entry.Uri[1..];
            var entryPath = Path.Combine(_fileLocation + Constants.EntryDirectory, uri);
            Directory.CreateDirectory(entryPath);

            // write raw data
            var rawFilePath = Path.Combine(entryPath, Constants.Raw);
            File.WriteAllText(rawFilePath, entry.RawData);
            _loggger.Debug("wrote raw data file");

            // write DTO object
            var dtoFilePath = Path.Combine(entryPath, Constants.Data);
            var dtoJson = JsonConvert.SerializeObject(dto);
            File.WriteAllText(dtoFilePath, dtoJson);
            _loggger.Debug("wrote DTO object file");

            // write quick-hash data
            var hashFilePath = Path.Combine(entryPath, Constants.Hash);
            File.WriteAllText(hashFilePath, hash);
            _loggger.Debug("wrote quick-hash file");

            _loggger.LogObject("entry object", dto);
            _loggger.Debug("saving entry ended");
        }

        public bool CheckEntryExists(string uri, string rawData)
        {
            _loggger.Debug("checking existing entry hash started");

            // hash of data
            var hash = GetSha512HashOfData(rawData);

            // read quick-hash data
            var entryPath = Path.Combine(_fileLocation + Constants.EntryDirectory, uri);
            var hashFilePath = Path.Combine(entryPath, Constants.Hash);
            if (!File.Exists(hashFilePath)) return false;
            var readHash = File.ReadAllText(hashFilePath);
            _loggger.Debug("read quick-hash file");

            // check quick-hash data
            var isExists = readHash == hash;

            _loggger.Info($"checked existing hash, equal: {isExists}");
            _loggger.Debug("checking existing entry hash ended");

            return isExists;
        }

        private string GetSha512HashOfData(string rawData)
        {
            _loggger.Debug("calculating hash");

            // get byte hash of rawdata
            var rawBytes = Encoding.ASCII.GetBytes(rawData);
            using var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(rawBytes);

            // get string hash from byte hash
            var stringBuilder = new StringBuilder();
            foreach (var b in hash)
            { stringBuilder.Append(b.ToString("x2")); }
            var result = stringBuilder.ToString();
            stringBuilder.Clear();

            _loggger.Debug("calculated hash");

            return result;
        }
    }
}