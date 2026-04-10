using MarketUees.Domain.Interfaces;
using MarketUees.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MarketUees.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<AppIdentityRole> _roleManager;

        public RoleRepository(RoleManager<AppIdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<bool> CreateRole(string roleName)
        {
            if (!await RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new AppIdentityRole(roleName));
                return result.Succeeded;
            }
            return false;
        }
    }
}
