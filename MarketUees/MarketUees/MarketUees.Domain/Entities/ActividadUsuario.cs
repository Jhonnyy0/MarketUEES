namespace MarketUees.Domain.Entities
{
    public class ActividadUsuario
    {
        public Guid UsuarioId { get; set; }
        public string TipoActividad { get; set; } = string.Empty; // "vista", "like", "compartido"
        public DateTimeOffset FechaActividad { get; set; }
        public string ContenidoId { get; set; } = string.Empty;
    }
}
