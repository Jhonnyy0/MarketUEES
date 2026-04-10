namespace MarketUees.Domain.Entities
{
    public enum EstadoPago
    {
        Pendiente,
        Pagado,
        Fallido
    }

    public enum EstadoEnvio
    {
        Pendiente,
        Enviado,
        Entregado,
        Cancelado
    }

    public class Compra : BaseEntity
    {
        public string UsuarioId { get; set; } = string.Empty;
        public List<ItemCompra> Items { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public EstadoPago EstadoPago { get; set; } = EstadoPago.Pendiente;
        public EstadoEnvio EstadoEnvio { get; set; } = EstadoEnvio.Pendiente;
    }

    public class ItemCompra
    {
        public string ProductoId { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
