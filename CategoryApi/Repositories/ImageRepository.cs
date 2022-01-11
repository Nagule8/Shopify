using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ShopApi.Data;
using ShopApi.Interface;
using ShopApi.Models;
using System;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShopApi.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly CategoryApiContext _categoryApiContext;
        private readonly IDistributedCache _cache;
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        public ImageRepository(CategoryApiContext categoryApiContext, IDistributedCache cache)
        {
            _categoryApiContext = categoryApiContext;
            _cache = cache;
        }

        //Get image
        public byte[] DisplayImage(int id)
        {
            if(id == 0)
            {
                return null;
            }
            var res = _categoryApiContext.Images.FirstOrDefault(e => e.Id == id);
            return res.DataFiles;


        }

        //POST image
        public async Task<Models.Image> UploadImage(IFormFile file)
        {

            if (file != null)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    var objimage = new Models.Image
                    {
                        Name = newFileName,
                        FileType = fileExtension
                    };

                    using(var target = new MemoryStream())
                    {
                        file.CopyTo(target);
                        objimage.DataFiles = target.ToArray();
                    }

                    var res = await _categoryApiContext.Images.AddAsync(objimage);
                    await _categoryApiContext.SaveChangesAsync();

                    return res.Entity;
                }
            }

            return null;
        }

        private bool ImageExists(int id)
        {
            return _categoryApiContext.Images.Any(e => e.Id == id);
        }
    }
}
