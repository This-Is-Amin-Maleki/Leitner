using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities;
using Models.ViewModels;
using System.Data;

namespace Services.Services
{
    public class BoxService
    //: IBoxService
    {
        private readonly ILogger<BoxService> _logger;

        private readonly ApplicationDbContext _dbContext;
        private readonly CardService _cardService;
        public BoxService(ApplicationDbContext dbContext, CardService cardService, ILogger<BoxService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _cardService = cardService;
        }
        public async Task<List<BoxViewModel>> ReadAllAsync()
        {//use auto mapper
            return await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .ThenInclude(x => x.Cards)
                .ThenInclude(x => x.ContainerCards)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    CardPerDay = x.CardPerDay,
                    LastSlot = x.LastSlot,
                    LastCardId = x.LastCardId,
                    Completed = x.Completed,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,

                    },
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> GetAllAsync()
        {//use auto mapper
            return await _dbContext.Boxes
                .Include(x => x.Collection)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name
                    }
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> ReadByCollectionAsync(long id)
        {//use auto mapper
            return await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == id)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    LastCardId = x.LastCardId,
                    CardPerDay = x.CardPerDay,
                    Completed = x.Completed,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                    },
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> GetByCollectionAsync(long id)
        {//use auto mapper
            return await _dbContext.Boxes
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == id)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name
                    }
                })
                .ToListAsync();
        }

#warning check performance
        public async Task<BoxViewModel> ReadAsync(long id)
        {
            var box = await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return box is null ?
                CreateEmptyBoxViewModel() :
                MapBoxToViewModel(box);
        }

        private BoxViewModel CreateEmptyBoxViewModel()
        {
            return new BoxViewModel();
        }

        private BoxViewModel MapBoxToViewModel(Box box) => new BoxViewModel();
    }
}