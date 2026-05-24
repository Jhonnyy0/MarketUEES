using Cassandra;
using MarketUees.Domain.Common;
using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Infrastructure.Persistence.Cassandra
{
    public class VistaContenidoRepository : IVistaContenidoRepository
    {
        private readonly ISession _session;

        public VistaContenidoRepository(CassandraContext context)
        {
            _session = context.Session;
        }

        public async Task RegistrarVistaAsync(VistaContenido vista)
        {
            var statement = await _session.PrepareAsync(
                "INSERT INTO vistas_contenido (usuario_id, fecha_vista, contenido_id) VALUES (?, ?, ?)");

            var bound = statement.Bind(vista.UsuarioId, vista.FechaVista.UtcDateTime, vista.ContenidoId ?? string.Empty);
            await _session.ExecuteAsync(bound);
        }

        public async Task<IEnumerable<VistaContenido>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var statement = await _session.PrepareAsync(
                "SELECT usuario_id, fecha_vista, contenido_id FROM vistas_contenido WHERE usuario_id = ?");

            var bound = statement.Bind(usuarioId);
            var rows = await _session.ExecuteAsync(bound);

            return rows.Select(row => new VistaContenido
            {
                UsuarioId = row.GetValue<Guid>("usuario_id"),
                ContenidoId = row.GetValue<string>("contenido_id") ?? string.Empty,
                FechaVista = row.GetValue<DateTimeOffset>("fecha_vista")
            });
        }

        public async Task<CassandraPagedResult<VistaContenido>> ObtenerPorUsuarioPaginadoAsync(
            Guid usuarioId,
            int pageSize,
            string? pageState)
        {
            var statement = await _session.PrepareAsync(
                "SELECT usuario_id, fecha_vista, contenido_id FROM vistas_contenido WHERE usuario_id = ?");

            var bound = statement.Bind(usuarioId)
                .SetPageSize(pageSize);

            if (!string.IsNullOrWhiteSpace(pageState))
                bound.SetPagingState(Convert.FromBase64String(pageState));

            var rows = await _session.ExecuteAsync(bound);

            return new CassandraPagedResult<VistaContenido>
            {
                Items = rows.Select(row => new VistaContenido
                {
                    UsuarioId = row.GetValue<Guid>("usuario_id"),
                    ContenidoId = row.GetValue<string>("contenido_id") ?? string.Empty,
                    FechaVista = row.GetValue<DateTimeOffset>("fecha_vista")
                }).ToList(),
                NextPageState = rows.PagingState is null ? null : Convert.ToBase64String(rows.PagingState)
            };
        }
    }
}
