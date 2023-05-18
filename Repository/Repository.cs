using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPIProducto.Data;
using WebAPIProducto.Repository.IRepository;

namespace WebAPIProducto.Repository
{
    // Se trata de una clase genérica de repositorio que implementa operaciones CRUD para un tipo de entidad determinado.
    public class Repository<T> : IRepository<T> where T : class
    {
        //Inyectar DataContext
        private readonly DataContext? _db;
        internal DbSet<T> dbSet;

        public Repository(DataContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

    }
}