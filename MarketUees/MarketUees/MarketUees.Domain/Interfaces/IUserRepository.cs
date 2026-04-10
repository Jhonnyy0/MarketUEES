using MarketUees.Domain.Entities;

namespace MarketUees.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Usuario?> GetUserByEmail(string email);
        Task<Usuario> CreateUser(Usuario usuario);
        Task<Usuario> AddToRoleSync(Usuario usuario, string roleName);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<bool> UserExists(string email);
        Task<List<string>> GetUserRoles(string email);
    }
}
