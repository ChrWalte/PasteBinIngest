using paste.bin.ingest.core.models;
using paste.bin.ingest.core.services;

namespace paste.bin.ingest.api
{
    /// <summary>
    /// GraphQL controller for paste bin request data.
    /// </summary>
    public class Query
    {
        private readonly PasteBinService _pasteBinService;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the paste bin request GraphQL controller.
        /// </summary>
        /// <param name="pasteBinService">the paste bin service to use</param>
        /// <param name="logger">the logger to use</param>
        public Query(PasteBinService pasteBinService, Logger logger)
        {
            _pasteBinService = pasteBinService;
            _logger = logger;
        }

        // resources:
        // https://dotnetthoughts.net/getting-started-with-graphql-aspnetcore/
        // https://chillicream.com/docs/hotchocolate/get-started
        /// <summary>
        /// gets the pastebin requests from the given GUIDs.
        /// </summary>
        /// <param name="ids">the GUIDs of the wanted requests</param>
        /// <returns>a list of paste bin data requests</returns>
        public async Task<IEnumerable<PasteBinRequest>> GetPasteBinRequest(IEnumerable<Guid> ids)
        {
            try
            {
                var requests = await _pasteBinService.GetPasteBinRequestsByIdsAsync(ids, true);
                return requests;
            }
            catch (Exception ex)
            {
                await _logger.Error("an exception occurred in GraphQL.GetPasteBinRequest(...)");
                await _logger.LogObject("the exception", ex);
                return new List<PasteBinRequest>();
            }
        }
    }
}