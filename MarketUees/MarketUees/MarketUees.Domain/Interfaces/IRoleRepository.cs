namespace MarketUees.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> CreateRole(string roleName);
    }
}
