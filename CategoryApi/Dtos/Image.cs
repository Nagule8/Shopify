using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Dtos
{
    public class Image
    {
        public IFormFile Images { get; set; }
    }
}
