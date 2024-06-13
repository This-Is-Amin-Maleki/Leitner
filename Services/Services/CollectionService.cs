using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using ServicesLeit.Interfaces;
using SharedLeit;
using ModelsLeit.ViewModels;
using Microsoft.Extensions.Logging;
using ModelsLeit.DTOs;
using Services.Services;

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

        public async Task<List<CollectionViewModel>> ReadCollectionsAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .FromSqlRaw("SELECT Id, Name, LEFT(Description, 200) AS Description, PublishedDate, Status FROM Collections")
                .AsNoTracking()
                .Select(x => new CollectionViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                })
                .ToListAsync();
        }
        public async Task<List<CollectionViewModel>> GetCollectionsAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .FromSqlRaw("SELECT Id, Name, LEFT(Description, 200) AS Description, PublishedDate, Status FROM Collections")
                .Select(x => new CollectionViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                })
                .ToListAsync();
        }
#warning check performance
        public async Task<CollectionViewModel> ReadCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .Include(x => x.Boxes)
                .Select(x => new CollectionViewModel
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
#warning check performance
        public async Task<CollectionViewModel> GetCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .FindAsync(id);

            return collection is null ?
                CreateEmptyCollectionViewModel() :
                MapCollectionViewModel(collection);
        }
        public async Task<CollectionMiniDto> GetCollectionNameAndStatusAsync(long id)
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
        public async Task<BoxAddViewModel?> CheckExistCollectionAsync(long id)//pre add to box form
        {
            var model = await _dbContext.Collections
                .AsNoTracking()
                .Where(x => x.Id == id && x.Status == CollectionStatus.Published)
                .Select(x => new BoxAddViewModel()
                {
                    Name = x.Name,
                    CollectionId = x.Id,
                    Description = x.Description,
                })
                .FirstOrDefaultAsync();

            return model;
        }
        public async Task AddCollectionAsync(CollectionViewModel collectionViewModel)
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
            
#warning catch!!
            try
            {
                await _dbContext.Collections.AddAsync(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task EditCollectionAsync(CollectionViewModel collectionViewModel)
        {
            var oldCollection = await _dbContext.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x=>x.Id==collectionViewModel.Id);

            if (oldCollection is null)
            {
                throw new Exception("Not Found");
            }
            var collection = MapViewModelToCollection(collectionViewModel);

            //add published date Time
            collection.PublishedDate =
                collection.Status is CollectionStatus.Published &&
                oldCollection.Status != CollectionStatus.Published
                ? DateTime.UtcNow : oldCollection.PublishedDate;

#warning catch!!
            try
            {
                _dbContext.Collections.Update(collection);
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }
        public async Task DeleteCollectionAsync(CollectionViewModel collectionViewModel)
        {
            var collection = MapViewModelToCollection(collectionViewModel);

            var hasCollection = await _dbContext.Collections
                .AsNoTracking()
                .AnyAsync(x => x.Id == collectionViewModel.Id);

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
        private CollectionViewModel CreateEmptyCollectionViewModel()
        {
            return new CollectionViewModel();
        }
        private BoxAddViewModel CreateEmptyBoxAddViewModel()
        {
            return new BoxAddViewModel();
        }

        private CollectionViewModel MapCollectionViewModel(Collection collection)
        {
            return new CollectionViewModel()
            {
                Id = collection.Id,
                Description = collection.Description,
                Name = collection.Name,
                PublishedDate = collection.PublishedDate,
                Status = collection.Status,
            };
        }

        private BoxAddViewModel MapCollectionToBoxAdd(Collection collection)
        {
            return new BoxAddViewModel()
            {
                CollectionId = collection.Id,
                CardPerDay = 10,
                Description = collection.Description,
                Name = collection.Name,
            };
        }

        private Collection MapViewModelToCollection(CollectionViewModel collectionViewModel)
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