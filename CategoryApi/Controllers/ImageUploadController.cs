using ShopApi.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    public class ImageUploadController : ControllerBase
    {
        private static IWebHostEnvironment _webHostEnvironment;

        public ImageUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        [Authorize(new[] { Role.SuperSu,Role.Administrator })]
        public async Task<string> Upload([FromForm]Image image)
        {
            try
            {
                if ( image.Images.FileName.Length > 0)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\Photos\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\Photos\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\Photos\\" + image.Images.FileName))
                    {
                        image.Images.CopyTo(fileStream);
                        fileStream.Flush();
                        return (image.Images.FileName);

                    }
                }
                else
                {
                    return "noimage.png";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

    }
}
