using Microsoft.AspNetCore.Mvc;
using paste.bin.ingest.core.services;

namespace paste.bin.ingest.api.Controllers
{
    /// <summary>
    /// API controller for paste bin request data.
    /// </summary>
    [Route("request")]
    public class RequestController : ControllerBase
    {
        private readonly PasteBinService _pasteBinService;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the paste bin request API controller.
        /// </summary>
        /// <param name="pasteBinService">the paste bin service to use</param>
        /// <param name="logger">the logger to use</param>
        public RequestController(PasteBinService pasteBinService, Logger logger)
        {
            _pasteBinService = pasteBinService;
            _logger = logger;
        }

        /// <summary>
        /// gets the paste bin request guids from the folder names
        /// </summary>
        /// <param name="fromDate">(optional) only get requests after fromDate</param>
        /// <param name="toDate">(optional) only get requests before toDate</param>
        /// <returns></returns>
        [HttpGet("ids")]
        public async Task<IActionResult> PasteBinRequestGuidAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var requestGuids = await _pasteBinService.GetAllPasteBinRequestGuidsAsync(fromDate, toDate);
                return Ok(requestGuids);
            }
            catch (Exception ex)
            {
                await _logger.Error("an exception occurred in Controller.PasteBinRequestGuidAsync(...)");
                await _logger.LogObject("the exception", ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// gets the paste bin requests from the given GUIDs.
        /// </summary>
        /// <param name="ids">the GUIDs of the wanted requests</param>
        /// <returns>a list of paste bin data requests</returns>
        [HttpGet("data")]
        public async Task<IActionResult> PasteBinRequestAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var requests = await _pasteBinService.GetPasteBinRequestsByIdsAsync(ids);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                await _logger.Error("an exception occurred in Controller.PasteBinRequestAsync(...)");
                await _logger.LogObject("the exception", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}