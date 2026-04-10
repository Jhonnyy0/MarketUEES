using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly IMongoCollection<TEntity> _collection;

        protected BaseRepository(IMongoCollection<TEntity> collection)
        {
            _collection = collection;
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (!ObjectId.TryParse(entity.Id, out var objectId))
                throw new ArgumentException("Id de entidad inválido");

            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            await _collection.ReplaceOneAsync(filter, entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}