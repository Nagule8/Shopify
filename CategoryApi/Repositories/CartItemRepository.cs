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
    public class CartItemRepository : ICartItemRepository, ICommonRepository<CartItem>
    {
        private readonly CategoryApiContext _context;
        private readonly IDistributedCache _cache;
        public CartItemRepository(CategoryApiContext categoryApiContext, IDistributedCache cache)
        {
            _context = categoryApiContext;
            _cache = cache;
        }

        //Add item in the cart
        public async Task<CartItem> Add(CartItem cartItem)
        {
            var res =await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            return res.Entity;
        }

        //Delete Cart item
        public async Task Delete(int id)
        {
            var result = await _context.CartItems
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                _context.CartItems.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        //Get specific cart item
        public async Task<CartItem> GetSpecific(int id)
        {
            var cachedData = _cache.GetString("cart-item");

            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await _context.CartItems.Include(e => e.RegisterUser)
                .FirstOrDefaultAsync(e => e.Id == id);
                var cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };

                _cache.SetString("cart-item", JsonConvert.SerializeObject(res), cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<CartItem>(cachedData); }
        }

        //Get cart items
        public async Task<IEnumerable<CartItem>> GetCartItems(int userId)
        {
            var cachedData = _cache.GetString("cart-items");
            if (string.IsNullOrEmpty(cachedData))
            {
                var res = await _context.CartItems.Include(e => e.RegisterUser)
                .Where(x => x.RegisterUserId == userId).ToListAsync();
                var cachedOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                };

                _cache.SetString("cart-items", JsonConvert.SerializeObject(res), cachedOptions);
                return res;
            }
            else { return JsonConvert.DeserializeObject<IEnumerable<CartItem>>(cachedData); }            
        }

        //Get cart item by item id
        public async Task<CartItem> GetCartItemByItemId(int itemId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(x => x.ItemId == itemId);
        }

        //Increase Quantity
        public async Task<CartItem> IncreaseQuantity(int id, int quantity)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(e => e.Id == id);
            try
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
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

        //Update Cart Item
        public async Task<CartItem> Update(CartItem cartItem)
        {
            var result = await _context.CartItems
                .FirstOrDefaultAsync(e => e.Id == cartItem.Id);

            if(result != null)
            {
                result.ItemId = cartItem.ItemId;
                result.ItemName = cartItem.ItemName;
                result.Quantity = cartItem.Quantity;
                result.Price = cartItem.Price;
                result.ImageName = cartItem.ImageName;
                result.RegisterUserId = cartItem.RegisterUserId;

                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        //Not IMplemented
        public Task<IEnumerable<CartItem>> Get()
        {
            throw new NotImplementedException();
        }

        public bool Exists(int id)
        {
            return _context.CartItems.Count(e => e.Id == id) > 0;
        }
    }
}
