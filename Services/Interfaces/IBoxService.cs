using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Container;

namespace ServicesLeit.Interfaces
{
    public interface IBoxService
    {
        Task AddAsync(BoxAddDto model);
        Task DeleteAsync(long id, long userId);
        Task<long[]> GetAllCollectionIdAsync(long userId);
        Task<List<BoxMiniDto>> ReadAll4UserAsync(long userId);
        Task<List<BoxMiniDto>> ReadAllAsync();
        Task<List<BoxMiniDto>> ReadAllAsync(long userId);
        Task<List<BoxMiniDto>> ReadByCollectionAsync(long id);
        Task<List<BoxMiniDto>> ReadByCollectionAsync(long id, long userId);
        Task<CardDto> ReadNextCardAsync(long boxId, int num, long userId);
        Task<BoxReviewDto> ReviewAsync(long id, long userId);
        Task<ContainerStudyDto> StudyAsync(long id, long userId);
        Task<ContainerStudyDto> StudyFailAsync(ContainerStudiedDto model);
        Task UpdateAsync(ContainerStudiedDto model);
    }
}