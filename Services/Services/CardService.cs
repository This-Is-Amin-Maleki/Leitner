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

    }
}