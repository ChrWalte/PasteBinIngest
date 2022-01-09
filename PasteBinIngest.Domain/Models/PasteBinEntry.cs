namespace PasteBinIngest.Domain.Models
{
    public class PasteBinEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public DateTime Created { get; set; }
        public string RawData { get; set; }

        public PasteBinEntry(string name, string uri, string rawData)
        {
            Id = Guid.NewGuid();
            Name = name;
            Uri = uri;
            Created = DateTime.Now;
            RawData = rawData;
        }
    }
}