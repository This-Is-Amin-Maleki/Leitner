using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLeit.Interfaces
{
    public interface ICardService
    {
        Task AddCardAsync(CardDto model);
        Task BulkAddCardsAsync(List<Card> model);
        Task DeleteCardAsync(CardDto model);
        Task DeleteCardLimitedAsync(CardDto model);
        Task<CardDto> ReadCardAsync(long id);
        Task<CardDto> ReadCardLimitedAsync(long id, long userId);
        Task<List<Card>> ReadCardsAsync(IEnumerable<long> cards);
        Task<(List<CardDto>, string)> ReadCardsAsync(long collectionId);
        Task<List<CardDto>> ReadCardsAsync(long lastCardId, long collectionId, int count, int skip = 0);
        Task<List<CardDto>> ReadCardsLimitedAsync(long collectionId, long userId);
        Task<List<CardDto>> ReadCardsUnlimitedAsync(long collectionId);
        Task UpdateCardAsync(CardDto model);
        Task UpdateCardLimitedAsync(CardDto model);
    }
}
