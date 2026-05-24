namespace MarketUees.Domain.Common
{
    public class CassandraPagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public string? NextPageState { get; set; }
    }
}
