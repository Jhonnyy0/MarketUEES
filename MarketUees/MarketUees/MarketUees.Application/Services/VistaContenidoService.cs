using MarketUees.Domain.Common;
using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Application.Services
{
    public class VistaContenidoService
    {
        private readonly IVistaContenidoRepository _repository;

        public VistaContenidoService(IVistaContenidoRepository repository)
        {
            _repository = repository;
        }

        public async Task<VistaContenido> RegistrarVistaAsync(Guid usuarioId, string contenidoId)
        {
            var vista = new VistaContenido
            {
                UsuarioId = usuarioId,
                ContenidoId = contenidoId,
                FechaVista = DateTimeOffset.UtcNow
            };
            await _repository.RegistrarVistaAsync(vista);
            return vista;
        }

        public Task<CassandraPagedResult<VistaContenido>> ObtenerVistasPorUsuarioAsync(
            Guid usuarioId,
            int pageSize = 10,
            string? pageState = null)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            return _repository.ObtenerPorUsuarioPaginadoAsync(usuarioId, pageSize, pageState);
        }
    }
}
