using System;
using System.Threading.Tasks;
using ShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShopApi.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using ShopApi.Authorize;
using ShopApi.Entity;
using Microsoft.Extensions.Logging;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(new[] { Role.SuperSu, Role.Administrator })]
    public class ItemsController : ControllerBase
    {
        private readonly ICommonRepository<Item> _commonRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IDistributedCache _cache;

        public ItemsController(ICommonRepository<Item> commonRepository, IItemRepository itemRepository, IDistributedCache cache)
        {
            _commonRepository = commonRepository;
            _itemRepository = itemRepository;
            _cache = cache;
        }

        // GET: api/Items
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetItems()
        {
                var result = await _commonRepository.Get();

                return Ok(result);

        }

        // GET: api/Items/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
                Item item = await _commonRepository.GetSpecific(id);
                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
        }

        // PUT: api/Items/5
        [HttpPut("{id:int}")]
        
        public async Task<ActionResult<Item>> PutItem(int id, Item item)
        {
                var itemToUpdate = await _commonRepository.GetSpecific(id);
                if (itemToUpdate == null)
                {
                    return NotFound($"Item with id:{id} not found");
                }
                return await _commonRepository.Update(item);
        }

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {

                var item1 = await _itemRepository.GetItemBySlug(item.Name);
                if (item1 != null)
                {
                    ModelState.AddModelError("Item", "Item already exist.");
                    return BadRequest(ModelState);
                }
                var newitem = await _commonRepository.Add(item);


                return CreatedAtAction(nameof(GetItem),
                    new { id = newitem.Id }, newitem);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
                Item item = await _commonRepository.GetSpecific(id);
                if (item == null)
                {
                    return NotFound($"Item with id:{id} not found.");
                }

                await _commonRepository.Delete(id);

                return Ok($"Item with id:{id} Deleted.");
        }

        /*//Save Image
        [Route("api/items/SaveFile")]
        public string SaveFile()
        {
            try
            {
                var httpRequest = HttpContextHelper.Current.Request;
                var uploadedFile = httpRequest.Files[0];
                string fileName = uploadedFile.FileName;
                var physicalPath = HttpContextHelper.Current.Server.MapPath("~/Photos/"+fileName);

                uploadedFile.SaveAs(physicalPath);

                return (fileName);
            }
            catch(Exception)
            {
                return("noimage.png");
            }
        }*/
    }
}