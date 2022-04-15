namespace PasteBinIngest.Domain.Models
{
    public class PasteBinRequest
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Url { get; set; }
        public List<PasteBinEntry> PasteBinEntries { get; set; }

        public PasteBinRequest(string url)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            Url = url;
            PasteBinEntries = new List<PasteBinEntry>();
        }
    }
}