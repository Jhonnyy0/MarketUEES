namespace MarketUees.Domain.Entities
{
    public class Resena : BaseEntity
    {
        public string ProductoId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int Valoracion { get; set; }       // 1–5
        public string Comentario { get; set; } = string.Empty;
    }
}
