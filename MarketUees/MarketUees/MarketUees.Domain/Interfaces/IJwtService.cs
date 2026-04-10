using MarketUees.Domain.Entities;

namespace MarketUees.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario, IList<string> roles);
    }
}
