using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Collection;
using System.Runtime.InteropServices;

namespace ServicesLeit.Interfaces
{
    public interface ICollectionService
    {
        Task AddCollectionAsync(CollectionCreationDto collectionViewModel);
        Task<BoxAddDto?> CheckExistCollectionAsync(long id, long userId);
        Task CheckStatusAsync(long id, string message);
        Task DeleteCollectionAsync(CollectionDto collectionViewModel);
        Task DeleteCollectionLimitedAsync(CollectionModifyDto model);
        Task EditCollectionAsync(CollectionModifyDto model);
        Task EditCollectionLimitedAsync(CollectionModifyDto model);
        Task<List<CollectionListDto>> ReadAllAsync();
        Task<List<CollectionListDto>> ReadAllAsync(long userId);
        Task<CollectionUnlimitedDto> ReadCollectionAsync(long id);
        Task<CollectionModifyDto> ReadCollectionDataAsync(long id);
        Task<CollectionModifyDto> ReadCollectionDataAsync(long id, long userId);
        Task<CollectionMiniDto> ReadCollectionNameAndStatusAsync(long id);
        IEnumerable<CollectionShowDto> ReadPublishedCollections(int count, [Optional] long? userId);
        Task<List<CollectionShowDto>> ReadUnusedPublishedCollectionsAsync(long userId);
        Task<CollectionDto> ReadUserCollectionAsync(long id, long userId);
    }
}