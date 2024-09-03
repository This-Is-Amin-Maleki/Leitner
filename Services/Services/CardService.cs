using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using EFCore.BulkExtensions;
using Microsoft.Extensions.Logging;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Collection;
using SharedLeit;
using ServicesLeit.Interfaces;

namespace ServicesLeit.Services
{
    public class CardService : ICardService
    {
        private readonly ILogger<BoxService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly CollectionService _collection;
        public CardService(ApplicationDbContext dbContext, ILogger<BoxService> logger, CollectionService collection)
        {
            _logger = logger;
            _dbContext = dbContext;
            _collection = collection;
        }
        public async Task<List<CardMiniDto>> ReadCardsLimitedAsync(long collectionId, long userId)
        {//use auto mapper
#warning if invalid card send 2 save reyurn without collection
            var cards = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                .Select(x => new CardMiniDto()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    HasMp3 = x.HasMp3,
                    Status = x.Status,
                    Collection = new CollectionMiniDto()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                        Status = x.Collection.Status,
                    }
                })
                .ToListAsync();
            return cards;
        }
        public async Task<List<CardMiniUnlimitedDto>> ReadCardsUnlimitedAsync(long collectionId, CardStatus? state = null)
        {//use auto mapper
#warning if invalid card send 2 save return without collection
            return await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId &&
                (state == null || x.Status == state))
                .Select(x => new CardMiniUnlimitedDto()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    Answer = x.Answer,
                    HasMp3 = x.HasMp3,
                    Status = x.Status,
                    Collection = new CollectionMiniDto()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                        Status = x.Collection.Status,
                    }
                })
                .ToListAsync();
        }
        public async Task<(List<CardDto>, string)> ReadCardsAsync(long collectionId)
        {//use auto mapper
#warning if invalid card send 2 save reyurn without collection
            string collectionName = "";
            List<CardDto> list = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId)
                .Select(x => new CardDto()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    Answer = x.Answer,
                    Description = x.Description,
                    HasMp3 = x.HasMp3,
                    Status = x.Status,
                    Collection = new CollectionMiniDto()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                    }
                })
                .ToListAsync();
            collectionName = (list.Count > 0) ? list.First().Collection.Name! : await GetCollectionName(collectionId);
            return (list, collectionName);
        }
