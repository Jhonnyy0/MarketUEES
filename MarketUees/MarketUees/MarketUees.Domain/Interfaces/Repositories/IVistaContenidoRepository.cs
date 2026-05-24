using MarketUees.Domain.Entities;

namespace MarketUees.Domain.Interfaces.Repositories
{
    public interface IVistaContenidoRepository
    {
        Task RegistrarVistaAsync(VistaContenido vista);
        Task<IEnumerable<VistaContenido>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}
