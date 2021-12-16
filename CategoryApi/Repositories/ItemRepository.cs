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
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.Claims;

namespace ShopApi.Services
{
    public class ItemRepository : IItemRepository,ICommonRepository<Item>
    {
        private readonly CategoryApiContext categoryApiContext;
        private readonly IDistributedCache cache;

        public ItemRepository(CategoryApiContext categoryApiContext, IDistributedCache cache)
        {
            this.cache = cache;
            this.categoryApiContext = categoryApiContext;
        }

        public async Task<IEnumerable<Item>> Get()
        {
            var cachedData = cache.GetString("items");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await categoryApiContext.Items.Include(e => e.Category).ToListAsync();
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                cache.SetString("items", JsonConvert.SerializeObject(res), cacheOptions);

                return res;
            }
            else
            {
                return JsonConvert.DeserializeObject<IEnumerable<Item>>(cachedData);
            }

        }

        public async Task<Item> GetSpecific(int id)
        {
            var cachedData = cache.GetString("specific-item");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await categoryApiContext.Items.Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                cache.SetString("specific-item", JsonConvert.SerializeObject(res), cacheOptions);

                return res;
            }
            else
            {
                return JsonConvert.DeserializeObject<Item>(cachedData);
            }

        }
        public async Task<Item> GetItemBySlug(string name)
        {
            var res = await categoryApiContext.Items.FirstOrDefaultAsync(e => e.Name == name);

            return res;

        }

        public async Task<Item> Add(Item Item)
        {
            var res = await categoryApiContext.Items.AddAsync(Item);
            await categoryApiContext.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<Item> Update(Item Item)
        {
            var result = await categoryApiContext.Items
                .FirstOrDefaultAsync(e => e.Id == Item.Id);

            if (result != null)
            {
                result.Name = Item.Name;
                result.Description = Item.Description;
                result.Price = Item.Price;
                result.CategoryId = Item.CategoryId;
                result.ImageName = Item.ImageName;

                await categoryApiContext.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await categoryApiContext.Items
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                categoryApiContext.Items.Remove(result);
                await categoryApiContext.SaveChangesAsync();
            }

        }

        public bool Exists(int id)
        {
            return categoryApiContext.CartItems.Count(e => e.Id == id) > 0;
        }
    }
}
