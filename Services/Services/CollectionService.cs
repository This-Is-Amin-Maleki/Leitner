using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using ServicesLeit.Interfaces;
using SharedLeit;
using Microsoft.Extensions.Logging;
using ModelsLeit.DTOs.Collection;
using ModelsLeit.DTOs.Box;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.DTOs.User;

namespace ServicesLeit.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ILogger<BoxService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly CardService _cardService;

        public CollectionService(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            //CardService cardService,
            ILogger<BoxService> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
            //_cardService = cardService;
        }
        public async Task<List<CollectionListDto>> ReadAllAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .AsNoTracking()
                .Select(x => new CollectionListDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                    CardsQ = x.Cards.Count,
                    User = new UserMiniDto()
                    {
                        Id = x.UserId,
                        UserName = _userManager.Users.First(y => y.Id == x.UserId).UserName!,
                    },
                })
                .ToListAsync();
        }
        public async Task<List<CollectionListDto>> ReadAllAsync(long userId)
        {//use auto mapper
            string userName = _userManager.Users.First(y => y.Id == userId).UserName!;
            return await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new CollectionListDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                    CardsQ = x.Cards.Count,
                    User = new UserMiniDto()
                    {
                        Id = userId,
                        UserName = userName,
                    },
                })
                .ToListAsync();
        }
        public async Task<List<CollectionDto>> ReadPublishedCollectionsAsync()
        {
            return await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.Status == CollectionStatus.Published)
                .Select(x => new CollectionDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CardsQ = x.Cards.Count
                })
                .ToListAsync();
        }
        public async Task<List<CollectionDto>> ReadUnusedPublishedCollectionsAsync(long userId)
        {
            var boxes = await _dbContext.Boxes
                .Where(x => x.UserId == userId)
                .Select(x => x.CollectionId)
                .ToArrayAsync();
            return await _dbContext.Collections
                .AsNoTracking()
                .Where(x =>
                    !boxes.Contains(x.Id) &&
                    x.Status == CollectionStatus.Published)
                .Select(x => new CollectionDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CardsQ = x.Cards.Count

                })
                .ToListAsync();
        }

        public async Task<CollectionUnlimitedDto> ReadCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
                .Select(x => new CollectionUnlimitedDto
                {
                    BoxCount = x.Boxes.Count,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                    User = new UserMiniDto()
                    {
                        Id = x.UserId,
                        UserName = _userManager.Users.First(y => y.Id == x.UserId).UserName!,
                    },
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return collection is null ?
                CreateEmptyCollectionUnlimitedDto() :
                collection;
        }
        public async Task<CollectionModifyDto> ReadCollectionDataAsync(long id)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
                .Select(x => new CollectionModifyDto
                {
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return collection is null ?
                CreateEmptyCollectionModifyDto() :
                collection;
        }
        public async Task<CollectionDto> ReadUserCollectionAsync(long id, long userId)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
                .Where(x => x.UserId == userId && x.Id == id)
                .Select(x => new CollectionDto
                {
                    BoxCount = x.Boxes.Count,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return collection is null ?
                CreateEmptyCollectionDto() :
                collection;
        }
        public async Task<CollectionMiniDto> ReadCollectionNameAndStatusAsync(long id)
        {
            var model = await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CollectionMiniDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync();

            return model is null ?
                new CollectionMiniDto() :
                model;
        }
        public async Task<BoxAddDto?> CheckExistCollectionAsync(long id)//pre add to box form
        {
            var model = await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.Id == id && x.Status == CollectionStatus.Published)
                .Select(x => new BoxAddDto()
                {
                    Name = x.Name,
                    CollectionId = x.Id,
                    Description = x.Description,
                    UserId = x.UserId,
                })
                .FirstOrDefaultAsync();

            return model;
        }
        public async Task AddCollectionAsync(CollectionCreationDto collectionViewModel)
        {
            var collection = MapViewModelToCollection(collectionViewModel);

            //add published date Time
            if (collection.Status is CollectionStatus.Published)
            {
                collection.PublishedDate = DateTime.UtcNow;
                collection.Count = _dbContext.Cards
                    .Where(x => x.CollectionId == collection.Id)
                    .Count();
            }

            await _dbContext.Collections.AddAsync(collection);
            await _dbContext.SaveChangesAsync();
        }
        public async Task EditCollectionLimitedAsync(CollectionModifyDto model)
        {
            var oldCollection = await _dbContext.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == model.UserId);

            if (oldCollection is null)
            {
                throw new Exception("Not Found");
            }
            var collection = MapViewModelToCollection(model);

            //add published date Time
            collection.PublishedDate =
                collection.Status is CollectionStatus.Published &&
                oldCollection.Status != CollectionStatus.Published
                ? DateTime.UtcNow : oldCollection.PublishedDate;
            collection.UserId = oldCollection.UserId;

#warning catch!!
            try
            {
                _dbContext.Collections.Update(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task EditCollectionAsync(CollectionModifyDto model)
        {
            bool allCardsApproved = true;
            bool anyCardsIn = true;
            if (model.Status == CollectionStatus.Published)
            {
                anyCardsIn = await IsAnyCardsAsync(model.Id);
                allCardsApproved = await IsAllCardsApprovedAsync(model.Id);
            }
            if (anyCardsIn is false)
            {
                throw new Exception("No cards available for publication");
            }
            if (allCardsApproved is true)
            {
                throw new Exception("All cards must be approved before publication");
            }
            var oldCollection = await _dbContext.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (oldCollection is null)
            {
                throw new Exception("Collection not found");
            }
            var collection = MapViewModelToCollection(model);

            //add published date Time
            collection.PublishedDate =
                collection.Status is CollectionStatus.Published &&
                oldCollection.Status != CollectionStatus.Published
                ? DateTime.UtcNow : oldCollection.PublishedDate;
            collection.UserId = oldCollection.UserId;

#warning catch!!
            try
            {
                _dbContext.Collections.Update(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteCollectionLimitedAsync(CollectionModifyDto model)
        {
            CollectionStatus[] forbidStatuses = { CollectionStatus.Published, CollectionStatus.Submit };

            CollectionStatus[] allowedStatuses =
                ((CollectionStatus[])Enum.GetValues(typeof(CollectionStatus)))
                .Except(forbidStatuses)
                .ToArray();

            var collection = MapViewModelToCollection(model);

            var hasCollection = await _dbContext.Collections
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == model.Id &&
                    x.UserId == model.UserId &&
                    allowedStatuses.Contains(x.Status));

            if (hasCollection is false)
            {
                throw new Exception("Not Found");
            }

#warning catch!!
            try
            {
                _dbContext.Collections.Remove(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteCollectionAsync(CollectionDto collectionViewModel)
        {
            CollectionStatus[] forbidStatuses = { CollectionStatus.Published, CollectionStatus.Submit };

            CollectionStatus[] allowedStatuses =
                ((CollectionStatus[])Enum.GetValues(typeof(CollectionStatus)))
                .Except(forbidStatuses)
                .ToArray();

            var collection = MapViewModelToCollection(collectionViewModel);

            var hasCollection = await _dbContext.Collections
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == collectionViewModel.Id &&
                    allowedStatuses.Contains(x.Status));

            if (hasCollection is false)
            {
                throw new Exception("Not Found");
            }

#warning catch!!
            try
            {
                _dbContext.Collections.Remove(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task CheckStatusAsync(long id, string message)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Select(x => new Collection { Id = x.Id, Status = x.Status })
                .FirstOrDefaultAsync(x => x.Id == id);
            if (collection.Status is CollectionStatus.Published or CollectionStatus.Submit)
            {
                var text = message.Replace("@", nameof(collection.Status));
                throw new Exception(text);
            }
        }
        ////////////////////////////////////////////////////////
        private CollectionDto CreateEmptyCollectionDto()
        {
            return new CollectionDto();
        }
        private CollectionModifyDto CreateEmptyCollectionModifyDto()
        {
            return new CollectionModifyDto();
        }
        private CollectionUnlimitedDto CreateEmptyCollectionUnlimitedDto()
        {
            return new CollectionUnlimitedDto();
        }
        private BoxAddDto CreateEmptyBoxAddViewModel()
        {
            return new BoxAddDto();
        }

        private CollectionDto MapCollectionViewModel(Collection collection)
        {
            return new CollectionDto()
            {
                Id = collection.Id,
                Description = collection.Description,
                Name = collection.Name,
                PublishedDate = collection.PublishedDate,
                Status = collection.Status,
            };
        }

        private BoxAddDto MapCollectionToBoxAdd(Collection collection)
        {
            return new BoxAddDto()
            {
                CollectionId = collection.Id,
                CardPerDay = 10,
                Description = collection.Description,
                Name = collection.Name,
            };
        }

        private Collection MapViewModelToCollection(CollectionDto collectionViewModel)
        {
            return new Collection()
            {
                Id = collectionViewModel.Id,
                Description = collectionViewModel.Description.Trim(),
                Name = collectionViewModel.Name.Trim(),
                Status = collectionViewModel.Status,
            };
        }
        private Collection MapViewModelToCollection(CollectionCreationDto collectionViewModel)
        {
            return new Collection()
            {
                UserId = collectionViewModel.UserId,
                Description = collectionViewModel.Description.Trim(),
                Status = CollectionStatus.Draft,
                Name = (collectionViewModel.Name ?? $"NewCollection{DateTime.Now.ToString("@ yy-MM-dd HH:mm")}").Trim(),
            };
        }
        private Collection MapViewModelToCollection(CollectionModifyDto collectionViewModel)
        {
            return new Collection()
            {
                Id = collectionViewModel.Id,
                Description = collectionViewModel.Description.Trim(),
                Name = collectionViewModel.Name.Trim(),
                Status = collectionViewModel.Status,
                UserId = collectionViewModel.UserId,
            };
        }

        private async Task CheckHasBoxAsync(long id, string message)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
                .Select(x => new Collection { Id = x.Id, Count = x.Boxes.Count })
                .FirstOrDefaultAsync(x => x.Id == id);
            if (collection.Count > 0)
            {
                throw new Exception(message);
            }
        }
        private async Task<bool> IsAllCardsApprovedAsync(long id) =>
            await _dbContext.Cards
                .Where(x => x.CollectionId == id)
                .AnyAsync(x => x.Status != CardStatus.Approved);
        private async Task<bool> IsAnyCardsAsync(long id) =>
            await _dbContext.Cards
                .AsNoTracking()
                .AnyAsync(x => x.CollectionId == id);
    }
}