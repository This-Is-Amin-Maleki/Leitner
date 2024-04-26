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
        Task<CollectionViewModel> GetCollectionAsync(long id);
        Task<List<CollectionViewModel>> GetCollectionsAsync();
        Task<CollectionViewModel> ReadCollectionAsync(long id);
        Task<List<CollectionViewModel>> ReadCollectionsAsync();
    }
}
