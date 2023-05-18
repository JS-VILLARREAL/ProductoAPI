using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIProducto.Data;
using WebAPIProducto.Models;
using WebAPIProducto.Repository.IRepository;

namespace WebAPIProducto.Repository
{
    public class ProviderRepository : Repository<ProvidersProduct>, IProviderRepository
    {
        //Inyectar DataContext
        private readonly DataContext _db;
        public ProviderRepository(DataContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ProvidersProduct> Update(ProvidersProduct entity)
        {
            entity.dateRegister = DateTime.Now;
            _db.Providers.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}