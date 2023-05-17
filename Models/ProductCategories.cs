using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIProducto.Models
{
    public class ProductCategories
    {
        [Key]
        public int idCategory { get; set; }
        public string? nameCategory { get; set; }
    }
}