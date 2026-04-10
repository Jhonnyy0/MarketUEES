using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Application.Services
{
    public class CompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly IProductoRepository _productoRepository;

        public CompraService(ICompraRepository compraRepository, IProductoRepository productoRepository)
        {
            _compraRepository = compraRepository;
            _productoRepository = productoRepository;
        }

        public async Task<PagedResult<Compra>> GetCompras(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            return await _compraRepository.GetPagedAsync(page, pageSize);
        }

        public async Task<PagedResult<Compra>> GetComprasPorUsuario(string usuarioId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            return await _compraRepository.GetByUsuarioAsync(usuarioId, page, pageSize);
        }

        public async Task<Compra?> GetCompraById(string id)
        {
            return await _compraRepository.GetByIdAsync(id);
        }

        public async Task<Compra> CrearCompra(Compra compra)
        {
            compra.Fecha = DateTime.UtcNow;
            compra.CreatedAt = DateTime.UtcNow;
            compra.EstadoPago = EstadoPago.Pendiente;
            compra.EstadoEnvio = EstadoEnvio.Pendiente;

            // Calcular total desde los items
            compra.Total = compra.Items.Sum(i => i.Subtotal);

            return await _compraRepository.AddAsync(compra);
        }
    }
}
