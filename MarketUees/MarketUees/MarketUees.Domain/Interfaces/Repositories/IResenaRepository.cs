using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Domain.Interfaces.Repositories
{
    public interface IResenaRepository : IBaseRepository<Resena>
    {
        Task<PagedResult<Resena>> GetByProductoAsync(string productoId, int page, int pageSize);
        Task<PagedResult<Resena>> GetByUsuarioAsync(string usuarioId, int page, int pageSize);
    }
}
