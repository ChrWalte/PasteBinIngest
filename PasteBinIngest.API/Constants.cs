namespace PasteBinIngest.API
{
    /// <summary>
    /// constants used in the APIs.
    /// </summary>
    internal static class Constants
    {
        // data constants
        public const string PastebinRawUrl = "https://pastebin.com/raw";
        public const string DataSaveLocation = "E:\\pastebin";
        public const string LogSaveLocation = "E:\\pastebin\\api.logs";

        // logging constants:
        public const string InitLogger = "initialized loggger";
        public const string InitRepoAndService = "initialized pastebin repository and service";
        public const string ExitedLog = "exited.\n";
    }
}