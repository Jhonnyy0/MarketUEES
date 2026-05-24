using MarketUees.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MarketUees.API.Controllers
{
    public record RegistrarActividadRequest(
        [Required] string TipoActividad,
        [Required] string ContenidoId);

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActividadUsuarioController : ControllerBase
    {
        private readonly ActividadUsuarioService _service;

        public ActividadUsuarioController(ActividadUsuarioService service)
        {
            _service = service;
        }

        /// <summary>Registra una actividad del usuario autenticado (vista, like, compartido…).</summary>
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarActividadRequest request)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
                return Unauthorized();

            var actividad = await _service.RegistrarActividadAsync(usuarioId, request.TipoActividad, request.ContenidoId);

            return Ok(new
            {
                mensaje = "Actividad registrada correctamente.",
                data = actividad
            });
        }

        /// <summary>Devuelve el historial de actividad del usuario autenticado.</summary>
        [HttpGet("mi-actividad")]
        public async Task<IActionResult> MiActividad([FromQuery] int pageSize = 10, [FromQuery] string? pageState = null)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
                return Unauthorized();

            var actividad = await _service.ObtenerActividadPorUsuarioAsync(usuarioId, pageSize, pageState);
            return Ok(actividad);
        }
    }
}
