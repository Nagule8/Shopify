using Microsoft.AspNetCore.Http;
using ShopApi.Models;
using System.Drawing;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface IImageRepository
    {
        Task<Models.Image> UploadImage(IFormFile file);
        byte[] DisplayImage(int id);
    }
}
