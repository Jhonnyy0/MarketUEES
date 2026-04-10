namespace MarketUees.API.DTOs
{
    public class CrearCompraDto
    {
        public List<ItemCompraDto> Items { get; set; } = new();
    }

    public class ItemCompraDto
    {
        public string ProductoId { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class CrearResenaDto
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Valoracion { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
