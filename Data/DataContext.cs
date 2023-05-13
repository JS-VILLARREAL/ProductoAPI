using Microsoft.EntityFrameworkCore;
using WebAPIProducto.Models;

namespace WebAPIProducto.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}