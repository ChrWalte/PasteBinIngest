using Newtonsoft.Json;
using paste.bin.ingest.core.data.DataTransferObjects;
using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.models;
using paste.bin.ingest.core.services;
using System.Security.Cryptography;
using System.Text;

namespace paste.bin.ingest.core.data.Repositories
{
    /// <summary>
    /// repository for paste bin data.
    /// </summary>
    public class PasteBinRepository : IPasteBinRepository
    {
        private readonly string _fileLocation;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the paste bin data repository.
        /// </summary>
        /// <param name="fileLocation">the file location to store the data</param>
        /// <param name="logger">the logger to log messages</param>
        public PasteBinRepository(string fileLocation, Logger logger)
        {
            _fileLocation = fileLocation;
            _logger = logger;

            var requestsDiskLocation = Path.Combine(_fileLocation, Constants.RequestDirectory);
            if (!Directory.Exists(requestsDiskLocation))
                Directory.CreateDirectory(requestsDiskLocation);

            var entityDiskLocation = Path.Combine(_fileLocation, Constants.EntryDirectory);
            if (!Directory.Exists(entityDiskLocation))
                Directory.CreateDirectory(entityDiskLocation);
        }

        /// <summary>
        /// get all request ids from file names.
        /// </summary>
        /// <returns>a list of GUIDs of all saved requests</returns>
        public async Task<IEnumerable<Guid>> GetRequestIdsFromFileNamesAsync()
        {
            var requestsDiskLocation = Path.Combine(_fileLocation, Constants.RequestDirectory);
            var fileNames = Directory.GetFiles(requestsDiskLocation);
            await _logger.Info($"got {fileNames.Length} request ids from request folder");

            var cleanedFileNames = fileNames.Select(dir
                => dir[requestsDiskLocation.Length..].Replace("\\", string.Empty))
                .ToArray();
            await _logger.Debug($"cleaned {fileNames.Length} request ids by removing directory path");

            var requestIds = cleanedFileNames.Select(fileName
                => Guid.TryParse(fileName, out var id) ? id : Guid.Empty).ToList();
            await _logger.Debug($"converted {fileNames.Length} request ids into GUIDs");

            return requestIds;
        }

        /// <summary>
        /// get all entry ids from folder names.
        /// </summary>
        /// <returns>a list of GUIDs of all saved entries</returns>
        public async Task<IEnumerable<Guid>> GetEntryIdsFromFolderNamesAsync()
        {
            var entityDiskLocation = Path.Combine(_fileLocation, Constants.EntryDirectory);
            var dirs = Directory.GetDirectories(entityDiskLocation);
            await _logger.Info($"got {dirs.Length} entry URLs from entry folder");

            // remove entityDataPath from each URL
            var cleanedDirs = dirs.Select(dir
                    => dir[entityDiskLocation.Length..].Replace("\\", string.Empty))
                .ToArray();
            await _logger.Debug($"cleaned {dirs.Length} entry URLs by removing directory path");

            var entryIds = cleanedDirs.Select(fileName
                => Guid.TryParse(fileName, out var id) ? id : Guid.Empty).ToList();
            await _logger.Debug($"converted {dirs.Length} entry ids into GUIDs");
            return entryIds;
        }

        /// <summary>
        /// gets paste bin requests by GUIDs.
        /// optional flag to include entry objects with requests.
        /// </summary>
        /// <param name="ids">the GUIDs of the requests wanted</param>
        /// <param name="withEntries">the flag to include entries</param>
        /// <returns>a list of paste bin requests</returns>
        public async Task<IEnumerable<PasteBinRequest>> GetRequestsByIdsAsync(IEnumerable<Guid> ids, bool withEntries = false)
        {
            var givenIds = ids.ToList();
            if (!givenIds.Any())
                return new List<PasteBinRequest>();

            var requests = new List<PasteBinRequest>();
            await _logger.Info($"getting {requests.Count} requests...");
            foreach (var id in givenIds)
            {
                await _logger.Info($"starting to get {id} from disk...");
                var requestDataPath = Path.Combine(_fileLocation, Constants.RequestDirectory);
                var requestFilename = Path.Combine(requestDataPath, id.ToString());
                var requestDtoJson = await File.ReadAllTextAsync(requestFilename);
                var requestDto = JsonConvert.DeserializeObject<PasteBinRequestData>(requestDtoJson);

                if (requestDto == null)
                {
                    await _logger.Warning($"request {id} came back as null");
                    await _logger.LogObject($"request [{id}]", requestDtoJson);
                    continue;
                }
                await _logger.Debug($"read in requestDto from disk");

                var request = new PasteBinRequest
                {
                    Id = requestDto.Id,
                    TimeStamp = requestDto.TimeStamp,
                    Url = "",
                    PasteBinEntries = withEntries
                        ? (await GetEntriesByIdsAsync(requestDto.EntryIds ?? Array.Empty<Guid>(), true)).ToList()
                        : requestDto.EntryIds?.Select(eId => new PasteBinEntry { Id = eId }).ToList()
                };
                await _logger.Debug($"created PasteBinRequest from PasteBinRequestData");

                requests.Add(request);
                await _logger.Info($"added request {id} to return requests, making total: {requests.Count}");
            }
            await _logger.Info($"finished getting requests");

            return requests;
        }

