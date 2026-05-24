using Cassandra;

namespace MarketUees.Infrastructure.Persistence.Cassandra
{
    public class CassandraContext : IDisposable
    {
        private readonly ICluster _cluster;
        public ISession Session { get; }
        private const string Keyspace = "marketuees";

        public CassandraContext(string contactPoint, int port = 9042)
        {
            _cluster = Cluster.Builder()
                .AddContactPoint(contactPoint)
                .WithPort(port)
                .Build();

            // Crear keyspace si no existe
            using var tempSession = _cluster.Connect();
            tempSession.Execute($@"
                CREATE KEYSPACE IF NOT EXISTS {Keyspace}
                WITH replication = {{'class': 'SimpleStrategy', 'replication_factor': 1}}");

            Session = _cluster.Connect(Keyspace);
            InicializarSchema();
        }

        private void InicializarSchema()
        {
            Session.Execute(@"
                CREATE TABLE IF NOT EXISTS vistas_contenido (
                    usuario_id uuid,
                    fecha_vista timestamp,
                    contenido_id text,
                    PRIMARY KEY (usuario_id, fecha_vista)
                ) WITH CLUSTERING ORDER BY (fecha_vista DESC)");

            Session.Execute(@"
                CREATE TABLE IF NOT EXISTS actividad_usuario (
                    usuario_id uuid,
                    fecha_actividad timestamp,
                    tipo_actividad text,
                    contenido_id text,
                    PRIMARY KEY (usuario_id, fecha_actividad)
                ) WITH CLUSTERING ORDER BY (fecha_actividad DESC)");
        }

        public void Dispose()
        {
            Session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
