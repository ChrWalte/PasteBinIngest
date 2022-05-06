namespace paste.bin.ingest.core.data.DataTransferObjects
{
    /// <summary>
    /// paste bin request DTO.
    /// </summary>
    public class PasteBinRequestData
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid[]? EntryIds { get; set; }
    }
}