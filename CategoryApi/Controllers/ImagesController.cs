using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;
using ShopApi.Interface;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        //GET: api/Image/id
        [HttpGet("image")]
        public async Task<ActionResult<byte[]>> GetImage(string imageName)
        {

            var image =_imageRepository.DisplayImage(imageName);
            if(image == null)
            {
                return NotFound();
            }

            return image;
        }

        // POST: api/Images
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost()]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        public async Task<ActionResult<Image>> PostImage([FromForm]IFormFile image)
        {
            if(image == null)
            {
                ModelState.AddModelError("Image","Image is empty");
                return BadRequest(ModelState);
            }
            var newImage = await _imageRepository.UploadImage(image);

            return CreatedAtAction("Uploaded Image", new { id = newImage.Id }, image);
        }

        
    }
}
