namespace MarketUees.Domain.Entities
{
    public enum TipoProducto
    {
        Fisico,
        Digital,
        Servicio
    }

    public class Producto : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public TipoProducto Tipo { get; set; } = TipoProducto.Fisico;
        public List<string> Etiquetas { get; set; } = new();
        public int Inventario { get; set; } = 0;
        public List<string> Imagenes { get; set; } = new();
        public string VendedorId { get; set; } = string.Empty;
        public DateTime Actualizado { get; set; } = DateTime.UtcNow;

        // Reseñas embebidas (resumen rápido por producto)
        public List<ReseñaEmbed> Reseñas { get; set; } = new();
    }

    public class ReseñaEmbed
    {
        public string ReseñaId { get; set; } = string.Empty;
        public int Valoracion { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
