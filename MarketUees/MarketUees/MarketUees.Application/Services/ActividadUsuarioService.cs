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

        public Task RegistrarActividadAsync(Guid usuarioId, string tipoActividad, string contenidoId)
        {
            var actividad = new ActividadUsuario
            {
                UsuarioId = usuarioId,
                TipoActividad = tipoActividad,
                ContenidoId = contenidoId,
                FechaActividad = DateTimeOffset.UtcNow
            };
            return _repository.RegistrarActividadAsync(actividad);
        }

        public Task<IEnumerable<ActividadUsuario>> ObtenerActividadPorUsuarioAsync(Guid usuarioId)
            => _repository.ObtenerPorUsuarioAsync(usuarioId);
    }
}
