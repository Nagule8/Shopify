using ShopApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface IUserRepository
    {
        Task<RegisterUser> GetUserByName(string email);
        Task<int> GetUserId(string username);
    }
}
