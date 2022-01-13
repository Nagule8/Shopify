using ShopApi.Models;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface IUserRepository
    {
        Task<RegisterUser> GetUserByName(string email);
        Task<int> GetUserId(string username);
    }
}
