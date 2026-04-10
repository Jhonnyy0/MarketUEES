using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public class ResenaRepository : BaseRepository<Resena>, IResenaRepository
    {
        public ResenaRepository(MongoDbContext context)
            : base(context.Resenas) { }

        public async Task<PagedResult<Resena>> GetByProductoAsync(string productoId, int page, int pageSize)
        {
            var filter = Builders<Resena>.Filter.And(
                Builders<Resena>.Filter.Eq(r => r.ProductoId, productoId),
                Builders<Resena>.Filter.Eq(r => r.IsDeleted, false)
            );

            var totalItems = (int)await _collection.CountDocumentsAsync(filter);

            var items = await _collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<Resena>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Resena>> GetByUsuarioAsync(string usuarioId, int page, int pageSize)
        {
            var filter = Builders<Resena>.Filter.And(
                Builders<Resena>.Filter.Eq(r => r.UsuarioId, usuarioId),
                Builders<Resena>.Filter.Eq(r => r.IsDeleted, false)
            );

            var totalItems = (int)await _collection.CountDocumentsAsync(filter);

            var items = await _collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<Resena>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
