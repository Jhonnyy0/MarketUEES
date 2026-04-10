using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public class CompraRepository : BaseRepository<Compra>, ICompraRepository
    {
        public CompraRepository(MongoDbContext context)
            : base(context.Compras) { }

        public async Task<PagedResult<Compra>> GetPagedAsync(int page, int pageSize)
        {
            var filter = Builders<Compra>.Filter.Eq(c => c.IsDeleted, false);

            var totalItems = (int)await _collection.CountDocumentsAsync(filter);

            var items = await _collection
                .Find(filter)
                .SortByDescending(c => c.Fecha)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<Compra>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Compra>> GetByUsuarioAsync(string usuarioId, int page, int pageSize)
        {
            var filter = Builders<Compra>.Filter.And(
                Builders<Compra>.Filter.Eq(c => c.UsuarioId, usuarioId),
                Builders<Compra>.Filter.Eq(c => c.IsDeleted, false)
            );

            var totalItems = (int)await _collection.CountDocumentsAsync(filter);

            var items = await _collection
                .Find(filter)
                .SortByDescending(c => c.Fecha)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<Compra>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
