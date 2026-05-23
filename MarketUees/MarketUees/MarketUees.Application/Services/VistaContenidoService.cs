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

        public Task RegistrarVistaAsync(Guid usuarioId, string contenidoId)
        {
            var vista = new VistaContenido
            {
                UsuarioId = usuarioId,
                ContenidoId = contenidoId,
                FechaVista = DateTimeOffset.UtcNow
            };
            return _repository.RegistrarVistaAsync(vista);
        }

        public Task<IEnumerable<VistaContenido>> ObtenerVistasPorUsuarioAsync(Guid usuarioId)
            => _repository.ObtenerPorUsuarioAsync(usuarioId);
    }
}
