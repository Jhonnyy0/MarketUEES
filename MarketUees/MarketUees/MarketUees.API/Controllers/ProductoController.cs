using MarketUees.API.DTOs;
using MarketUees.Application.Services;
using MarketUees.Domain.Entities;
using MarketUees.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketUees.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET api/producto?page=1&pageSize=10&categoria=libros&precioMin=5&precioMax=100
        [HttpGet]
        public async Task<IActionResult> GetProductos([FromQuery] ProductoFiltrosDto filtrosDto)
        {
            var filtros = new ProductoFiltros
            {
                Categoria = filtrosDto.Categoria,
                PrecioMin = filtrosDto.PrecioMin,
                PrecioMax = filtrosDto.PrecioMax,
                Tipo = filtrosDto.Tipo
            };

            var resultado = await _productoService.GetProductos(
                filtrosDto.Page, filtrosDto.PageSize, filtros);

            return Ok(resultado);
        }

        // GET api/producto/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { Message = "El id no puede estar vacío" });

            var producto = await _productoService.GetProductoById(id);

            if (producto == null)
                return NotFound(new { Message = $"No se encontró el producto con id: {id}" });

            return Ok(producto);
        }

        // GET api/producto/categorias
        [HttpGet("categorias")]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _productoService.GetCategorias();
            return Ok(categorias);
        }

        // GET api/producto/vendedor/{vendedorId}
        [HttpGet("vendedor/{vendedorId}")]
        public async Task<IActionResult> GetByVendedor(string vendedorId)
        {
            var productos = await _productoService.GetProductosPorVendedor(vendedorId);
            return Ok(productos);
        }

        // POST api/producto
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear(CrearProductoDto dto)
        {
            var vendedorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Categoria = dto.Categoria,
                Tipo = dto.Tipo,
                Etiquetas = dto.Etiquetas,
                Inventario = dto.Inventario,
                Imagenes = dto.Imagenes
            };

            var creado = await _productoService.CrearProducto(producto, vendedorId);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT api/producto/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Actualizar(string id, CrearProductoDto dto)
        {
            var existente = await _productoService.GetProductoById(id);
            if (existente == null) return NotFound();

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;
            existente.Precio = dto.Precio;
            existente.Categoria = dto.Categoria;
            existente.Tipo = dto.Tipo;
            existente.Etiquetas = dto.Etiquetas;
            existente.Inventario = dto.Inventario;
            existente.Imagenes = dto.Imagenes;

            var actualizado = await _productoService.ActualizarProducto(existente);
            return Ok(actualizado);
        }

        // DELETE api/producto/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(string id)
        {
            var eliminado = await _productoService.EliminarProducto(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}