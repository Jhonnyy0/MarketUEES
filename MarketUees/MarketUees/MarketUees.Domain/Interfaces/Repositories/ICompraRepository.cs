using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Domain.Interfaces.Repositories
{
    public interface ICompraRepository : IBaseRepository<Compra>
    {
        Task<PagedResult<Compra>> GetPagedAsync(int page, int pageSize);
        Task<PagedResult<Compra>> GetByUsuarioAsync(string usuarioId, int page, int pageSize);
    }
}
