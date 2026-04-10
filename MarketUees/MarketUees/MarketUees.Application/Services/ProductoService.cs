using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Application.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<PagedResult<Producto>> GetProductos(int page, int pageSize, ProductoFiltros? filtros = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            return await _productoRepository.GetPagedAsync(page, pageSize, filtros);
        }

        public async Task<Producto?> GetProductoById(string id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task<Producto> CrearProducto(Producto producto, string vendedorId)
        {
            producto.VendedorId = vendedorId;
            producto.CreatedAt = DateTime.UtcNow;
            producto.Actualizado = DateTime.UtcNow;
            return await _productoRepository.AddAsync(producto);
        }

        public async Task<Producto> ActualizarProducto(Producto producto)
        {
            producto.Actualizado = DateTime.UtcNow;
            return await _productoRepository.UpdateAsync(producto);
        }

        public async Task<bool> EliminarProducto(string id)
        {
            return await _productoRepository.DeleteAsync(id);
        }

        public async Task<List<Producto>> GetProductosPorVendedor(string vendedorId)
        {
            return await _productoRepository.GetByVendedorAsync(vendedorId);
        }

        public async Task<List<string>> GetCategorias()
        {
            return await _productoRepository.GetCategoriasAsync();
        }
    }
}
