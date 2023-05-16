using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIProducto.Data;
using WebAPIProducto.Models;
using WebAPIProducto.Models.Dto;

namespace WebAPIProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        //Logger Inyección de Dependencia
        private readonly ILogger<ProductosController> _logger;
        private readonly DataContext _db;

        public ProductosController(ILogger<ProductosController> logger, DataContext db)
        {
            _logger = logger;
            _db = db;
        }
        //Entity Framework Core Modelos de Base de Datos
        //Para trabajar con una base de datos, es necesario trabajar Entity Framework Core
        //Es un ORM (Object Relational Mapping), maneja las operaciones en la base de datos de manera sencilla, en vez
        //de utilizar querys con sintasix complejas

        //Endpoint de consultar todo
        [HttpGet(Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProducto()
        {
            _logger.LogInformation("Obtener las villas");
            return Ok(await _db.Productos.ToListAsync());
        }

        //Endpoint de consultar por id
        [HttpGet("{id:int}", Name = "GetProductoId")]
        //Documentations Code Status - Documentación de codigo de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> GetProductoId(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer villa con Id " + id);
                return BadRequest();
            }

            var producto = await _db.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        //Endpoint de crear
        [HttpPost(Name = "PostProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoCreate producto)
        {
            //Validar que si hay fallo en modelo
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //Validaciones Personalizadas
            if (await _db.Productos.FirstOrDefaultAsync(v => v.nameProduct.ToLower() == producto.nameProduct.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "El nombre ya existe");
                return BadRequest(ModelState);
            }

            if (producto == null)
            {
                return BadRequest();
            }
            //Crear nuevo modelo
            Producto modelo = new()
            {
                nameProduct = producto.nameProduct,
                description = producto.description,
                price = producto.price,
                active = producto.active
            };


            await _db.AddAsync(modelo);
            await _db.SaveChangesAsync();

            return new CreatedAtRouteResult("GetProducto", new { id = modelo.id }, modelo);
        }

        //Endpoint actualizar
        [HttpPut("{id:int}", Name = "PutProducto")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PutProducto(int id, [FromBody] ProductoUpdate producto)
        {
            if (producto == null || id != producto.id)
            {
                return BadRequest();
            }

            Producto modelo = new()
            {
                id = producto.id,
                nameProduct = producto.nameProduct,
                description = producto.description,
                price = producto.price,
                active = producto.active
            };

            //se le informa que el objeto producto ha sido modificado
            _db.Entry(modelo).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Endpoint de actualizar por partes, solo una propiedad
        [HttpPatch("{id:int}", Name = "PatchProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchProducto(int id, JsonPatchDocument<ProductoUpdate> productPatch)
        {

            if (productPatch == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStoreClass.villaList.FirstOrDefault(v => v.Id == id);
            var product = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(v => v.id == id);

            ProductoUpdate modelo = new()
            {
                id = product.id,
                nameProduct = product.nameProduct,
                description = product.description,
                price = product.price,
                active = product.active
            };

            if (product == null) return BadRequest();

            productPatch.ApplyTo(modelo, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Producto model = new()
            {
                id = modelo.id,
                nameProduct = modelo.nameProduct,
                description = modelo.description,
                price = modelo.price,
                active = modelo.active
            };

            //se le informa que el objeto producto ha sido modificado
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Endpoint Eliminar
        [HttpDelete("{id:int}", Name = "DeleteProducto")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Producto>> DeleteProducto(int id)
        {
            var producto = await _db.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _db.Remove(producto);
            await _db.SaveChangesAsync();

            return producto;
        }
    }
}