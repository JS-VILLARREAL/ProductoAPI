using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIProducto.Data;
using WebAPIProducto.Models;
using WebAPIProducto.Models.Dto;
using WebAPIProducto.Repository.IRepository;

namespace WebAPIProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        //Logger Inyección de Dependencia
        private readonly ILogger<ProductosController> _logger;

        // private readonly DataContext _db;

        /* Declarar un campo privado de solo lectura de tipo `IProductRepository` llamado
        `_productRepo`. Se usa para acceder a métodos y propiedades definidos en la interfaz `IProductRepository`,
        que se implementa mediante una clase concreta en otra parte del código. */
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductosController(ILogger<ProductosController> logger, IProductRepository productRepo, IMapper mapper)
        {
            _logger = logger;
            _productRepo = productRepo;
            _mapper = mapper;
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

            /* Recuperar todos los objetos `Producto` de la base de datos y almacenarlos en una
            colección `IEnumerable` llamada `productList`. El método `ToListAsync()` se utiliza para
            recuperar de forma asincrónica los objetos de la base de datos.
            IEnumerable<Producto> productList = await _db.Productos.ToListAsync(); */

            /* Está recuperando todos los objetos `Producto` de la base de datos utilizando el método `GetAll()`
            definido en la interfaz `IProductRepository`, que se implementa en otra parte del
            código. Los objetos recuperados luego se almacenan en una colección de tipo
            `IEnumerable<Producto>` llamada `productList`. La palabra clave `await` se utiliza para
            recuperar de forma asíncrona los objetos de la base de datos. */
            IEnumerable<Producto> productList = await _productRepo.GetAll();

            return Ok(_mapper.Map<IEnumerable<ProductoDto>>(productList));
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

            /* Usa el objeto `_productRepo` para llamar al método `Get`, que toma
            una expresión lambda como parámetro para filtrar los resultados. En este caso, está
            filtrando los resultados para encontrar un objeto `Producto` con una propiedad `id` que
            coincida con el parámetro `id` pasado al método. El resultado se almacena luego en la
            variable `producto`. */
            var producto = await _productRepo.Get(v => v.id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductoDto>(producto));
        }

        //Endpoint de crear
        [HttpPost(Name = "PostProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoCreate productPost)
        {
            //Validar que si hay fallo en modelo
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            /* Este bloque de código está comprobando si ya existe un producto en la base de datos con
            el mismo nombre que el que se está creando. Si lo hay, agrega un error de estado del
            modelo con la clave "NombreExiste" y el mensaje "El nombre ya existe" y devuelve una
            solicitud incorrecta con el estado del modelo. Esta es una validación personalizada para
            garantizar que no haya nombres de productos duplicados en la base de datos. */
            if (await _productRepo.Get(v => v.nameProduct.ToLower() == productPost.nameProduct.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "El nombre ya existe");
                return BadRequest(ModelState);
            }

            if (productPost == null)
            {
                return BadRequest(productPost);
            }

            //Antes se utilizo esto
            //Crear nuevo modelo
            // Producto modelo = new()
            // {
            //     nameProduct = producto.nameProduct,
            //     description = producto.description,
            //     price = producto.price,
            //     active = producto.active
            // };

            /* Esta línea de código usa AutoMapper para mapear las propiedades de un objeto
            `ProductoCreate` (`productPost`) a un objeto `Producto` (`modelo`). Esto permite una
            fácil conversión entre diferentes tipos de objetos con propiedades similares. */
            Producto modelo = _mapper.Map<Producto>(productPost);

            await _productRepo.Create(modelo);

            return new CreatedAtRouteResult("GetProducto", new { id = modelo.id }, modelo);
        }

        //Endpoint actualizar
        [HttpPut("{id:int}", Name = "PutProducto")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> PutProducto(int id, [FromBody] ProductoUpdate productPut)
        {
            if (productPut == null || id != productPut.id)
            {
                return BadRequest();
            }

            // Producto modelo = new()
            // {
            //     id = producto.id,
            //     nameProduct = producto.nameProduct,
            //     description = producto.description,
            //     price = producto.price,
            //     active = producto.active
            // };

            //Mapper
            Producto modelo = _mapper.Map<Producto>(productPut);

            await _productRepo.Update(modelo);

            return Ok(modelo);
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


            var product = await _productRepo.Get(v => v.id == id, tracked: false);

            ProductoUpdate modelo = _mapper.Map<ProductoUpdate>(product);
            // ProductoUpdate modelo = new()
            // {
            //     id = product.id,
            //     nameProduct = product.nameProduct,
            //     description = product.description,
            //     price = product.price,
            //     active = product.active
            // };

            if (product == null) return BadRequest();

            productPatch.ApplyTo(modelo, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Producto model = _mapper.Map<Producto>(modelo);

            // Producto model = new()
            // {
            //     id = modelo.id,
            //     nameProduct = modelo.nameProduct,
            //     description = modelo.description,
            //     price = modelo.price,
            //     active = modelo.active
            // };

            //se le informa que el objeto producto ha sido modificado
            await _productRepo.Update(model);

            return Ok(model);
        }

        //Endpoint Eliminar
        [HttpDelete("{id:int}", Name = "DeleteProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Producto>> DeleteProducto(int id)
        {
            var producto = await _productRepo.Get(v => v.id == id);

            if (producto == null)
            {
                return NotFound();
            }

            await _productRepo.Remove(producto);

            return Ok(producto);
        }
    }
}