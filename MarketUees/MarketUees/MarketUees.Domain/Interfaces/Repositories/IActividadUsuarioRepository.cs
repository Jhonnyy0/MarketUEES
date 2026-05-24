using MarketUees.Domain.Common;
using MarketUees.Domain.Entities;

namespace MarketUees.Domain.Interfaces.Repositories
{
    public interface IActividadUsuarioRepository
    {
        Task RegistrarActividadAsync(ActividadUsuario actividad);
        Task<IEnumerable<ActividadUsuario>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<CassandraPagedResult<ActividadUsuario>> ObtenerPorUsuarioPaginadoAsync(
            Guid usuarioId,
            int pageSize,
            string? pageState);
    }
}
