using HtmlAgilityPack;
using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.models;
using System.Security.Cryptography;

namespace paste.bin.ingest.core.services
{
    /// <summary>
    /// the paste bin service that handles all things paste bin logic.
    /// </summary>
    public class PasteBinService
    {
        private readonly string _pasteBinRawUrl;
        private readonly IPasteBinRepository _pasteBinRepository;
        private readonly Logger _logger;

        /// <summary>
        /// initialize the paste bin service with a starting paste bin URL, a data repository, and a logger.
        /// </summary>
        /// <param name="pasteBinRawUrl">a valid paste bin URL to pull data from</param>
        /// <param name="pasteBinRepository">a paste bin data repository</param>
        /// <param name="logger">a logger to log messages</param>
        public PasteBinService(string pasteBinRawUrl, IPasteBinRepository pasteBinRepository, Logger logger)
        {
            _pasteBinRawUrl = pasteBinRawUrl;
            _pasteBinRepository = pasteBinRepository;
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetAllPasteBinRequestGuidsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var requestGuids = await _pasteBinRepository.GetRequestIdsFromFileNamesAsync();
            if (fromDate == null && toDate == null)
                return requestGuids;
            fromDate ??= DateTime.MinValue;
            toDate ??= DateTime.MaxValue;

            var requestData = (await _pasteBinRepository.GetRequestsByIdsAsync(requestGuids)).ToList();
            var filteredRequestData =
                requestData.Where(request => request.TimeStamp >= fromDate && request.TimeStamp <= toDate);
            var filteredRequestGuids = filteredRequestData.Select(request => request.Id);

            return filteredRequestGuids;
        }

        /// <summary>
        /// get paste bin request objects from the data repository by GUIDs.
        /// optional flag for including entry objects with request objects.
        /// </summary>
        /// <param name="ids">the GUIDs of the requests wanted</param>
        /// <param name="withEntries">flag to include entries or not</param>
        /// <returns> a list of paste bin request objects</returns>
        /// <returns> a list of paste bin request objects</returns>
        public async Task<IEnumerable<PasteBinRequest>> GetPasteBinRequestsByIdsAsync(IEnumerable<Guid> ids, bool withEntries = false)
        {
            await _logger.LogObject("get paste bin requests by ids", ids);
            var requests = await _pasteBinRepository.GetRequestsByIdsAsync(ids, withEntries);

            await _logger.Debug("got paste bin requests");
            return requests;
        }

        /// <summary>
        /// send a paste bin request and get entry data.
        /// </summary>
        /// <param name="basePasteBinUrl">the paste bin URL to extract data from</param>
        /// <returns>a paste bin request object with entry objects</returns>
        public async Task<PasteBinRequest> SendPasteBinRequestAsync(string basePasteBinUrl)
        {
            var pasteBinRequest = new PasteBinRequest(basePasteBinUrl);
            var rawHtml = await GetHtmlDocument(basePasteBinUrl);
            await _logger.Debug("obtained initial paste bin html document");

            // extracts all links from paste bin archive table
            var dataLinksOrNulls = rawHtml.DocumentNode.SelectSingleNode(Constants.PasteBinTableSelector)
                .Descendants()
                .Where(node => node.GetAttributeValue(Constants.Href, null) != null)
                .ToList();
            await _logger.Info($"found {dataLinksOrNulls.Count} paste bin dataLinksOrNulls");

            await _logger.Debug("starting paste bin entry extract...");
            foreach (var dataLink in dataLinksOrNulls)
            {
                // no URI, continue
                var uri = dataLink?.GetAttributeValue(Constants.Href, null);
                if (uri == null) { continue; }

                // no inner text, continue
                var title = dataLink?.InnerText;
                if (title == null) { continue; }

                // skip syntax archive links, build with?
                if (uri.Contains("archive")) { continue; }

                try
                {
                    await _logger.Info($"found [{uri}][{title}]");
                    var rawDataUrl = _pasteBinRawUrl + uri;
                    var rawData = await GetHtmlDocument(rawDataUrl);
                    var pasteBinEntry = new PasteBinEntry(title, uri, rawData.Text);
                    pasteBinRequest.PasteBinEntries?.Add(pasteBinEntry);
                    await _logger.Info($"added to pasteBinRequest.PasteBinEntries for a total count of {pasteBinRequest.PasteBinEntries?.Count}");

                    // sleep a random amount of time, to throw off paste bin servers...
                    var randomNumber = RandomNumberGenerator.GetInt32(20000);
                    await _logger.Info($"sleeping for {(10000 + randomNumber) / 1000} seconds...");
                    Thread.Sleep(millisecondsTimeout: 10000 + randomNumber);
                }
                catch (Exception ex)
                {
                    await _logger.Error("error getting paste bin entry data, will continue");
                    await _logger.LogObject("exception!", ex);
                }
            }
            await _logger.Debug("finished paste bin entry extract.");
            return pasteBinRequest;
        }

        /// <summary>
        /// save the paste bin request using the data repository.
        /// </summary>
        /// <param name="request">the request to save</param>
        public async Task SavePasteBinRequestAsync(PasteBinRequest request)
        {
            await _logger.Debug("starting to save request...");
            await _pasteBinRepository.SaveRequestAsync(request);
            await _logger.Debug("finished saving request");
        }

        /// <summary>
        /// gets the raw HTML document from the URL and returns it.
        /// </summary>
        /// <param name="url">the URL to get the HTML from</param>
        /// <returns></returns>
        private static async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            // create the request
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Constants.UserAgentHeader, Constants.UserAgentFirefox);

            // send request, get response
            using var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);

            // read response as stream
            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            // raw html code
            var rawHtml = new HtmlDocument();
            rawHtml.LoadHtml(result);

            return rawHtml;
        }
    }
}