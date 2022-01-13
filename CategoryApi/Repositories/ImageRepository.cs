using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using ShopApi.Data;
using ShopApi.Interface;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly CategoryApiContext _context;
        private readonly IDistributedCache _cache;

        public ImageRepository(CategoryApiContext categoryApiContext, IDistributedCache cache)
        {
            _context = categoryApiContext;
            _cache = cache;
        }

        //Get image
        public byte[] DisplayImage(string imageName)
        {
            if(imageName == "")
            {
                return null;
            }
            var res = _context.Images.FirstOrDefault(e => e.Name == imageName);
            return res.DataFiles;


        }

        //POST image
        public async Task<Models.Image> UploadImage(IFormFile file)
        {
            if (!ImageExists(file.FileName))
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    var objimage = new Models.Image
                    {
                        Name = fileName,
                        FileType = fileExtension
                    };

                    using(var target = new MemoryStream())
                    {
                        file.CopyTo(target);
                        objimage.DataFiles = target.ToArray();
                    }

                    var res = await _context.Images.AddAsync(objimage);
                    await _context.SaveChangesAsync();

                    return res.Entity;
                }
            }

            return null;
        }

        private bool ImageExists(string imageName)
        {
            return _context.Images.Any(e => e.Name == imageName);
        }
    }
}
