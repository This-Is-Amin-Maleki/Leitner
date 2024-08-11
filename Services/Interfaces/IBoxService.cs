using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Container;

namespace ServicesLeit.Interfaces
{
    public interface IBoxService
    {
       Task<List<BoxMiniDto>> ReadAllAsync();
       Task<List<BoxMiniDto>> GetAllAsync();
       Task<List<BoxMiniDto>> ReadByCollectionAsync(long id);
       Task<List<BoxMiniDto>> GetByCollectionAsync(long id);
       Task<BoxMiniDto> ReadAsync(long id);
       Task<BoxMiniDto> GetAsync(long id);
       Task AddAsync(BoxAddDto model);

       Task<BoxReviewDto> ReviewAsync(long id);

       Task<ContainerStudyDto> StudyAsync(long id);
       Task UpdateAsync(ContainerStudiedDto model);
       Task<ContainerStudyDto> StudyFailAsync(ContainerStudiedDto model);
       Task DeleteAsync(long id);

        Task<CardDto> ReadNextCardAsync(long boxId, int num);
    }
}