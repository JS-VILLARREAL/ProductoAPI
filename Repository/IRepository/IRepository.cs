using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebAPIProducto.Repository.IRepository
{
    /* Esta es una interfaz llamada `IRepository` que define las operaciones CRUD (Crear, Leer,
    Actualizar, Eliminar) básicas para un tipo genérico `T`. La `T` es un marcador de posición para
    cualquier clase que implemente esta interfaz. La interfaz incluye métodos para crear, obtener,
    eliminar y guardar entidades de tipo `T`. El método `GetAll` toma una expresión de filtro
    opcional para recuperar un subconjunto de entidades. El método `Get` también toma una expresión
    de filtro y un indicador booleano para indicar si el contexto debe rastrear la entidad. */
    public interface IRepository<T> where T : class
    {
        Task Create(T entity);
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);

        Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracked = true);

        Task Remove(T entity);

        Task Save();

    }
}