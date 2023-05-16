using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebAPIProducto.Models;
using WebAPIProducto.Models.Dto;

namespace WebAPIProducto
{
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