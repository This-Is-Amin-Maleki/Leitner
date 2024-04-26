using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.ViewModels;
using Services.Interfaces;
using Shared;
using Models.DTOs;
using System.Collections;
using System.Collections.Generic;

namespace Services.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _dbContext;

        public CardService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(List<CardViewModel>, string)> ReadCardsAsync(long collectionId)
        {//use auto mapper
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
                    Collection = new CardCollectionViewModel()
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
        public async Task AddCardAsync(CardViewModel cardViewModel)
        {
            var card = MapViewModelToCard(cardViewModel);
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
        public async Task EditCardAsync(CardViewModel cardViewModel)
        {
            var card = MapViewModelToCard(cardViewModel);

            var hasCard = await _dbContext.Cards
                .AnyAsync(x => x.Id == cardViewModel.Id);

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
        public async Task DeleteCardAsync(CardViewModel cardViewModel)
        {
            var card = MapViewModelToCard(cardViewModel);

            var hasCard = await _dbContext.Cards
                .AnyAsync(x => x.Id == cardViewModel.Id);

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
                Collection = new CardCollectionViewModel()
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