namespace PasteBinIngest.Data.DataTransferObjects
{
    /// <summary>
    /// paste bin entry DTO.
    /// </summary>
    internal class PasteBinData
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string? Name { get; set; }
        public string? Uri { get; set; }
        public string? RawDataHash { get; set; }
        public DateTime Created { get; set; }
    }
}