using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces;

namespace MarketUees.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Usuario> RegisterUser(Usuario usuario)
        {
            await _userRepository.CreateUser(usuario);
            return usuario;
        }

        public async Task<string> Login(string email, string password, bool remember)
        {
            var usuario = await _userRepository.GetUserByEmail(email);

            if (usuario == null)
                return "Usuario no encontrado";

            var credencialesValidas = await _userRepository.CheckPasswordAsync(usuario.Id.ToString(), password);

            if (!credencialesValidas)
                return "Credenciales invalidas";

            var roles = await _userRepository.GetUserRoles(email);

            return _jwtService.GenerateToken(usuario, roles);
        }
    }
}
