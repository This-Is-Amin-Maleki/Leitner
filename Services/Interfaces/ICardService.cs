using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using SharedLeit;

namespace ServicesLeit.Interfaces
{
    public interface ICardService
    {
        Task AddCardAsync(CardDto model);
        Task BulkAddCardsAsync(List<Card> model);
        Task DeleteCardAsync(CardDto model);
        Task DeleteCardLimitedAsync(CardDto model);
        Task<CardDto> ReadCardAsync(long id);
        Task<CardCheckDto> ReadCardCheck(long collectionId, CardStatus? cardStatus, int skip = 0);
        Task<CardDto> ReadCardLimitedAsync(long id, long userId);
        Task<List<Card>> ReadCardsAsync(IEnumerable<long> cards);
        Task<(List<CardDto>, string)> ReadCardsAsync(long collectionId);
        Task<List<CardDto>> ReadCardsAsync(long lastCardId, long collectionId, int count, int skip = 0);
        Task<List<CardMiniDto>> ReadCardsLimitedAsync(long collectionId, long userId);
        Task<List<CardMiniUnlimitedDto>> ReadCardsUnlimitedAsync(long collectionId, CardStatus? state = null);
        Task TickAllCardsAsync(long collectionId);
        Task UpdateCardAsync(CardDto model);
        Task UpdateCardLimitedAsync(CardDto model);
        Task<CardCheckDto> UpdateStatusAndReadNextCardCheck(CardCheckStatusDto model);
    }
}
