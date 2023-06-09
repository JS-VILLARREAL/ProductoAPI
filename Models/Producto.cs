using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace WebAPIProducto.Models
{
    public class Producto
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("id")]
        public ProvidersProduct ProvidersProduct { get; set; }
        public string? nameProduct { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public bool active { get; set; }
        public DateTime highDate { get; set; }
    }
}