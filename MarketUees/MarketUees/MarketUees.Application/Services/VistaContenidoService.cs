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

        public async Task<IEnumerable<VistaContenido>> ObtenerVistasPorUsuarioAsync(
            Guid usuarioId,
            int page = 1,
            int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var vistas = await _repository.ObtenerPorUsuarioAsync(usuarioId);
            return vistas
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
