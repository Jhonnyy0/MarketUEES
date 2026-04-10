using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Application.Services
{
    public class ResenaService
    {
        private readonly IResenaRepository _resenaRepository;

        public ResenaService(IResenaRepository resenaRepository)
        {
            _resenaRepository = resenaRepository;
        }

        public async Task<PagedResult<Resena>> GetResenasPorProducto(string productoId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            return await _resenaRepository.GetByProductoAsync(productoId, page, pageSize);
        }

        public async Task<PagedResult<Resena>> GetResenasPorUsuario(string usuarioId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            return await _resenaRepository.GetByUsuarioAsync(usuarioId, page, pageSize);
        }

        public async Task<Resena> CrearResena(Resena resena)
        {
            if (resena.Valoracion < 1 || resena.Valoracion > 5)
                throw new ArgumentException("La valoracion debe estar entre 1 y 5");

            resena.CreatedAt = DateTime.UtcNow;
            return await _resenaRepository.AddAsync(resena);
        }

        public async Task<bool> EliminarResena(string id)
        {
            return await _resenaRepository.DeleteAsync(id);
        }
    }
}
