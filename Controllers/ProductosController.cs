using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIProducto.Data;
using WebAPIProducto.Models;

namespace WebAPIProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        //Logger Inyección de Dependencia
        private readonly ILogger<ProductosController> _logger;
        private readonly DataContext _context;

        public ProductosController(ILogger<ProductosController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        //Entity Framework Core Modelos de Base de Datos
        //Para trabajar con una base de datos, es necesario trabajar Entity Framework Core
        //Es un ORM (Object Relational Mapping), maneja las operaciones en la base de datos de manera sencilla, en vez
        //de utilizar querys con sintasix complejas

        //Endpoint de consultar todo
        [HttpGet(Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProducto()
        {
            //_logger.LogInformation("Obtener las villas");
            //return Ok(VillaStoreClass.villaList);
            return Ok(await _context.Productos.ToListAsync());
        }

        //Endpoint de consultar por id
        [HttpGet("{id}", Name = "GetProductoId")]
        //Documentations Code Status - Documentación de codigo de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Producto>> GetProductoId(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        //Endpoint de crear
        [HttpPost(Name = "PostProducto")]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            await _context.AddAsync(producto);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("GetProducto", new { id = producto.id }, producto);
        }

        //Endpoint actualizar
        [HttpPut("{id}", Name = "PutProducto")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.id)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        //Endpoint Eliminar
        [HttpDelete("{id}", Name = "DeleteProducto")]
        public async Task<ActionResult<Producto>> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Remove(producto);
            await _context.SaveChangesAsync();

            return producto;
        }

    }
}