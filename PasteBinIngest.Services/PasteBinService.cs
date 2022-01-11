using HtmlAgilityPack;
using PasteBinIngest.Domain.Models;

namespace PasteBinIngest.Services
{
    public class PasteBinService
    {
        private readonly string _pasteBinRawUrl;
        private readonly Loggger _loggger;

        public PasteBinService(string pasteBinRawUrl, Loggger loggger)
        {
            _pasteBinRawUrl = pasteBinRawUrl;
            _loggger = loggger;
        }

        public PasteBinRequest GetRequest(string pasteBinUrl)
        {
            _loggger.Debug("pastebin request started->");

            var pasteBinRequest = new PasteBinRequest();
            var rawHtml = GetHtmlDocument(pasteBinUrl);

            // extracts all links from pastebin archive table
            var dataLinksOrNulls = rawHtml.DocumentNode.SelectSingleNode(Constants.PasteBinTableSelector)
                .Descendants()
                .Where(node => node.GetAttributeValue(Constants.Href, null) != null)
                .ToList();

            _loggger.Info($"found {dataLinksOrNulls.Count} total links");

            foreach (var linkOrNull in dataLinksOrNulls)
            {
                // no uri, continue
                var uri = linkOrNull?.GetAttributeValue(Constants.Href, null);
                if (uri == null) { continue; }

                // no inner text, continue
                var title = linkOrNull?.InnerText;
                if (title == null) { continue; }

                // skip syntax archive links, build with?
                if (uri.Contains("archive")) { continue; }

                var rawDataUrl = _pasteBinRawUrl + uri;
                var rawData = GetHtmlDocument(rawDataUrl).Text;
                
                var pasteBinEntry = new PasteBinEntry(title, uri, rawData);
                pasteBinRequest.PasteBinEntries.Add(pasteBinEntry);
            }

            _loggger.Info($"found {pasteBinRequest.PasteBinEntries.Count} total entries");
            _loggger.Debug("->pastebin request finished");

            return pasteBinRequest;
        }

        private static HtmlDocument GetHtmlDocument(string url)
        {
            // create the request
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Constants.UserAgentHeader, Constants.UserAgentFirefox);

            // send request, get response
            using var httpClient = new HttpClient();
            var response = httpClient.Send(request);

            // read response as stream
            using var stream = response.Content.ReadAsStream();
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            // raw html code
            var rawHtml = new HtmlDocument();
            rawHtml.LoadHtml(result);

            return rawHtml;
        }
    }
}