#warning check performance
        public async Task<CardDto> ReadCardLimitedAsync(long id, long userId)
        {
            var card = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            return card is null ?
                CreateEmptyCardViewModel() :
                MapCardToViewModel(card);
        }
        public async Task<CardDto> ReadCardAsync(long id)
        {
            var card = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return card is null ?
                CreateEmptyCardViewModel() :
                MapCardToViewModel(card);
        }

        public async Task<List<CardDto>> ReadCardsAsync(long lastCardId, long collectionId, int count, int skip = 0)
        {
            //get new cardsArray
            return await _dbContext.Cards
                .AsNoTracking()
                .Where(x => x.Id > lastCardId && x.CollectionId == collectionId)
                .Select(x => new CardDto
                {
                    Id = x.Id,
                    Answer = x.Answer,
                    Ask = x.Ask,
                    Description = x.Description,
                    HasMp3 = x.HasMp3,
                })
                .Skip(skip)
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<Card>> ReadCardsAsync(IEnumerable<long> cards)
        {
            return await _dbContext.Cards
                .AsNoTracking()
                .Where(x => cards.Contains(x.Id))
                .ToListAsync();
        }
        public async Task<CardsListStatusDto> ReadCardsByStatusAsync(long id)
        {
            CardsListStatusDto? checkList = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == id && x.Status == CardStatus.Submitted)
                .GroupBy(x => new { x.Collection.Id, x.Collection.Name })
                .Select(group => new CardsListStatusDto
                {
                    CollectionName = group.Key.Name,
                    Id = group.Key.Id,
                    Submitted = group.Select(x => new CardCheckDto
                    {
                        Id = x.Id,
                        Answer = x.Answer,
                        Description = x.Description,
                        Ask = x.Ask,
                        HasMp3 = x.HasMp3,
                        Status = x.Status,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (checkList is null)
            {
                throw new Exception("Collection Not Found");
            }
            if (checkList.Submitted.Count is 0)
            {
                throw new Exception("Any Cards to Check");
            }
            return checkList;
        }
        public async Task<CardCheckDto> ReadCardCheck(long collectionId, CardStatus? cardStatus, int skip = 0)
        {
            cardStatus = cardStatus is null or CardStatus.Approved ? CardStatus.Submitted : cardStatus;
            var card = await _dbContext.Cards
                .Include(x=>x.Collection)
                .Skip(skip)
                .FirstOrDefaultAsync(x =>
                    x.CollectionId == collectionId &&
                    x.Status == cardStatus
                );

            if (card is null)
            {
                throw new Exception("No more cards available");
            }

            return ToCardCheckDto(card);
        }

        public async Task<CardCheckDto> UpdateStatusAndReadNextCardCheck(CardCheckStatusDto model)
        {
            var output = new CardCheckDto();
#warning catch!!
            try
            {
                Card[]? card = await _dbContext.Cards
                    .Where(x =>
                        x.CollectionId == model.CollectionId
                        && x.Status == model.DefaultStatus)
                    .Skip(model.Skip)
                    .Take(2)
                    .ToArrayAsync();

                if (card is null)
                {
                    throw new Exception("Card not found");
                }

                card[0].Status = model.Status;
                await _dbContext.SaveChangesAsync();

                if (card[1] is not null)
                {
                    output = ToCardCheckDto(card[1]);
                }
            }
            catch (Exception ex)
            {

            }
            return output;
        }

        public async Task TickAllCardsAsync(long collectionId)
        {
            var cards = await _dbContext.Cards
                .Where(x =>
                    x.CollectionId == collectionId &&
                    x.Status != CardStatus.Approved)
                .ToListAsync();
            if (cards is null)
            {
                throw new Exception("No cards were found.");
            }
            cards.ForEach(x => x.Status = CardStatus.Approved);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddCardAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not add any cards to the published collection!");
            var card = MapViewModelToCard(model);
            card.Id = 0;
            card.Status = CardStatus.Submitted;

            try
            {
                await _dbContext.Cards.AddAsync(card);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task BulkAddCardsAsync(List<Card> model)
        {
            await _collection.CheckStatusAsync(model.First().CollectionId, "Can not add any cards to the published collection!");
            await _dbContext.BulkInsertAsync(model);
        }
        public async Task UpdateCardLimitedAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not update any card of the published collection!");
            var card = MapViewModelToCard(model);

            var oldCard = await _dbContext.Cards
                .AsNoTracking()
                .FirstAsync(x => x.Id == model.Id && x.UserId == model.UserId);
            if (card.Status == CardStatus.Blocked)
            {
                throw new Exception("The Card has been blocked!");
            }
            card.Status = CardStatus.Submitted;
            if (oldCard is null)
            {
                throw new Exception("Not Found");
            }

#warning catch!!
            try
            {
                _dbContext.Cards.Update(card);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task UpdateCardAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not update any card of the published collection!");
            var card = MapViewModelToCard(model);

            var oldCard = await _dbContext.Cards
                .AsNoTracking()
                .FirstAsync(x => x.Id == model.Id);

            if (oldCard is null)
            {
                throw new Exception("Not Found");
            }

            card.UserId = oldCard.UserId;
#warning catch!!
            try
            {
                _dbContext.Cards.Update(card);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteCardLimitedAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not delete any card of the published collection!");
            var card = MapViewModelToCard(model);

            var hasCard = await _dbContext.Cards
                .AnyAsync(x => x.Id == model.Id && x.UserId == model.UserId);

            if (hasCard is false)
            {
                throw new Exception("Not Found");
            }

#warning catch!!
            try
            {
                _dbContext.Cards.Remove(card);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteCardAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not delete any card of the published collection!");
            var card = MapViewModelToCard(model);

            var hasCard = await _dbContext.Cards
                .AnyAsync(x => x.Id == model.Id && x.Status != CardStatus.Approved);

            if (hasCard is false)
            {
                throw new Exception("Not Found");
            }

#warning catch!!
            try
            {
                _dbContext.Cards.Remove(card);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
        }
        public async Task<CardCheckedResultDto> BulkUpdateCardsStatusAsync(CardsArrayStatusDto model)
        {
            var allCards = (model.Rejected ?? [])
                .Concat(model.Approved ?? [])
                .Concat(model.Blocked ?? [])
                .ToArray();

            var cards = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => allCards.Contains(x.Id))
                /*.Select(x=> new CardForCheckDto
                {
                     Id = x.Id,
                      Status = x.Status,
                      Collection = new CollectionMiniDto
                      {
                           Id = x.CollectionId,
                           Name = x.Collection.Name
                      }
                })*/
                .ToListAsync();

            if (cards is null)
            {
                throw new Exception("Not Found");
            }

            if (cards.Count is 0)
            {
                throw new Exception("Any Cards");
            }

            if (model.Approved != null && model.Approved.Length > 0)
                cards.Where(x => model.Approved.Contains(x.Id)).ToList().ForEach(x => x.Status = CardStatus.Approved);

            if (model.Rejected != null && model.Approved.Length > 0)
                cards.Where(x => model.Rejected.Contains(x.Id)).ToList().ForEach(x => x.Status = CardStatus.Rejected);

            if (model.Blocked != null && model.Approved.Length > 0)
                cards.Where(x => model.Blocked.Contains(x.Id)).ToList().ForEach(x => x.Status = CardStatus.Blocked);
            /*
            List<Card> updateCards = cards
                .Select(x => new Card
                    {
                        Id = x.Id,
                        Status = x.Status
                    })
                .ToList();
            */
            await _dbContext.BulkUpdateAsync(cards);

            var firstCard = cards.FirstOrDefault();

            CardCheckedResultDto output = new()
            {
                CheckedCards = allCards.Length,
                Collection = new CollectionMiniDto
                {
                    Id = firstCard.Collection.Id,
                    Name = firstCard.Collection.Name,
                }
            };

            return output;
        }
        ////////////////////////////////////////////////////////
        private CardDto CreateEmptyCardViewModel() => new();

        private CardDto MapCardToViewModel(Card card)
        {
            return new CardDto()
            {
                Id = card.Id,
                Ask = card.Ask,
                Answer = card.Answer,
                Description = card.Description,
                HasMp3 = card.HasMp3,
                Status = card.Status,
                Collection = new CollectionMiniDto()
                {
                    Id = card.Collection.Id,
                    Name = card.Collection.Name,
                },
            };
        }

        private Card MapViewModelToCard(CardDto model)
        {
            return new Card()
            {
                Id = model.Id,
                Ask = model.Ask,
                Answer = model.Answer,
                Description = model.Description,
                HasMp3 = model.HasMp3,
                CollectionId = model.Collection.Id,
                UserId = model.UserId,
                Status = model.Status,
            };
        }
        private async Task<string> GetCollectionName(long collectionId)
        {
            var collection = await _dbContext.Collections
            .AsNoTracking()
            .Select(x => new { x.Name, x.Id })
            .FirstAsync(x => x.Id == collectionId);

            return collection.Name;
        }

        private CardCheckDto ToCardCheckDto(Card card)
        {
            CardCheckDto output = new()
            {
                Id = card.Id,
                Ask = card.Ask,
                Answer = card.Answer,
                Description = card.Description,
                HasMp3 = card.HasMp3,
                Status = card.Status,
                Collection = new CollectionMiniDto {
                    Id = card.CollectionId,
                }
            };
            if (card.Collection is not null)
            {
                output.Collection.Name = card.Collection.Name;
            }
            return output;
        }
    }
}