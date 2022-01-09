namespace PasteBinIngest.Data.DataTransferObjects
{
    public class PasteBinRequestData
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid[]? EntryIds { get; set; }
    }
}