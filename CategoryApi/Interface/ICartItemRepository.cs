using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopApi.Models;

namespace ShopApi.Interface
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetCartItems(int userId);
        Task<CartItem> GetCartItemByItemId(int itemId);
        Task<CartItem> IncreaseQuantity(int id,int quantity);
    }
}
