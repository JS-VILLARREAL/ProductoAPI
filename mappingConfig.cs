using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebAPIProducto.Models;
using WebAPIProducto.Models.Dto;

namespace WebAPIProducto
{
    /* La clase mappingConfig crea mapeos entre diferentes tipos de objetos Producto y sus DTO
    correspondientes y modelos de actualización/creación. */
    public class mappingConfig : Profile
    {
        public mappingConfig()
        {
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoDto, Producto>();

            CreateMap<Producto, ProductoCreate>().ReverseMap();
            CreateMap<Producto, ProductoUpdate>().ReverseMap();
        }
    }
}