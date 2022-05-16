namespace paste.bin.ingest.cmd
{
    /// <summary>
    /// constants used in the CLI.
    /// </summary>
    internal static class Constants
    {
        // data constants
        public const string PasteBinBaseUrl = "https://pastebin.com/archive";
        public const string PasteBinRawUrl = "https://pastebin.com/raw";

        // logging constants:
        public const string InitLogger = "initialized logger";
        public const string InitRepoAndService = "initialized pastebin repository and service";
        public const string StartedRequestLog = "starting pastebin request...";
        public const string FinishedRequestLog = "finished pastebin request";
        public const string ExitedLog = "exited.\n";
    }
}