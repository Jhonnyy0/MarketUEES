using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace MarketUees.Infrastructure.Identity
{
    [CollectionName("usuarios")]
    public class AppIdentityUser : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
    }

    [CollectionName("roles")]
    public class AppIdentityRole : MongoIdentityRole<Guid>
    {
        public AppIdentityRole() : base() { }
        public AppIdentityRole(string roleName) : base(roleName) { }
    }
}
