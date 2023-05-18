using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIProducto.Data;
using WebAPIProducto.Models;
using WebAPIProducto.Repository.IRepository;

namespace WebAPIProducto.Repository
{
    public class ProductRepository : Repository<Producto>, IProductRepository
    {
        //Inyectar DataContext
        private readonly DataContext _db;
        public ProductRepository(DataContext db): base(db)
        {
            _db = db;
        }

        public async Task<Producto> Update(Producto entity)
        {
            entity.highDate = DateTime.Now;
            _db.Productos.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}