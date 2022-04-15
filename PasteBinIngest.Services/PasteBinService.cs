using HtmlAgilityPack;
using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Domain.Models;
using PasteBinIngest.Shared;

namespace PasteBinIngest.Services
{
    public class PasteBinService
    {
        private readonly string _pasteBinRawUrl;
        private readonly IPasteBinRepository _pasteBinRepository;
        private readonly Loggger _loggger;

        public PasteBinService(string pasteBinRawUrl, IPasteBinRepository pasteBinRepository, Loggger loggger)
        {
            _pasteBinRawUrl = pasteBinRawUrl;
            _pasteBinRepository = pasteBinRepository;
            _loggger = loggger;
        }

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
                    pasteBinRequest.PasteBinEntries.Add(pasteBinEntry);
                    await _loggger.Info($"added to pasteBinRequest.PasteBinEntries for a total count of {pasteBinRequest.PasteBinEntries.Count}");
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

        public async Task SavePasteBinRequestAsync(PasteBinRequest request)
        {
            await _loggger.Debug("starting to save request...");
            await _pasteBinRepository.SaveRequestAsync(request);
            await _loggger.Debug("finished saving request");
        }

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