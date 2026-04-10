using MarketUees.Domain.Entities;
using MarketUees.Infrastructure.Identity;

namespace MarketUees.Infrastructure.Mapping
{
    public static class UserMappingExtension
    {
        public static AppIdentityUser ToIdentityUser(this Usuario usuario)
        {
            return new AppIdentityUser
            {
                Id = usuario.Id == Guid.Empty ? Guid.NewGuid() : usuario.Id,
                UserName = usuario.Email,
                Email = usuario.Email,
                PhoneNumber = usuario.Tel,
                FirstName = usuario.FirstName,
                LastName = usuario.LastName
            };
        }

        public static Usuario ToDomainUser(this AppIdentityUser identityUser)
        {
            return new Usuario
            {
                Id = identityUser.Id,
                Email = identityUser.Email ?? string.Empty,
                Tel = identityUser.PhoneNumber ?? string.Empty,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName
            };
        }
    }
}
