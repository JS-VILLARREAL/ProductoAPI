using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIProducto.Models;

namespace WebAPIProducto.Repository.IRepository
{
    public interface IProductRepository : IRepository<Producto>
    {
        Task<Producto> Update(Producto entity);
    }
}