namespace MarketUees.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
