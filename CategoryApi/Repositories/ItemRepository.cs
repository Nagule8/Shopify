using ShopApi.Data;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Services
{
    public class ItemRepository : IItemRepository,ICommonRepository<Item>
    {
        private readonly CategoryApiContext _context;
        private readonly IDistributedCache _cache;

        public ItemRepository(CategoryApiContext context, IDistributedCache cache)
        {
            _cache = cache;
            _context = context;
        }

        public async Task<IEnumerable<Item>> Get()
        {
            var cachedData = _cache.GetString("items");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await _context.Items.Include(e => e.Category).ToListAsync();
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                _cache.SetString("items", JsonConvert.SerializeObject(res), cacheOptions);

                return res;
            }
            else
            {
                return JsonConvert.DeserializeObject<IEnumerable<Item>>(cachedData);
            }

        }

        public async Task<Item> GetSpecific(int id)
        {
            var cachedData = _cache.GetString("specific-item");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await _context.Items.Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                _cache.SetString("specific-item", JsonConvert.SerializeObject(res), cacheOptions);

                return res;
            }
            else
            {
                return JsonConvert.DeserializeObject<Item>(cachedData);
            }

        }
        public async Task<Item> GetItemBySlug(string name)
        {
            var res = await _context.Items.FirstOrDefaultAsync(e => e.Name == name);

            return res;

        }

        public async Task<Item> Add(Item Item)
        {
            var res = await _context.Items.AddAsync(Item);
            await _context.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<Item> Update(Item Item)
        {
            var result = await _context.Items
                .FirstOrDefaultAsync(e => e.Id == Item.Id);

            if (result != null)
            {
                result.Name = Item.Name;
                result.Description = Item.Description;
                result.Price = Item.Price;
                result.CategoryId = Item.CategoryId;
                result.ImageName = Item.ImageName;

                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await _context.Items
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                _context.Items.Remove(result);
                await _context.SaveChangesAsync();
            }

        }

        public bool Exists(int id)
        {
            return _context.CartItems.Count(e => e.Id == id) > 0;
        }
    }
}
