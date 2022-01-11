using System.Threading.Tasks;
using ShopApi.Models;

namespace ShopApi.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByName(string name);
    }
}
