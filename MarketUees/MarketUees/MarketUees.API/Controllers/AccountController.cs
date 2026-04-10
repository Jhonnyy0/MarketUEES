using MarketUees.API.DTOs;
using MarketUees.Application.Services;
using MarketUees.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MarketUees.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var usuario = new Usuario
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Tel = dto.Phone,
                Password = dto.Password
            };

            await _authService.RegisterUser(usuario);
            return Ok(new { Message = "Registro exitoso" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto.Email, dto.Password, dto.RememberMe);
            return Ok(new { Token = token });
        }
    }
}
