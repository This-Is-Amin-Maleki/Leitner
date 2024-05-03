using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using ServicesLeit.Interfaces;
using SharedLeit;
using System.Collections;
using System.Collections.Generic;
using EFCore.BulkExtensions;
using System.Xml;
using Microsoft.Extensions.Logging;

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
        public async Task<List<CardViewModel>> ReadCardsLimitedAsync(long collectionId)
        {//use auto mapper
#warning if invalid card send 2 save reyurn without collection
            string collectionName = "";
            List<CardViewModel> list = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId)
                .Select(x => new CardViewModel()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    HasMp3 = x.HasMp3,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                        Status = x.Collection.Status,
                    }
                })
                .ToListAsync();
            return list;
        }

        public async Task<(List<CardViewModel>, string)> ReadCardsAsync(long collectionId)
        {//use auto mapper
#warning if invalid card send 2 save reyurn without collection
            string collectionName = "";
            List<CardViewModel> list = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId)
                .Select(x => new CardViewModel()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    Answer = x.Answer,
                    Description = x.Description,
                    HasMp3 = x.HasMp3,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                    }
                })
                .ToListAsync();
            if (list.Count > 0)
            {
                collectionName = list.FirstOrDefault().Collection.Name;
            }
            else
            {
                var collection = await _dbContext.Collections
                .AsNoTracking()
                .Select(x => new { x.Name, x.Id })
                .FirstOrDefaultAsync(x => x.Id == collectionId);

                collectionName = collection.Name;
            }
            return (list, collectionName);
        }
        public async Task<(List<CardViewModel>, string)> GetCardsAsync(long collectionId)
        {//use auto mapper
            string collectionName = "";
            List<CardViewModel> list = await _dbContext.Cards
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == collectionId)
                .Select(x => new CardViewModel()
                {
                    Id = x.Id,
                    Ask = x.Ask,
                    Answer = x.Answer,
                    Description = x.Description,
                    HasMp3 = x.HasMp3,
                })
                .ToListAsync();
            if (list.Count > 0)
            {
                collectionName = list.FirstOrDefault().Collection.Name;
            }
            else
            {
                var collection = await _dbContext.Collections
                .AsNoTracking()
                .Select(x => new { x.Name, x.Id })
                .FirstOrDefaultAsync(x => x.Id == collectionId);

                collectionName = collection.Name;
            }
            return (list, collectionName);
        }
#warning check performance
        public async Task<CardViewModel> ReadCardAsync(long id)
        {
            var card = await _dbContext.Cards
                .AsNoTracking()
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return card is null ?
                CreateEmptyCardViewModel() :
                MapCardToViewModel(card);
        }
#warning check performance
        public async Task<CardViewModel> GetCardAsync(long id)
        {
            var card = await _dbContext.Cards
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return card is null ?
                CreateEmptyCardViewModel() :
                MapCardToViewModel(card);
        }
        public async Task AddCardAsync(CardViewModel model)
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
        public async Task<int> AddCardsAsync(List<Card> model)
        {
            await _collection.CheckStatusAsync(model.First().CollectionId, "Can not add any cards to the @ collection!");
            int counter = 0;
            int done = 0;
            try
            {
                foreach (Card card in model)
                {
                    await _dbContext.Cards.AddAsync(card);
                    if ((++counter) % 10 is 0)
                    {
                        done += await _dbContext.SaveChangesAsync();
                    }
                }
                done += await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            return done;
        }
        public async Task UpdateCardAsync(CardViewModel model)
        {
            await _collection.CheckStatusAsync(model.Collection.Id, "Can not update any card of the @ collection!");
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
                _dbContext.Cards.Update(card);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteCardAsync(CardViewModel model)
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

        public async Task<List<CardViewModel>> ReadCardsAsync(long lastCardId, long collectionId, int count, int skip = 0)
        {
            //get new cardsArray
            return await _dbContext.Cards
                .AsNoTracking()
                .Where(x => x.Id > lastCardId && x.CollectionId == collectionId)
                .Select(x => new CardViewModel
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
        private CardViewModel CreateEmptyCardViewModel()
        {
            return new CardViewModel();
        }

        private CardViewModel MapCardToViewModel(Card card)
        {
            return new CardViewModel()
            {
                Id = card.Id,
                Ask = card.Ask,
                Answer = card.Answer,
                Description = card.Description,
                HasMp3 = card.HasMp3,
                Collection = new CollectionMiniViewModel()
                {
                    Id = card.Collection.Id,
                    Name = card.Collection.Name,
                },
            };
        }

        private Card MapViewModelToCard(CardViewModel cardViewModel)
        {
            return new Card()
            {
                Id = cardViewModel.Id,
                Ask = cardViewModel.Ask,
                Answer = cardViewModel.Answer,
                Description = cardViewModel.Description,
                HasMp3 = cardViewModel.HasMp3,
                CollectionId = cardViewModel.Collection.Id,
            };
        }
    }
}