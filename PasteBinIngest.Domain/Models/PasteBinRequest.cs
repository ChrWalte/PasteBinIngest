namespace PasteBinIngest.Domain.Models
{
    /// <summary>
    /// paste bin request object holding entry objects.
    /// </summary>
    public class PasteBinRequest
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? Url { get; set; }
        public List<PasteBinEntry>? PasteBinEntries { get; set; }

        /// <summary>
        /// empty constructor for object reading.
        /// </summary>
        public PasteBinRequest()
        { }

        /// <summary>
        /// new request constructor for object creation.
        /// </summary>
        /// <param name="url">the URL used in the request</param>
        public PasteBinRequest(string url)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            Url = url;
            PasteBinEntries = new List<PasteBinEntry>();
        }

        /// <summary>
        /// full constructor for object reading.
        /// </summary>
        /// <param name="id">existing id</param>
        /// <param name="timeStamp">existing timeStamp</param>
        /// <param name="url">existing URL</param>
        /// <param name="pasteBinEntries">existing entries</param>
        public PasteBinRequest(Guid id, DateTime timeStamp, string url, List<PasteBinEntry> pasteBinEntries)
        {
            Id = id;
            TimeStamp = timeStamp;
            Url = url;
            PasteBinEntries = pasteBinEntries;
        }
    }
}