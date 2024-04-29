using ModelsLeit.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLeit.Interfaces
{
    public interface ICardService
    {
        Task AddCardAsync(CardViewModel cardViewModel);
        Task DeleteCardAsync(CardViewModel cardViewModel);
        Task EditCardAsync(CardViewModel cardViewModel);
        Task<CardViewModel> GetCardAsync(long id);
        Task<(List<CardViewModel>,string)> GetCardsAsync(long collectionId);
        Task<CardViewModel> ReadCardAsync(long id);
        Task<(List<CardViewModel>,string)> ReadCardsAsync(long collectionId);
    }
}
