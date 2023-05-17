using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIProducto.Models;

namespace WebAPIProducto.Repository.IRepository
{
    public interface IProviderRepository : IRepository<ProvidersProduct>
    {
        Task<ProvidersProduct> Update(ProvidersProduct entity);
    }
}