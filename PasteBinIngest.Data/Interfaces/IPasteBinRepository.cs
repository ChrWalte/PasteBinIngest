using PasteBinIngest.Domain.Models;

namespace PasteBinIngest.Data.Interfaces
{
    public interface IPasteBinRepository
    {
        string[] GetUrlsFromFolderNames();
        Task SaveRequestAsync(PasteBinRequest request);
    }
}