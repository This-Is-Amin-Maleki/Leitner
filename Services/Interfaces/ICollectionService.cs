using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICollectionService
    {
        Task AddCollectionAsync(CollectionDTO collectionDTO);
        Task DeleteCollectionAsync(CollectionDTO collectionDTO);
        Task EditCollectionAsync(CollectionDTO collectionDTO);
        Task<CollectionDTO> GetCollectionAsync(long id);
        Task<List<CollectionDTO>> GetCollectionsAsync();
        Task<CollectionDTO> ReadCollectionAsync(long id);
        Task<List<CollectionDTO>> ReadCollectionsAsync();
    }
}
