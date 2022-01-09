namespace PasteBinIngest.Domain.Models
{
    public class PasteBinRequest
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<PasteBinEntry> PasteBinEntries { get; set; }

        public PasteBinRequest()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            PasteBinEntries = new List<PasteBinEntry>();
        }
    }
}