using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface IImageRepository
    {
        Task<Models.Image> UploadImage(IFormFile file);
        byte[] DisplayImage(string imageName);
    }
}
