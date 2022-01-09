using PasteBinIngest.Domain.Models;

namespace PasteBinIngest.Data.Interfaces
{
    public interface IPasteBinRepository
    {
        void SaveRequest(PasteBinRequest request);
        void SaveEntry(Guid requestId, PasteBinEntry entry);
        bool CheckEntryExists(string uri, string rawData);
    }
}