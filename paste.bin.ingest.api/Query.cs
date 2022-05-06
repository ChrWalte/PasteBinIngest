using paste.bin.ingest.core.models;
using paste.bin.ingest.core.services;

namespace paste.bin.ingest.api
{
    /// <summary>
    /// GraphQL controller for pastebin request data.
    /// </summary>
    public class Query
    {
        private readonly PasteBinService _pasteBinService;
        private readonly Loggger _loggger;

        /// <summary>
        /// initialize the pastebin request GraphQL controller.
        /// </summary>
        /// <param name="pasteBinService">the pastebin service to use</param>
        /// <param name="loggger">the logger to use</param>
        public Query(PasteBinService pasteBinService, Loggger loggger)
        {
            _pasteBinService = pasteBinService;
            _loggger = loggger;
        }

        // resources:
        // https://dotnetthoughts.net/getting-started-with-graphql-aspnetcore/
        // https://chillicream.com/docs/hotchocolate/get-started
        /// <summary>
        /// gets the pastebin requests from the given GUIDs.
        /// </summary>
        /// <param name="ids">the GUIDs of the wanted requests</param>
        /// <returns>a list of pastebin data requests</returns>
        public async Task<IEnumerable<PasteBinRequest>> GetPasteBinRequest(IEnumerable<Guid> ids)
        {
            try
            {
                var requests = await _pasteBinService.GetPasteBinRequestsByIdsAsync(ids, true);
                return requests;
            }
            catch (Exception ex)
            {
                await _loggger.Error("an exception occurred in GraphQL.GetPasteBinRequest(...)");
                await _loggger.LogObject("the exception", ex);
                return new List<PasteBinRequest>();
            }
        }
    }
}