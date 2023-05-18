using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebAPIProducto.Models
{
    public class APIResponse
    {
        public HttpStatusCode statusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public object? Results { get; set; }

    }
}