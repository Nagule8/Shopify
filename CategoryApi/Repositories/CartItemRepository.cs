using ShopApi.Interface;
using ShopApi.Data;
using ShopApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Newtonsoft.Json;

namespace ShopApi.Services
{
    public class CartItemRepository : ICartItemRepository,ICommonRepository<CartItem>
    {
        private readonly CategoryApiContext categoryApiContext;
        private readonly IDistributedCache cache;
        public CartItemRepository(CategoryApiContext categoryApiContext, IDistributedCache cache)
        {
            this.categoryApiContext = categoryApiContext;
            this.cache = cache;
        }
        public async Task<CartItem> Add(CartItem cartItem)
        {
            var res =await categoryApiContext.CartItems.AddAsync(cartItem);
            await categoryApiContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task Delete(int id)
        {
            var result = await categoryApiContext.CartItems
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                categoryApiContext.CartItems.Remove(result);
                await categoryApiContext.SaveChangesAsync();
            }
        }

        public async Task<CartItem> GetSpecific(int id)
        {
            var res = await categoryApiContext.CartItems.Include(e=>e.RegisterUser)
                .FirstOrDefaultAsync(e => e.Id == id);
            return res;
        }

        public async Task<IEnumerable<CartItem>> GetCartItems(int userId)
        {
            var cachedData = cache.GetString("cart-item");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await categoryApiContext.CartItems.Include(e => e.RegisterUser)
                .Where(x => x.RegisterUserId == userId).ToListAsync();
                var cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };

                cache.SetString("cart-item", JsonConvert.SerializeObject(res), cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<IEnumerable<CartItem>>(cachedData); }            
        }

        public async Task<CartItem> GetCartItemByItemId(int itemId)
        {
            return await categoryApiContext.CartItems.FirstOrDefaultAsync(x => x.ItemId == itemId);
        }

        public async Task<CartItem> IncreaseQuantity(int id, int quantity)
        {
            var cartItem = await categoryApiContext.CartItems.FirstOrDefaultAsync(e => e.Id == id);
            try
            {
                cartItem.Quantity = quantity;
                await categoryApiContext.SaveChangesAsync();
                return cartItem;
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<CartItem> Update(CartItem cartItem)
        {
            var result = await categoryApiContext.CartItems
                .FirstOrDefaultAsync(e => e.Id == cartItem.Id);

            if(result != null)
            {
                result.ItemId = cartItem.ItemId;
                result.ItemName = cartItem.ItemName;
                result.Quantity = cartItem.Quantity;
                result.Price = cartItem.Price;
                result.ImageName = cartItem.ImageName;
                result.RegisterUserId = cartItem.RegisterUserId;

                await categoryApiContext.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public Task<IEnumerable<CartItem>> Get()
        {
            throw new NotImplementedException();
        }
        public bool Exists(int id)
        {
            return categoryApiContext.CartItems.Count(e => e.Id == id) > 0;
        }
    }
}
