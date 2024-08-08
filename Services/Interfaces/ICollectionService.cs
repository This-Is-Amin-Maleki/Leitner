using ModelsLeit.DTOs;
using ModelsLeit.DTOs.Collection;
using ModelsLeit.DTOs.User;
using ModelsLeit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLeit.Interfaces
{
    public interface ICollectionService
    {
        Task AddCollectionAsync(CollectionAddDto collectionViewModel);
        Task DeleteCollectionAsync(CollectionDto collectionViewModel);
        Task EditCollectionAsync(CollectionDto collectionViewModel);
        Task<CollectionDto> GetCollectionAsync(long id);
        Task<List<CollectionDto>> GetCollectionsAsync();
        Task<CollectionDto> ReadCollectionAsync(long id);
        Task<List<CollectionDto>> ReadCollectionsAsync();
    }
}
