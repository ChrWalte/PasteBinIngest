namespace paste.bin.ingest.cmd
{
    /// <summary>
    /// constants used in the CLI.
    /// </summary>
    internal static class Constants
    {
        // data constants
        public const string PastebinBaseUrl = "https://pastebin.com/archive";

        public const string PastebinRawUrl = "https://pastebin.com/raw";
        public const string DataSaveLocation = "E:\\pastebin";
        public const string LogSaveLocation = "E:\\pastebin\\cli.logs";

        // logging constants:
        public const string InitLogger = "initialized loggger";

        public const string InitRepoAndService = "initialized pastebin repository and service";
        public const string StartedRequestLog = "starting pastebin request...";
        public const string FinishedRequestLog = "finished pastebin request";
        public const string ExitedLog = "exited.\n";
    }
}