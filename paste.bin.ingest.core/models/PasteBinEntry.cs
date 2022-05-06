namespace paste.bin.ingest.core.models
{
    /// <summary>
    /// paste bin entry object holding raw entry data.
    /// </summary>
    public class PasteBinEntry
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Uri { get; set; }
        public DateTime Created { get; set; }
        public string? RawData { get; set; }

        /// <summary>
        /// empty constructor for object reading.
        /// </summary>
        public PasteBinEntry()
        { }

        /// <summary>
        /// new entry constructor for object creation.
        /// </summary>
        /// <param name="name">the name of the entry</param>
        /// <param name="uri">the URL of the entry on paste bin</param>
        /// <param name="rawData">the raw data of the entry</param>
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