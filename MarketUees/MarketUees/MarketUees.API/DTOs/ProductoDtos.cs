using MarketUees.Domain.Entities;

namespace MarketUees.API.DTOs
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public TipoProducto Tipo { get; set; } = TipoProducto.Fisico;
        public List<string> Etiquetas { get; set; } = new();
        public int Inventario { get; set; }
        public List<string> Imagenes { get; set; } = new();
    }

    public class ProductoFiltrosDto
    {
        public string? Categoria { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public TipoProducto? Tipo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
