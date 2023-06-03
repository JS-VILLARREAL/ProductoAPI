using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        //Inyección de Dependencias
        private readonly ILogger<ProductosController> _logger;

        // private readonly DataContext _db;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        /* Declarar un campo privado `_response` de tipo `APIResponse`. Este campo se utiliza para
        almacenar y devolver información sobre la respuesta de la API, como códigos de estado y
        mensajes de error. Se inicializa en el constructor de la clase `ProductosController`. */
        private APIResponse _response;

        /* Es responsable de inicializar los campos privados `_logger`, `_productRepo`, `_mapper` y `_response` con las dependencias correspondientes inyectadas a través del constructor.*/
        public ProductosController(ILogger<ProductosController> logger, IProductRepository productRepo, IMapper mapper)
        {
            _logger = logger;
            _productRepo = productRepo;
            _mapper = mapper;

            /* Está creando una nueva instancia de la clase `APIResponse` y asignándola al campo `_response`.
            Es para almacenar y devolver información sobre la respuesta de la API, como códigos de estado y mensajes de error. */
            _response = new();
        }

        /*Entity Framework Core Modelos de Base de Datos
        Para trabajar con una base de datos, es necesario trabajar Entity Framework Core
        Es un ORM (Object Relational Mapping), maneja las operaciones en la base de datos de manera sencilla, en vez
        de utilizar querys con sintasix complejas*/

        //Endpoint de consultar todo
        [HttpGet(Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetProducto()
        {
            try
            {
                _logger.LogInformation("Obtener los productos");

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

                _response.Results = _mapper.Map<IEnumerable<ProductoDto>>(productList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsSuccessful = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        //Endpoint de consultar por id
        [HttpGet("{id:int}", Name = "GetProductoId")]
        //Documentations Code Status - Documentación de codigo de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProductoId(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer producto con Id " + id);
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                /* Usa el objeto `_productRepo` para llamar al método `Get`, que toma
                una expresión lambda como parámetro para filtrar los resultados. En este caso, está
                filtrando los resultados para encontrar un objeto `Producto` con una propiedad `id` que
                coincida con el parámetro `id` pasado al método. El resultado se almacena luego en la
                variable `producto`. */
                var producto = await _productRepo.Get(v => v.idProduct == id);

                if (producto == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Results = _mapper.Map<ProductoDto>(producto);
                _response.IsSuccessful = true;
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        //Endpoint de crear
        [HttpPost(Name = "PostProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PostProducto(ProductoCreate productPost)
        {
            try
            {
                //Validar que si hay fallo en modelo
                if (!ModelState.IsValid)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
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

                _response.Results = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetProducto", new { id = modelo.idProduct }, _response);
            }
            catch (Exception ex)
            {

                _response.IsSuccessful = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        //Endpoint actualizar
        [HttpPut("{id:int}", Name = "PutProducto")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> PutProducto(int id, [FromBody] ProductoUpdate productPut)
        {
            try
            {
                if (productPut == null || id != productPut.idProduct)
                {
                    _response.IsSuccessful = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
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

                _response.Results = modelo;
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
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
                _response.IsSuccessful = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


            var product = await _productRepo.Get(v => v.idProduct == id, tracked: false);

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

            _response.Results = model;
            _response.IsSuccessful = true;
            _response.statusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        //Endpoint Eliminar
        [HttpDelete("{id:int}", Name = "DeleteProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteProducto(int id)
        {
            try
            {
                var producto = await _productRepo.Get(v => v.idProduct == id);

                if (producto == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _productRepo.Remove(producto);

                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}