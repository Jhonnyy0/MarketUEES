using MarketUees.API.DTOs;
using MarketUees.Application.Services;
using MarketUees.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketUees.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompraController : ControllerBase
    {
        private readonly CompraService _compraService;

        public CompraController(CompraService compraService)
        {
            _compraService = compraService;
        }

        // GET api/compra?page=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTodas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _compraService.GetCompras(page, pageSize);
            return Ok(resultado);
        }

        // GET api/compra/mis-compras?page=1&pageSize=10
        [HttpGet("mis-compras")]
        public async Task<IActionResult> GetMisCompras([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var resultado = await _compraService.GetComprasPorUsuario(usuarioId, page, pageSize);
            return Ok(resultado);
        }

        // GET api/compra/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var compra = await _compraService.GetCompraById(id);
            if (compra == null) return NotFound();
            return Ok(compra);
        }

        // POST api/compra
        [HttpPost]
        public async Task<IActionResult> Crear(CrearCompraDto dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var compra = new Compra
            {
                UsuarioId = usuarioId,
                Items = dto.Items.Select(i => new ItemCompra
                {
                    ProductoId = i.ProductoId,
                    NombreProducto = i.NombreProducto,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Subtotal = i.Cantidad * i.PrecioUnitario
                }).ToList()
            };

            var creada = await _compraService.CrearCompra(compra);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ResenaController : ControllerBase
    {
        private readonly ResenaService _resenaService;

        public ResenaController(ResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        // GET api/resena/producto/{productoId}?page=1&pageSize=10
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> GetByProducto(
            string productoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _resenaService.GetResenasPorProducto(productoId, page, pageSize);
            return Ok(resultado);
        }

        // GET api/resena/usuario/{usuarioId}?page=1&pageSize=10
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(
            string usuarioId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _resenaService.GetResenasPorUsuario(usuarioId, page, pageSize);
            return Ok(resultado);
        }

        // POST api/resena
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear(CrearResenaDto dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var resena = new Resena
            {
                ProductoId = dto.ProductoId,
                UsuarioId = usuarioId,
                Valoracion = dto.Valoracion,
                Comentario = dto.Comentario
            };

            var creada = await _resenaService.CrearResena(resena);
            return Ok(creada);
        }

        // DELETE api/resena/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(string id)
        {
            var eliminado = await _resenaService.EliminarResena(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
