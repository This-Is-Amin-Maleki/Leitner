using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.DTOs;
using Services.Interfaces;
using Shared;

namespace Services.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ApplicationDbContext _dbContext;

        public CollectionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CollectionDTO>> ReadCollectionsAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .FromSqlRaw("SELECT Id, Name, LEFT(Description, 200) AS Description, PublishedDate, Status FROM Collections")
                .AsNoTracking()
                .Select(x => new CollectionDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    PublishedDate = x.PublishedDate,
                    Status = x.Status,
                })
                .ToListAsync();
        }
        public async Task<List<CollectionDTO>> GetCollectionsAsync()
        {//use auto mapper
            return await _dbContext.Collections
                .FromSqlRaw("SELECT Id, Name, LEFT(Description, 200) AS Description, PublishedDate, Status FROM Collections")
                .Select(x => new CollectionDTO()
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
        public async Task<CollectionDTO> ReadCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return collection is null ?
                CreateEmptyCollectionDTO() :
                MapCollectionToDTO(collection);
        }
#warning check performance
        public async Task<CollectionDTO> GetCollectionAsync(long id)
        {
            var collection = await _dbContext.Collections
                .FindAsync(id);

            return collection is null ?
                CreateEmptyCollectionDTO() :
                MapCollectionToDTO(collection);
        }
        public async Task AddCollectionAsync(CollectionDTO collectionDTO)
        {
            var collection = MapDTOToCollection(collectionDTO);
            await _dbContext.Collections.AddAsync(collection);

            //add published date Time
            collection.PublishedDate = collection.Status is CollectionStatus.Published ? DateTime.UtcNow : new();
            await _dbContext.SaveChangesAsync();
        }
        public async Task EditCollectionAsync(CollectionDTO collectionDTO)
        {
            var collection = MapDTOToCollection(collectionDTO);

            //add published date Time
            collection.PublishedDate = collection.Status is CollectionStatus.Published ? DateTime.UtcNow : new();

            var hasCollection = await _dbContext.Collections
                .AnyAsync(x => x.Id == collectionDTO.Id);

            if (hasCollection is false)
            {
                throw new Exception("Not Found");
            }
            await _dbContext.Collections.AddAsync(collection);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteCollectionAsync(CollectionDTO collectionDTO)
        {
            var collection = MapDTOToCollection(collectionDTO);

            var hasCollection = await _dbContext.Collections
                .AnyAsync(x => x.Id == collectionDTO.Id);

            if (hasCollection is false)
            {
                throw new Exception("Not Found");
            }
            _dbContext.Collections.Remove(collection);
            await _dbContext.SaveChangesAsync();
        }


        ////////////////////////////////////////////////////////
        private CollectionDTO CreateEmptyCollectionDTO()
        {
            return new CollectionDTO();
        }

        private CollectionDTO MapCollectionToDTO(Collection collection)
        {
            return new CollectionDTO()
            {
                Id = collection.Id,
                Description = collection.Description,
                Name = collection.Name,
                PublishedDate = collection.PublishedDate,
                Status = collection.Status,
            };
        }
        private Collection MapDTOToCollection(CollectionDTO collectionDTO)
        {
            return new Collection()
            {
                Id = collectionDTO.Id ?? 0,
                Description = collectionDTO.Description,
                Name = collectionDTO.Name,
                PublishedDate = DateTime.Now,
                Status = collectionDTO.Status,
            };
        }
    }
}