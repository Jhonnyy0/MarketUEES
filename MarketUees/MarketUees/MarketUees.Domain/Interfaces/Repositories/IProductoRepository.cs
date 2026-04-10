using MarketUees.Domain.Entities;

namespace MarketUees.Domain.Interfaces.Repositories
{
    public class ProductoFiltros
    {
        public string? Categoria { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public TipoProducto? Tipo { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }

    public interface IProductoRepository : IBaseRepository<Producto>
    {
        Task<PagedResult<Producto>> GetPagedAsync(int page, int pageSize, ProductoFiltros? filtros = null);
        Task<List<Producto>> GetByVendedorAsync(string vendedorId);
        Task<List<string>> GetCategoriasAsync();
    }
}
