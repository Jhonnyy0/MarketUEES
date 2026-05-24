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

        public async Task<IEnumerable<ActividadUsuario>> ObtenerActividadPorUsuarioAsync(
            Guid usuarioId,
            int page = 1,
            int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var actividad = await _repository.ObtenerPorUsuarioAsync(usuarioId);
            return actividad
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
