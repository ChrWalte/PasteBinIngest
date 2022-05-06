using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.models;
using paste.bin.ingest.core.services;
using System.Security.Cryptography;
using System.Text;

namespace paste.bin.ingest.scripts.data
{
    /// <summary>
    /// all kinds of duplicate data scripts.
    /// </summary>
    internal class DuplicateData
    {
        private readonly IPasteBinRepository _pasteBinRepository;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the duplicate data scripts.
        /// </summary>
        /// <param name="pasteBinRepository">the data repository to use</param>
        /// <param name="logger">the logger to use</param>
        public DuplicateData(IPasteBinRepository pasteBinRepository, Logger logger)
        {
            _pasteBinRepository = pasteBinRepository;
            _logger = logger;
        }

        /// <summary>
        /// script to check and remove duplicate data.
        /// this script no longer works in production due to file structure changes.
        /// </summary>
        public async Task CheckAndRemoveDuplicateDataAsync()
        {
            // load all data
            var allRequestIds = await _pasteBinRepository.GetRequestIdsFromFileNamesAsync();
            var allRequests = (await _pasteBinRepository.GetRequestsByIdsAsync(allRequestIds, true)).ToList();
            var allEntries = new List<PasteBinEntry>();
            foreach (var request in allRequests)
            {
                if (request.PasteBinEntries == null)
                    continue;

                allEntries.AddRange(request.PasteBinEntries);
                await _logger.Info($"added [{request.PasteBinEntries.Count}] entries to allEntries");
            }
            await _logger.Debug($"finished getting data from repository");

            // get hashes of all entries
            var entryIdsAndHashes = new Dictionary<string, string>();
            foreach (var entry in allEntries)
                entryIdsAndHashes.Add(entry.Id.ToString(), await GetSha512HashOfDataAsync(entry.RawData));
            await _logger.Debug($"finished getting hashes of all entries");

            // check all entries for duplicate data
            var allDuplicateHashes = new List<string>();
            var allDuplicateEntries = new List<PasteBinEntry>();
            foreach (var entry in entryIdsAndHashes)
            {
                if (allDuplicateHashes.Contains(entry.Value))
                    continue;

                var duplicateEntryIds = entryIdsAndHashes.Where(e => e.Value == entry.Value)
                    .Select(e => e.Key)
                    .ToList();
                if (duplicateEntryIds.Count <= 1)
                    continue;
                await _logger.Info($"found {duplicateEntryIds.Count} duplicate entries for hash [{entry.Value}]");

                var duplicateEntries = allEntries.Where(e => duplicateEntryIds.Contains(e.Id.ToString())).ToList();
                var oldestEntry = duplicateEntries.First();
                foreach (var duplicateEntry in duplicateEntries)
                    if (duplicateEntry.Created < oldestEntry.Created)
                        oldestEntry = duplicateEntry;
                await _logger.Info($"found [{oldestEntry.Created}] as oldest date for entry, will only save this entry");

                duplicateEntries.Remove(oldestEntry);
                allDuplicateHashes.Add(entry.Value);
                allDuplicateEntries.AddRange(duplicateEntries);
            }

            // save all data, removing duplicates and empty requests
            foreach (var request in allRequests)
            {
                if (request.PasteBinEntries == null)
                    continue;
                foreach (var entry in allDuplicateEntries)
                    if (request.PasteBinEntries.Contains(entry))
                        request.PasteBinEntries.Remove(entry);

                if (request.PasteBinEntries == null || !request.PasteBinEntries.Any())
                {
                    await _logger.Info("request does not contain any entries, skipping save");
                    continue;
                }

                await _pasteBinRepository.SaveRequestAsync(request);
                await _logger.Info($"saved request [{request.Id}] with [{request.PasteBinEntries.Count}] entries");
            }
            await _logger.Info($"finished checking and removed duplicate entry data");
        }

        /// <summary>
        /// same hash function used in the data repository.
        /// needed to check for duplicate data.
        /// </summary>
        /// <param name="rawData">the data to hash</param>
        /// <returns>the sha512 hash of the data</returns>
        private static async Task<string> GetSha512HashOfDataAsync(string? rawData)
        {
            // if nothing, return nothing
            if (string.IsNullOrWhiteSpace(rawData))
            { return string.Empty; }

            // get byte hash of rawData
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