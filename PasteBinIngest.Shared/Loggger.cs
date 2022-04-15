using Newtonsoft.Json;

namespace PasteBinIngest.Shared
{
    public class Loggger
    {
        private readonly string _fileLocation;

        public Loggger(string fileLocation)
        {
            _fileLocation = fileLocation;
            Directory.CreateDirectory(_fileLocation);
        }

        public async Task Debug(string details)
            => await Log(details, "  DEBUG  ");

        public async Task Info(string details)
            => await Log(details, "  INFO   ");

        public async Task Warning(string details)
            => await Log(details, " WARNING ");

        public async Task Error(string details)
            => await Log(details, "  ERROR  ");

        public async Task LogObject(string details, object data)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            await Log($"{details} ~ {dataJson}", " OBJECT  ");
        }

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