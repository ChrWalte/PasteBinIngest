using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Shared;

namespace PasteBinIngest.Scripts.data
{
    /// <summary>
    /// all kinds of file adjustment scripts.
    /// </summary>
    internal class FileAdjustments
    {
        private readonly IPasteBinRepository _pastebinRepository;
        private readonly Loggger _loggger;

        /// <summary>
        /// initialize the adjustment scripts.
        /// </summary>
        /// <param name="pastebinRepository">the data repository to use</param>
        /// <param name="loggger">the logger to use</param>
        public FileAdjustments(IPasteBinRepository pastebinRepository, Loggger loggger)
        {
            _pastebinRepository = pastebinRepository;
            _loggger = loggger;
        }

        /// <summary>
        /// script to rename the entry folders from the pastebin URL to the GUID.
        /// this script no longer works production data as the data structure has changed.
        /// </summary>
        public async Task RenameEntryFolderNamesFromUrlToIds()
        {
            // read in all the requests into memory before format change
            var allRequestIds = await _pastebinRepository.GetRequestIdsFromFileNamesAsync();
            foreach (var requestId in allRequestIds)
            {
                var request = (await _pastebinRepository.GetRequestsByIdsAsync(new[] { requestId }, true)).ToList().FirstOrDefault();

                // if request has no entries, skip
                if (request?.PasteBinEntries == null || !request.PasteBinEntries.Any())
                {
                    await _loggger.Info("request does not contain any entries, skipping");
                    continue;
                }

                // rewrite the request and entries using new format
                await _pastebinRepository.SaveRequestAsync(request);
                await _loggger.Info("saved request and entries using new format");
            }
        }
    }
}