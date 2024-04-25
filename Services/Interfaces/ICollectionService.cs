using Models.DTOs;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICollectionService
    {
        Task AddCollectionAsync(CollectionViewModel collectionViewModel);
        Task DeleteCollectionAsync(CollectionViewModel collectionViewModel);
        Task EditCollectionAsync(CollectionViewModel collectionViewModel);
        Task<CollectionDTO> GetCollectionAsync(long id);
        Task<List<CollectionDTO>> GetCollectionsAsync();
        Task<CollectionDTO> ReadCollectionAsync(long id);
        Task<List<CollectionDTO>> ReadCollectionsAsync();
    }
}
