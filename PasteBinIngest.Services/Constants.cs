namespace PasteBinIngest.Services
{
    internal class Constants
    {
        // User-Agent Constants
        public const string UserAgentHeader = "User-Agent";
        public const string UserAgentFirefox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";

        // HTML DOM Parsing Constants
        public const string Href = "href";
        public const string PasteBinTableSelector = "//table[@class='maintable']";
    }
}
