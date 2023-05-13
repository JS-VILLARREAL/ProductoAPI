using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebAPIProducto.Models
{
    public class ProductoCrear
    {
        public int id { get; set; }
        [Required]
        public string? nameProduct { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public DateTime highDate { get; set; }
        public bool active { get; set; }
    }
}