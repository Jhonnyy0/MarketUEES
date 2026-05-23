using Cassandra;
using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;

namespace MarketUees.Infrastructure.Persistence.Cassandra
{
    public class ActividadUsuarioRepository : IActividadUsuarioRepository
    {
        private readonly ISession _session;

        public ActividadUsuarioRepository(CassandraContext context)
        {
            _session = context.Session;
        }

        public async Task RegistrarActividadAsync(ActividadUsuario actividad)
        {
            var statement = await _session.PrepareAsync(
                "INSERT INTO actividad_usuario (usuario_id, fecha_actividad, tipo_actividad, contenido_id) VALUES (?, ?, ?, ?)");

            var bound = statement.Bind(
                actividad.UsuarioId,
                actividad.FechaActividad.UtcDateTime,
                actividad.TipoActividad,
                actividad.ContenidoId ?? string.Empty);

            await _session.ExecuteAsync(bound);
        }

        public async Task<IEnumerable<ActividadUsuario>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var statement = await _session.PrepareAsync(
                "SELECT usuario_id, fecha_actividad, tipo_actividad, contenido_id FROM actividad_usuario WHERE usuario_id = ?");

            var bound = statement.Bind(usuarioId);
            var rows = await _session.ExecuteAsync(bound);

            return rows.Select(row => new ActividadUsuario
            {
                UsuarioId = row.GetValue<Guid>("usuario_id"),
                FechaActividad = row.GetValue<DateTimeOffset>("fecha_actividad"),
                TipoActividad = row.GetValue<string>("tipo_actividad"),
                ContenidoId = row.GetValue<string>("contenido_id") ?? string.Empty,
            });
        }
    }
}
