namespace MarketUees.Domain.Entities
{
    public class VistaContenido
    {
        public Guid UsuarioId { get; set; }
        public string ContenidoId { get; set; } = string.Empty;
        public DateTimeOffset FechaVista { get; set; }
    }
}
