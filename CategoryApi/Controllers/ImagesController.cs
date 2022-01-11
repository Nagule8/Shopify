using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{id:int}")]
        public async Task<ActionResult<byte[]>> GetImage(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var image = _imageRepository.DisplayImage(id);

            return image;
        }

        // POST: api/Images
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost()]
        public async Task<ActionResult<Models.Image>> PostImage([FromForm]IFormFile image)
        {
            if(image == null)
            {
                return BadRequest();
            }
            var newImage = await _imageRepository.UploadImage(image);

            return CreatedAtAction("Uploaded Image", new { id = newImage.Id }, image);
        }

        
    }
}
