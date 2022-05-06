using Microsoft.AspNetCore.Mvc;
using paste.bin.ingest.core.services;

namespace paste.bin.ingest.api.Controllers
{
    /// <summary>
    /// API controller for pastebin request data.
    /// </summary>
    [Route("request")]
    public class RequestController : ControllerBase
    {
        private readonly PasteBinService _pasteBinService;
        private readonly Loggger _loggger;

        /// <summary>
        /// initialize the pastebin request API controller.
        /// </summary>
        /// <param name="pasteBinService">the pastebin service to use</param>
        /// <param name="loggger">the logger to use</param>
        public RequestController(PasteBinService pasteBinService, Loggger loggger)
        {
            _pasteBinService = pasteBinService;
            _loggger = loggger;
        }

        /// <summary>
        /// gets the pastebin requests from the given GUIDs.
        /// </summary>
        /// <param name="ids">the GUIDs of the wanted requests</param>
        /// <returns>a list of pastebin data requests</returns>
        [HttpGet]
        public async Task<IActionResult> PasteBinRequestAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var requests = await _pasteBinService.GetPasteBinRequestsByIdsAsync(ids);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                await _loggger.Error("an exception occurred in Controller.PasteBinRequestAsync(...)");
                await _loggger.LogObject("the exception", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}