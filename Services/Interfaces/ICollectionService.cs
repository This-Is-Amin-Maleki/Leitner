using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Collection;

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
        Task<CollectionMiniDto> ReadCollectionNameAndStatusAsync(long id);
        Task<List<CollectionDto>> ReadPublishedCollectionsAsync();
        Task<List<CollectionDto>> ReadUnusedPublishedCollectionsAsync(long userId);
        Task<CollectionDto> ReadUserCollectionAsync(long id, long userId);
    }
}
