using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using ServicesLeit.Interfaces;
using SharedLeit;
using Microsoft.Extensions.Logging;
using ServicesLeit.Services;
using ModelsLeit.DTOs.Collection;
using ModelsLeit.DTOs.Box;

namespace ServicesLeit.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ILogger<BoxService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public CollectionService(ApplicationDbContext dbContext, ILogger<BoxService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<List<CollectionDto>> ReadCollectionsAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .AsNoTracking()
                .Select(x => new CollectionDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                    CardsQ = x.Cards.Count
                })
                .ToListAsync();
        }
        public async Task<List<CollectionDto>> ReadUserCollectionsAsync(long userId)
        {//use auto mapper
            return await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new CollectionDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                    CardsQ = x.Cards.Count
                })
                .ToListAsync();
        }
        public async Task<List<CollectionDto>> ReadPublishedCollectionsAsync()
        {
                return await _dbContext.Collections
                    .AsNoTracking()
                    .Where(x=> x.Status == CollectionStatus.Published)
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

        public async Task<CollectionDto> ReadCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
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
                CreateEmptyCollectionViewModel() :
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
                CreateEmptyCollectionViewModel() :
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
            if(collection.Status is CollectionStatus.Published)
            {
                collection.PublishedDate = DateTime.UtcNow;
                collection.Count = _dbContext.Cards
                    .Where(x=>x.CollectionId == collection.Id)
                    .Count();
            }

                await _dbContext.Collections.AddAsync(collection);
                await _dbContext.SaveChangesAsync();
        }
        public async Task EditCollectionLimitedAsync(CollectionEditDto model)
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
        public async Task EditCollectionAsync(CollectionDto model)
        {
            var oldCollection = await _dbContext.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.Id);

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
        public async Task DeleteCollectionLimitedAsync(CollectionEditDto model)
        {
            CollectionStatus[] forbidStatuses = { CollectionStatus.Published, CollectionStatus.Submit };

            CollectionStatus[] allowedStatuses =
                ((CollectionStatus[]) Enum.GetValues(typeof(CollectionStatus)))
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
        private CollectionDto CreateEmptyCollectionViewModel()
        {
            return new CollectionDto();
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
        private Collection MapViewModelToCollection(CollectionAddDto collectionViewModel)
        {
            return new Collection()
            {
                Id = collectionViewModel.Id ?? 0,
                Description = collectionViewModel.Description.Trim(),
                Name = collectionViewModel.Name.Trim(),
                Status = collectionViewModel.Status,
            };
        }

        private async Task CheckHasBoxAsync(long id, string message)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x=>x.Boxes)
                .Select(x => new Collection { Id = x.Id, Count = x.Boxes.Count})
                .FirstOrDefaultAsync(x => x.Id == id);
            if (collection.Count > 0)
            {
                throw new Exception(message);
            }
        }
    }
}