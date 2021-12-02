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
    public class CategoryRepository : ICategoryRepository, ICommonRepository<Category>
    {
        private readonly CategoryApiContext categoryApiContext;
        private readonly IDistributedCache cache;

        public CategoryRepository(CategoryApiContext categoryApiContext, IDistributedCache cache )
        {
            this.categoryApiContext = categoryApiContext;
            this.cache = cache;
        }

        public async Task<IEnumerable<Category>> Get()
        {
            var cachedData = cache.GetString("categories");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await categoryApiContext.Categories.ToListAsync();

                var cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                cache.SetString("categories", JsonConvert.SerializeObject(res), cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<IEnumerable<Category>>(cachedData); }
                
        }

        public async Task<Category> GetSpecific(int id)
        {
            var cachedData = cache.GetString("specific-category");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await categoryApiContext.Categories
                .FirstOrDefaultAsync(e => e.Id == id);

                var cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                cache.SetString("specific-category", JsonConvert.SerializeObject(res), cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<Category>(cachedData); }
        }

        public async Task<Category> Add(Category category)
        {
                var res =await categoryApiContext.Categories.AddAsync(category);
            await categoryApiContext.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<Category> Update(Category Category)
        {
            var result = await categoryApiContext.Categories
                .FirstOrDefaultAsync(e => e.Id == Category.Id);

            if (result != null)
            {
                result.Name = Category.Name;
                result.Sorting = 10;

                await categoryApiContext.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await categoryApiContext.Categories
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                categoryApiContext.Categories.Remove(result);
                await categoryApiContext.SaveChangesAsync();
            }

        }

        public async Task<Category> GetCategoryByName(string name)
        {
            var res = await categoryApiContext.Categories
        .FirstOrDefaultAsync(e => e.Name == name);
            try
            {
                return res;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public bool Exists(int id)
        {
            return categoryApiContext.Categories.Count(e => e.Id == id) > 0;
        }
    }
}