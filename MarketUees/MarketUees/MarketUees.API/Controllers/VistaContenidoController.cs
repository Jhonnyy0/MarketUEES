using MarketUees.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketUees.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VistaContenidoController : ControllerBase
    {
        private readonly VistaContenidoService _service;

        public VistaContenidoController(VistaContenidoService service)
        {
            _service = service;
        }

        /// <summary>Registra que el usuario autenticado vio un contenido.</summary>
        [HttpPost("{contenidoId}")]
        public async Task<IActionResult> Registrar(string contenidoId)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
                return Unauthorized();

            await _service.RegistrarVistaAsync(usuarioId, contenidoId);
            return NoContent();
        }

        /// <summary>Devuelve el historial de vistas del usuario autenticado.</summary>
        [HttpGet("mis-vistas")]
        public async Task<IActionResult> MisVistas()
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
                return Unauthorized();

            var vistas = await _service.ObtenerVistasPorUsuarioAsync(usuarioId);
            return Ok(vistas);
        }
    }
}
