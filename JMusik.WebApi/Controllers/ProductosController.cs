using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JMusik.Data;
using JMusik.Models;
using JMusik.Data.Contratos;
using AutoMapper;
using Musik.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using JMusik.WebApi.Helpers;

namespace JMusik.WebApi.Controllers
{
    //[Authorize(Roles = "Administrador,Vendedor")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private IProductosRepositorio _productosRepositorio;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductosRepositorio productosRepositorio, 
               IMapper mapper,
               ILogger<ProductosController> logger)
        {
            _productosRepositorio = productosRepositorio;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<IEnumerable<ProductoDto>>> Get()
        public async Task<ActionResult<Paginador<ProductoDto>>> Get(
            int paginaActual = 1, int registrosPorPagina = 3)
        {
            try
            {
                var resultado = await _productosRepositorio.ObtenerPaginasProductosAsync(paginaActual, registrosPorPagina);

                var listaProductosDto = _mapper.Map<List<ProductoDto>>(resultado.registros);

                return new Paginador<ProductoDto>(listaProductosDto, resultado.totalRegistros, paginaActual, registrosPorPagina);

                //var productos = await _productosRepositorio.ObtenerProductosAsync();
                //return _mapper.Map<List<ProductoDto>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(Get)}: | ${ex.Message}");
                return BadRequest();
            }

        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Get(int id)
        {
            try
            {
                var producto = await _productosRepositorio.ObtenerProductoAsync(id);
                if (producto == null)
                {
                    return NotFound();
                }
                //return producto;
                return _mapper.Map<ProductoDto>(producto);
            }
            catch (Exception)
            {
                _logger.LogError($"Error al obtener producto");
                return BadRequest();
            }
        }

        // POST: api/Productos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Post(ProductoDto productoDto)
        {
            try
            {
                var producto = _mapper.Map<Producto>(productoDto);
                var nuevoProducto = await _productosRepositorio.Agregar(producto);
                if (nuevoProducto == null)
                {
                    return BadRequest();
                }

                var nuevoProductoDto = _mapper.Map<ProductoDto>(nuevoProducto);
                return CreatedAtAction(nameof(Post), new { id = nuevoProducto.Id }, nuevoProductoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(Post)}: | ${ex.Message}");
                return BadRequest();
            }

        }

        //// PUT: api/Productos/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Put(int id, [FromBody] ProductoDto productoDto)
        {
            if (productoDto == null)
            {
                return NotFound();
            }

            try
            {
                var producto = _mapper.Map<Producto>(productoDto);

                var resultado = await _productosRepositorio.Actualizar(producto);
                if (!resultado)
                {
                    return BadRequest();
                }

                return productoDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(Put)}: | ${ex.Message}");
                return BadRequest();
            }
        }

        // DELETE: api/Productos/id
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _productosRepositorio.Eliminar(id);
                if (!resultado)
                {
                    return BadRequest();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(Delete)}: | ${ex.Message}");
                return BadRequest();
            }
        }

        //// GET: api/Productos
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        //{
        //    return await _context.Productos.ToListAsync();
        //}

        //// GET: api/Productos/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Producto>> GetProducto(int id)
        //{
        //    var producto = await _context.Productos.FindAsync(id);

        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }

        //    return producto;
        //}

        //// PUT: api/Productos/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProducto(int id, Producto producto)
        //{
        //    if (id != producto.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(producto).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Productos
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        //{
        //    _context.Productos.Add(producto);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        //}

        //// DELETE: api/Productos/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Producto>> DeleteProducto(int id)
        //{
        //    var producto = await _context.Productos.FindAsync(id);
        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Productos.Remove(producto);
        //    await _context.SaveChangesAsync();

        //    return producto;
        //}

        //private bool ProductoExists(int id)
        //{
        //    return _context.Productos.Any(e => e.Id == id);
        //}
    }
}
