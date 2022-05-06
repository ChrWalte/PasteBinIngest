using Newtonsoft.Json;

namespace paste.bin.ingest.core.services
{
    /// <summary>
    /// a logger that writes messages to the console and a file.
    /// </summary>
    public class Logger
    {
        private readonly string _fileLocation;

        /// <summary>
        /// initialize the logger and tell it where to store the log files.
        /// </summary>
        /// <param name="fileLocation">the file location of the log files</param>
        public Logger(string fileLocation)
        {
            _fileLocation = fileLocation;
            Directory.CreateDirectory(_fileLocation);
        }

        /// <summary>
        /// log the details as DEBUG.
        /// </summary>
        /// <param name="details">the log message</param>
        /// <returns>nothing, writes log to console and file</returns>
        public async Task Debug(string details)
            => await Log(details, "  DEBUG  ");

        /// <summary>
        /// log the details as INFO.
        /// </summary>
        /// <param name="details">the log message</param>
        /// <returns>nothing, writes log to console and file</returns>
        public async Task Info(string details)
            => await Log(details, "  INFO   ");

        /// <summary>
        /// log the details as a WARNING.
        /// </summary>
        /// <param name="details">the log message</param>
        /// <returns>nothing, writes log to console and file</returns>
        public async Task Warning(string details)
            => await Log(details, " WARNING ");

        /// <summary>
        /// log the details as an ERROR.
        /// </summary>
        /// <param name="details">the log message</param>
        /// <returns>nothing, writes log to console and file</returns>
        public async Task Error(string details)
            => await Log(details, "  ERROR  ");

        /// <summary>
        /// log the JSON string of any object.
        /// </summary>
        /// <param name="details">the message next to the object</param>
        /// <param name="data">the object data</param>
        /// <returns>nothing, writes log to console and file</returns>
        public async Task LogObject(string details, object data)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            await Log($"{details} ~ {dataJson}", " OBJECT  ");
        }

        /// <summary>
        /// private generic log function that writes to the console and to a file.
        /// </summary>
        /// <param name="details">the log details to write</param>
        /// <param name="level">the log level</param>
        /// <returns>nothing, writes log to console and file</returns>
        private async Task Log(string details, string level)
        {
            var timestamp = DateTime.Now;
            var filename = timestamp.ToString("MMddyyyy") + ".log";
            var fullPath = Path.Combine(_fileLocation, filename);
            var fullLog = $"[{timestamp:hh:mm:ss:fff}] ~ [{level}]: {details}\n";

            try
            {
                Console.Write(fullLog);
                await File.AppendAllTextAsync(fullPath, fullLog);
            }
            catch (Exception ex)
            {
                var exJson = JsonConvert.SerializeObject(ex);
                Console.WriteLine($"[~EXCEPTION WHILE LOGGING]: {exJson}");
                throw;
            }
        }
    }
}