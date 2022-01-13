using Microsoft.AspNetCore.Http;

namespace ShopApi.Dtos
{
    public class Image
    {
        public IFormFile Images { get; set; }
    }
}
