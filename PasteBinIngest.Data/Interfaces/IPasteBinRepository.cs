﻿using PasteBinIngest.Domain.Models;

namespace PasteBinIngest.Data.Interfaces
{
    /// <summary>
    /// pastebin data repository interface.
    /// </summary>
    public interface IPasteBinRepository
    {
        Task<IEnumerable<Guid>> GetRequestIdsFromFileNamesAsync();

        Task<IEnumerable<Guid>> GetEntryIdsFromFolderNamesAsync();

        Task<IEnumerable<PasteBinRequest>> GetRequestsByIdsAsync(IEnumerable<Guid> ids, bool withEntries = false);

        Task<IEnumerable<PasteBinEntry>> GetEntriesByIdsAsync(IEnumerable<Guid> ids, bool withRawData = false);

        Task SaveRequestAsync(PasteBinRequest request);
    }
}