        /// <summary>
        /// gets paste bin entries by GUIDs.
        /// optional flag to include entry raw data with objects.
        /// </summary>
        /// <param name="ids">the GUIDs of the wanted entries</param>
        /// <param name="withRawData">the flag to include entry raw data</param>
        /// <returns>a list of paste bin entries</returns>
        public async Task<IEnumerable<PasteBinEntry>> GetEntriesByIdsAsync(IEnumerable<Guid> ids, bool withRawData = false)
        {
            var wantedIds = ids.ToList();
            var wantedEntries = new List<PasteBinEntry>();
            var allEntryDiskLocation = Path.Combine(_fileLocation, Constants.EntryDirectory);
            foreach (var id in wantedIds)
            {
                await _logger.Info($"checking {id} entry from disk...");
                var entryLocation = Path.Combine(allEntryDiskLocation, id.ToString());
                var entryDataLocation = Path.Combine(entryLocation, Constants.Data);
                if (!File.Exists(entryDataLocation))
                {
                    await _logger.Warning($"entry [{id}] data object file not found, skipping");
                    continue;
                }

                var entryDtoJson = await File.ReadAllTextAsync(entryDataLocation);
                var entryDto = JsonConvert.DeserializeObject<PasteBinData>(entryDtoJson);
                if (entryDto == null)
                {
                    await _logger.Warning($"entry {id} came back as null");
                    await _logger.LogObject($"entry [{id}]", entryDtoJson);
                    continue;
                }
                await _logger.Debug($"read in entryDto from disk");

                var entry = new PasteBinEntry
                {
                    Id = entryDto.Id,
                    Name = entryDto.Name,
                    Uri = entryDto.Uri,
                    Created = entryDto.Created,
                    RawData = string.Empty,
                };
                await _logger.Debug($"created PasteBinEntry from PasteBinData");

                if (withRawData)
                {
                    var entryRawDataLocation = Path.Combine(entryLocation, Constants.Raw);
                    if (!File.Exists(entryRawDataLocation))
                    {
                        await _logger.Warning($"entry [{id}] data file not found, skipping");
                        continue;
                    }
                    else
                    {
                        var entryRawData = await File.ReadAllTextAsync(entryRawDataLocation);
                        entry.RawData = entryRawData;
                        await _logger.Info($"added {entryDto.Id} entry raw data");
                    }
                }

                wantedEntries.Add(entry);
                await _logger.Info($"added entry to wantedEntries, making total: {wantedEntries.Count}");

                if (wantedEntries.Count != wantedIds.Count)
                    continue;

                await _logger.Info($"all entries found, no need to continue search");
                break;
            }
            await _logger.Info($"finished getting entries");

            return wantedEntries;
        }

