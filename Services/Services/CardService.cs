using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using EFCore.BulkExtensions;
using Microsoft.Extensions.Logging;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Collection;

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
            return await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                .Select(x => new CardMiniDto()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    HasMp3 = x.HasMp3,
                    Collection = new CollectionMiniDto()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                        Status = x.Collection.Status,
                    }
                })
                .ToListAsync();
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
        public async Task AddCardAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not add any cards to the @ collection!");
            var card = MapViewModelToCard(model);
            card.Id = 0;

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
            await _collection.CheckStatusAsync(model.First().CollectionId, "Can not add any cards to the @ collection!");
            await _dbContext.BulkInsertAsync(model);
        }
        public async Task UpdateCardLimitedAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not update any card of the @ collection!");
            var card = MapViewModelToCard(model);

            var oldCard = await _dbContext.Cards
                .FirstAsync(x => x.Id == model.Id && x.UserId == model.UserId);

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
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not update any card of the @ collection!");
            var card = MapViewModelToCard(model);

            var oldCard = await _dbContext.Cards
                .FirstAsync(x => x.Id == model.Id);

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
        public async Task DeleteCardLimitedAsync(CardDto model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not delete any card of the @ collection!");
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
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not delete any card of the @ collection!");
            var card = MapViewModelToCard(model);

            var hasCard = await _dbContext.Cards
                .AnyAsync(x => x.Id == model.Id);

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

        ////////////////////////////////////////////////////////
        private CardDto CreateEmptyCardViewModel()
        {
            return new CardDto();
        }

        private CardDto MapCardToViewModel(Card card)
        {
            return new CardDto()
            {
                Id = card.Id,
                Ask = card.Ask,
                Answer = card.Answer,
                Description = card.Description,
                HasMp3 = card.HasMp3,
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
    }
}