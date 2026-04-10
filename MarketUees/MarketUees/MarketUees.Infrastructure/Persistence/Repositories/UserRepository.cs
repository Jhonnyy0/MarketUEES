using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces;
using MarketUees.Infrastructure.Identity;
using MarketUees.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppIdentityUser> _userManager;

        public UserRepository(UserManager<AppIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Usuario> CreateUser(Usuario usuario)
        {
            var result = await _userManager.CreateAsync(usuario.ToIdentityUser(), usuario.Password);

            if (result.Succeeded)
            {
                var newUser = await _userManager.FindByEmailAsync(usuario.Email);
                usuario.Id = newUser!.Id;
                return usuario;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al crear usuario: {errors}");
        }

        public async Task<Usuario> AddToRoleSync(Usuario usuario, string roleName)
        {
            var userDb = await _userManager.FindByEmailAsync(usuario.Email);
            await _userManager.AddToRoleAsync(userDb!, roleName);
            return usuario;
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Usuario?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.ToDomainUser();
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new List<string>();
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
