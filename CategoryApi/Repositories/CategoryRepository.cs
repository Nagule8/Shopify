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
        private readonly CategoryApiContext _context;
        private readonly IDistributedCache _cache;

        public CategoryRepository(CategoryApiContext context, IDistributedCache cache )
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Category>> Get()
        {
            var _cachedData = _cache.GetString("categories");
            if (string.IsNullOrEmpty(_cachedData))
            {
                var res = await _context.Categories.ToListAsync();

                var _cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                _cache.SetString("categories", JsonConvert.SerializeObject(res), _cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<IEnumerable<Category>>(_cachedData); }
                
        }

        public async Task<Category> GetSpecific(int id)
        {
            var _cachedData = _cache.GetString("specific-category");
            if (string.IsNullOrEmpty(_cachedData))
            {
                var res = await _context.Categories
                .FirstOrDefaultAsync(e => e.Id == id);

                var _cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };
                _cache.SetString("specific-category", JsonConvert.SerializeObject(res), _cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<Category>(_cachedData); }
        }

        public async Task<Category> Add(Category category)
        {
                var res =await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<Category> Update(Category Category)
        {
            var result = await _context.Categories
                .FirstOrDefaultAsync(e => e.Id == Category.Id);

            if (result != null)
            {
                result.Name = Category.Name;
                result.Sorting = 10;

                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await _context.Categories
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                _context.Categories.Remove(result);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<Category> GetCategoryByName(string name)
        {
            var res = await _context.Categories
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
            return _context.Categories.Count(e => e.Id == id) > 0;
        }
    }
}