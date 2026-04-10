using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(MongoDbContext context)
            : base(context.Productos) { }

        public async Task<PagedResult<Producto>> GetPagedAsync(int page, int pageSize, ProductoFiltros? filtros = null)
        {
            var filterBuilder = Builders<Producto>.Filter;
            var filter = filterBuilder.Eq(p => p.IsDeleted, false);

            if (filtros != null)
            {
                if (!string.IsNullOrEmpty(filtros.Categoria))
                    filter &= filterBuilder.Eq(p => p.Categoria, filtros.Categoria);

                if (filtros.PrecioMin.HasValue)
                    filter &= filterBuilder.Gte(p => p.Precio, filtros.PrecioMin.Value);

                if (filtros.PrecioMax.HasValue)
                    filter &= filterBuilder.Lte(p => p.Precio, filtros.PrecioMax.Value);

                if (filtros.Tipo.HasValue)
                    filter &= filterBuilder.Eq(p => p.Tipo, filtros.Tipo.Value);
            }

            var totalItems = (int)await _collection.CountDocumentsAsync(filter);

            var items = await _collection
                .Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<Producto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<Producto>> GetByVendedorAsync(string vendedorId)
        {
            var filter = Builders<Producto>.Filter.And(
                Builders<Producto>.Filter.Eq(p => p.VendedorId, vendedorId),
                Builders<Producto>.Filter.Eq(p => p.IsDeleted, false)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<string>> GetCategoriasAsync()
        {
            return await _collection
                .Distinct<string>("categoria", Builders<Producto>.Filter.Empty)
                .ToListAsync();
        }
    }
}
