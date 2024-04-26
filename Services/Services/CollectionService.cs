using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.DTOs;
using Services.Interfaces;
using Shared;
using Models.ViewModels;

namespace Services.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ApplicationDbContext _dbContext;

        public CollectionService(ApplicationDbContext dbContext)
        {
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
                .FirstOrDefaultAsync(x => x.Id == id);

            return collection is null ?
                CreateEmptyCollectionViewModel() :
                MapCollectionViewModel(collection);
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
        public async Task AddCollectionAsync(CollectionViewModel collectionViewModel)
        {
            var collection = MapViewModelToCollection(collectionViewModel);

            //add published date Time
            collection.PublishedDate = collection.Status is CollectionStatus.Published ? DateTime.UtcNow : new();
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
            var collection = MapViewModelToCollection(collectionViewModel);

            //add published date Time
            collection.PublishedDate = collection.Status is CollectionStatus.Published ? DateTime.UtcNow : new();

            var hasCollection = await _dbContext.Collections
                .AnyAsync(x => x.Id == collectionViewModel.Id);

            if (hasCollection is false)
            {
                throw new Exception("Not Found");
            }
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

        ////////////////////////////////////////////////////////
        private CollectionViewModel CreateEmptyCollectionViewModel()
        {
            return new CollectionViewModel();
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
    }
}