using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.services;

namespace paste.bin.ingest.scripts.data
{
    /// <summary>
    /// all kinds of file adjustment scripts.
    /// </summary>
    internal class FileAdjustments
    {
        private readonly IPasteBinRepository _pasteBinRepository;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the adjustment scripts.
        /// </summary>
        /// <param name="pasteBinRepository">the data repository to use</param>
        /// <param name="logger">the logger to use</param>
        public FileAdjustments(IPasteBinRepository pasteBinRepository, Logger logger)
        {
            _pasteBinRepository = pasteBinRepository;
            _logger = logger;
        }

        /// <summary>
        /// script to rename the entry folders from the paste bin URL to the GUID.
        /// this script no longer works production data as the data structure has changed.
        /// </summary>
        public async Task RenameEntryFolderNamesFromUrlToIds()
        {
            // read in all the requests into memory before format change
            var allRequestIds = await _pasteBinRepository.GetRequestIdsFromFileNamesAsync();
            foreach (var requestId in allRequestIds)
            {
                var request = (await _pasteBinRepository.GetRequestsByIdsAsync(new[] { requestId }, true)).ToList().FirstOrDefault();

                // if request has no entries, skip
                if (request?.PasteBinEntries == null || !request.PasteBinEntries.Any())
                {
                    await _logger.Info("request does not contain any entries, skipping");
                    continue;
                }

                // rewrite the request and entries using new format
                await _pasteBinRepository.SaveRequestAsync(request);
                await _logger.Info("saved request and entries using new format");
            }
        }
    }
}