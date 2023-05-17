using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIProducto.Models.Dto
{
    public class ProvidersCreate
    {
        [Required]
        public string? nameProvider { get; set; }
        public string? address { get; set; }
        public string? email { get; set; }
        public decimal priceProduct { get; set; }
    }
}