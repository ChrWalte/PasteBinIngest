using Newtonsoft.Json;

namespace PasteBinIngest.Services
{
    public class Loggger
    {
        private readonly string _fileLocation;

        public Loggger(string fileLocation)
        {
            _fileLocation = fileLocation;
            Directory.CreateDirectory(_fileLocation);
        }

        public void Debug(string details)
            => Log(details, "DEBUG");

        public void Info(string details)
            => Log(details, "INFO");

        public void Warning(string details)
            => Log(details, "WARNING");

        public void Error(string details)
            => Log(details, "ERROR");

        public void LogObject(string details, object data)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            Log($"{details} ~ {dataJson}", "OBJECT");
        }

        private void Log(string details, string level)
        {
            var timestamp = DateTime.Now;
            var filename = timestamp.ToString("MMddyyyy") + ".log";
            var fullPath = Path.Combine(_fileLocation, filename);
            var fullLog = $"[{timestamp:hh:mm:ss:fff}] ~ [{level}]: {details}\n";

            try
            {
                Console.Write(fullLog);
                File.AppendAllText(fullPath, fullLog);
            }
            catch (Exception ex)
            {
                var exJson = JsonConvert.SerializeObject(ex);
                Console.WriteLine($"~EXCEPTION WHILE LOGGING: {exJson}");
                throw;
            }
        }
    }
}