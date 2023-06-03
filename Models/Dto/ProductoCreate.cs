using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIProducto.Models.Dto
{
    /* La clase define propiedades para crear un producto, incluido su nombre, descripción, precio y
    estado activo. */
    public class ProductoCreate
    {
        [Required]
        public int idProduct { get; set; }
        [Required]
        [MaxLength(40)]
        public string? nameProduct { get; set; }
        public string? description { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public bool active { get; set; }
    }
}