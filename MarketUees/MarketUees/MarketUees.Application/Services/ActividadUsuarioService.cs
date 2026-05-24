using MarketUees.Domain.Common;
using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Application.Services
{
    public class ActividadUsuarioService
    {
        private readonly IActividadUsuarioRepository _repository;

        public ActividadUsuarioService(IActividadUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActividadUsuario> RegistrarActividadAsync(Guid usuarioId, string tipoActividad, string contenidoId)
        {
            var actividad = new ActividadUsuario
            {
                UsuarioId = usuarioId,
                TipoActividad = tipoActividad,
                ContenidoId = contenidoId,
                FechaActividad = DateTimeOffset.UtcNow
            };
            await _repository.RegistrarActividadAsync(actividad);
            return actividad;
        }

        public Task<CassandraPagedResult<ActividadUsuario>> ObtenerActividadPorUsuarioAsync(
            Guid usuarioId,
            int pageSize = 10,
            string? pageState = null)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            return _repository.ObtenerPorUsuarioPaginadoAsync(usuarioId, pageSize, pageState);
        }
    }
}
