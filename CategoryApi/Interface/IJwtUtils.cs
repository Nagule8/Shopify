using ShopApi.Models;

namespace ShopApi.Interface
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(RegisterUser registerUser);
        public int? ValidateJwtToken(string token);
    }
}
