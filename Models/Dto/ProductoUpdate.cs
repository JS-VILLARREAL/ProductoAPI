using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebAPIProducto.Models.Dto
{
    public class ProductoUpdate
    {
        [Required]
        public int id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? nameProduct { get; set; }
        [Required]
        public string? description { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public bool active { get; set; }
    }
}