        /// <summary>
        /// saves the given request and all entries to disk.
        /// if the request has no entries, it will not be saved.
        /// </summary>
        /// <param name="request">the request to save</param>
        /// <returns>nothing, but the request and its entries will be saved to disk</returns>
        public async Task SaveRequestAsync(PasteBinRequest request)
        {
            if (request.PasteBinEntries == null || request.PasteBinEntries.Count == 0)
            {
                await _logger.Warning($"request contained 0 entries, skipping");
                return;
            }

            // save each entry
            var entries = request.PasteBinEntries?.ToList() ?? new List<PasteBinEntry>();
            foreach (var entry in entries)
            {
                await _logger.Info($"checking entry [{entry.Uri}][{entry.Id}]...");

                // check if entry exists
                var id = entry.Id.ToString();
                if (entry.RawData != null && !await CheckEntryExistsByRawDataHashAsync(entry.RawData))
                {
                    // doesn't exists
                    await SaveEntryAsync(entry.Id, entry);
                    await _logger.Info($"entry saved to [{entry.Id}]");
                    continue;
                }

                // already exists
                request.PasteBinEntries?.Remove(entry);
                await _logger.Info($"entry [{entry.Id}] already exist and unchanged, removed from request");
            }
            await _logger.Debug("finished saving entries");

            // save request
            var entryIds = request.PasteBinEntries?.Select(entry => entry.Id).ToArray();
            var dto = new PasteBinRequestData
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                EntryIds = entryIds
            };
            var dtoJson = JsonConvert.SerializeObject(dto);
            await _logger.Debug("created request save object");

            // create directory
            var requestDataPath = Path.Combine(_fileLocation, Constants.RequestDirectory);
            Directory.CreateDirectory(requestDataPath);
            await _logger.Debug("created request directory");

            // write file
            var requestFilename = Path.Combine(requestDataPath, request.Id.ToString());
            await File.WriteAllTextAsync(requestFilename, dtoJson);
            await _logger.Debug("finished saving request");
        }

        /// <summary>
        /// save the given entry to disk.
        /// </summary>
        /// <param name="requestId">the request id that the entry came from</param>
        /// <param name="entry">the entry object to save to disk</param>
        /// <returns></returns>
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
            await _logger.Debug($"created entry object for [{entry.Uri}]");

            // create directory
            var entryPath = Path.Combine(_fileLocation, Constants.EntryDirectory, dto.Id.ToString());
            Directory.CreateDirectory(entryPath);
            await _logger.Debug($"created directory for [{entry.Uri}]");

            if (string.IsNullOrWhiteSpace(entry.RawData))
            {
                await _logger.Warning($"entry RawData was null or empty, skipping entry");
                return;
            }

            // write raw data
            var rawFilePath = Path.Combine(entryPath, Constants.Raw);
            await File.WriteAllTextAsync(rawFilePath, entry.RawData);
            await _logger.Debug($"saved raw data file for [{entry.Uri}]");

            // write DTO object
            var dtoFilePath = Path.Combine(entryPath, Constants.Data);
            var dtoJson = JsonConvert.SerializeObject(dto);
            await File.WriteAllTextAsync(dtoFilePath, dtoJson);
            await _logger.Debug($"saved DTO object file for [{entry.Uri}]");

            // write quick-hash data
            var hashFilePath = Path.Combine(entryPath, Constants.Hash);
            await File.WriteAllTextAsync(hashFilePath, hash);
            await _logger.Debug($"saved quick-hash file for [{entry.Uri}]");
        }

        /// <summary>
        /// check if the raw data already exists in the paste bin data set.
        /// </summary>
        /// <param name="rawData">the raw data to hash and check</param>
        /// <returns>true if the hash is found in the data set, false if it does not</returns>
        private async Task<bool> CheckEntryExistsByRawDataHashAsync(string rawData)
        {
            // hash of data
            var hash = await GetSha512HashOfDataAsync(rawData);
            await _logger.Debug($"got hash of raw data");

            // read quick-hash data
            var existingEntries = await GetEntryIdsFromFolderNamesAsync();
            foreach (var entry in existingEntries)
            {
                var entryPath = Path.Combine(_fileLocation, Constants.EntryDirectory, entry.ToString());
                var hashFilePath = Path.Combine(entryPath, Constants.Hash);
                if (!File.Exists(hashFilePath))
                    continue;

                // check quick-hash data
                var readHash = await File.ReadAllTextAsync(hashFilePath);
                var isExists = readHash == hash;
                if (!isExists)
                    continue;

                await _logger.Info($"found existing entry with id [{entry}]");
                return true;
            }

            await _logger.Info($"finished checking entry hashes, no existing entry found with matching hash");
            return false;
        }

        /// <summary>
        /// get the SHA512 hash of the given rawData
        /// </summary>
        /// <param name="rawData">the data to hash</param>
        /// <returns>the SHA512 hash of the given rawData</returns>
        private static async Task<string> GetSha512HashOfDataAsync(string? rawData)
        {
            // if nothing, return nothing
            if (string.IsNullOrWhiteSpace(rawData))
            { return string.Empty; }

            // get byte hash of raw data
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