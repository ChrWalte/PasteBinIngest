namespace PasteBinIngest.Services
{
    /// <summary>
    /// constants used in the services.
    /// </summary>
    internal class Constants
    {
        // User-Agent Constants
        public const string UserAgentHeader = "User-Agent";
        public const string UserAgentFirefox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0";
        // previous user agents:
        // Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0
        // Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0
        // Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0

        // HTML DOM Parsing Constants
        public const string Href = "href";
        public const string PasteBinTableSelector = "//table[@class='maintable']";
    }
}