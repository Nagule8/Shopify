using System.Threading.Tasks;
using ShopApi.Models;

namespace ShopApi.Interface
{
    public interface IItemRepository
    {
        Task<Item> GetItemBySlug(string slug);
    }
}
