using ShopApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(RegisterUser registerUser);
        public int? ValidateJwtToken(string token);
    }
}
