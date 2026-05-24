using MarketUees.Domain.Interfaces.Repositories;
using MarketUees.Infrastructure.Persistence.Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketUees.Infrastructure
{
    public static class CassandraServiceExtensions
    {
        public static IServiceCollection AddCassandra(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var contactPoint = configuration["Cassandra:ContactPoint"] ?? "localhost";
            var port = int.TryParse(configuration["Cassandra:Port"], out var p) ? p : 9042;

            services.AddSingleton(_ => new CassandraContext(contactPoint, port));

            services.AddScoped<IVistaContenidoRepository, VistaContenidoRepository>();
            services.AddScoped<IActividadUsuarioRepository, ActividadUsuarioRepository>();

            return services;
        }
    }
}
