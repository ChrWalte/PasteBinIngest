using System.Security.Cryptography;
using HtmlAgilityPack;
using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.models;

namespace paste.bin.ingest.core.services
{
    /// <summary>
    /// the pastebin service that handles all things pastebin logic.
    /// </summary>
    public class PasteBinService
    {
        private readonly string _pasteBinRawUrl;
        private readonly IPasteBinRepository _pasteBinRepository;
        private readonly Loggger _loggger;

        /// <summary>
        /// initialize the pastebin service with a starting pastebin URL, a data repository, and a logger.
        /// </summary>
        /// <param name="pasteBinRawUrl">a valid pastebin URL to pull data from</param>
        /// <param name="pasteBinRepository">a pastebin data repository</param>
        /// <param name="loggger">a logger to log messages</param>
        public PasteBinService(string pasteBinRawUrl, IPasteBinRepository pasteBinRepository, Loggger loggger)
        {
            _pasteBinRawUrl = pasteBinRawUrl;
            _pasteBinRepository = pasteBinRepository;
            _loggger = loggger;
        }

        /// <summary>
        /// get pastebin request objects from the data repository by GUIDs.
        /// optional flag for including entry objects with request objects.
        /// </summary>
        /// <param name="ids">the GUIDs of the requests wanted</param>
        /// <param name="withEntries">flag to include entries or not</param>
        /// <returns> a list of pastebin request objects</returns>
        public async Task<IEnumerable<PasteBinRequest>> GetPasteBinRequestsByIdsAsync(IEnumerable<Guid> ids, bool withEntries = false)
        {
            var requests = await _pasteBinRepository.GetRequestsByIdsAsync(ids, withEntries);
            return requests;
        }

        /// <summary>
        /// send a pastebin request and get entry data.
        /// </summary>
        /// <param name="basePasteBinUrl">the pastebin URL to extract data from</param>
        /// <returns>a pastebin request object with entry objects</returns>
        public async Task<PasteBinRequest> SendPasteBinRequestAsync(string basePasteBinUrl)
        {
            var pasteBinRequest = new PasteBinRequest(basePasteBinUrl);
            var rawHtml = await GetHtmlDocument(basePasteBinUrl);
            await _loggger.Debug("obtained initial pastebin html document");

            // extracts all links from pastebin archive table
            var dataLinksOrNulls = rawHtml.DocumentNode.SelectSingleNode(Constants.PasteBinTableSelector)
                .Descendants()
                .Where(node => node.GetAttributeValue(Constants.Href, null) != null)
                .ToList();
            await _loggger.Info($"found {dataLinksOrNulls.Count} pastebin dataLinksOrNulls");

            await _loggger.Debug("starting pastebin entry extract...");
            foreach (var dataLink in dataLinksOrNulls)
            {
                // no uri, continue
                var uri = dataLink?.GetAttributeValue(Constants.Href, null);
                if (uri == null) { continue; }

                // no inner text, continue
                var title = dataLink?.InnerText;
                if (title == null) { continue; }

                // skip syntax archive links, build with?
                if (uri.Contains("archive")) { continue; }

                try
                {
                    await _loggger.Info($"found [{uri}][{title}]");
                    var rawDataUrl = _pasteBinRawUrl + uri;
                    var rawData = await GetHtmlDocument(rawDataUrl);
                    var pasteBinEntry = new PasteBinEntry(title, uri, rawData.Text);
                    pasteBinRequest.PasteBinEntries?.Add(pasteBinEntry);
                    await _loggger.Info($"added to pasteBinRequest.PasteBinEntries for a total count of {pasteBinRequest.PasteBinEntries?.Count}");

                    // sleep a random amount of time, to throw off pastebin servers...
                    var randomNumber = RandomNumberGenerator.GetInt32(20000);
                    await _loggger.Info($"sleeping for {(10000 + randomNumber) / 1000} seconds...");
                    Thread.Sleep(millisecondsTimeout: 10000 + randomNumber);
                }
                catch (Exception ex)
                {
                    await _loggger.Error("error getting pastebin entry data, will continue");
                    await _loggger.LogObject("exception!", ex);
                }
            }
            await _loggger.Debug("finished pastebin entry extract.");
            return pasteBinRequest;
        }

        /// <summary>
        /// save the pastebin request using the data repository.
        /// </summary>
        /// <param name="request">the request to save</param>
        public async Task SavePasteBinRequestAsync(PasteBinRequest request)
        {
            await _loggger.Debug("starting to save request...");
            await _pasteBinRepository.SaveRequestAsync(request);
            await _loggger.Debug("finished saving request");
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