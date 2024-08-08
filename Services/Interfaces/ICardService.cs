using ModelsLeit.DTOs.Card;
using ModelsLeit.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLeit.Interfaces
{
    public interface ICardService
    {
        Task AddCardAsync(CardDto cardViewModel);
        Task DeleteCardAsync(CardDto cardViewModel);
        Task UpdateCardAsync(CardDto cardViewModel);
        Task<CardDto> GetCardAsync(long id);
        Task<(List<CardDto>,string)> GetCardsAsync(long collectionId);
        Task<CardDto> ReadCardAsync(long id);
        Task<(List<CardDto>,string)> ReadCardsAsync(long collectionId);
    }
}
