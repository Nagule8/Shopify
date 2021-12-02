using ShopApi.Data;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Services
{
    public class UserRepository : IUserRepository,ICommonRepository<RegisterUser>
    {
        private readonly CategoryApiContext categoryApiContext;
        public UserRepository(CategoryApiContext categoryApiContext)
        {
            this.categoryApiContext = categoryApiContext;
        }
        public async Task<IEnumerable<RegisterUser>> Get()
        {
            return await categoryApiContext.RegisterUsers.ToListAsync();
        }

        public async Task<RegisterUser> GetSpecific(int id)
        {
            var res = await categoryApiContext.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == id);
            return res;

        }
        public async Task<int> GetUserId(string username)
        {
            RegisterUser registerUser = await categoryApiContext.RegisterUsers
                .FirstOrDefaultAsync(e => e.UserName == username);

            if (registerUser != null)
            {
                return (registerUser.Id);
            }

            return 0;

        }

        public async  Task<RegisterUser> GetUserByName(string username)
        {
            var res = await categoryApiContext.RegisterUsers
                .FirstOrDefaultAsync(e => e.UserName == username);

            return res;

        }

        public async Task<RegisterUser> Add(RegisterUser user)
        {
            var user1 = new RegisterUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
        };
            var res = await categoryApiContext.RegisterUsers.AddAsync(user1);
            await categoryApiContext.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<RegisterUser> Update(RegisterUser user)
        {
            var result = await categoryApiContext.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == user.Id);

            if (result != null)
            {
                result.UserName = user.UserName;
                result.Email = user.Email;
                result.Role = user.Role;
                result.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                await categoryApiContext.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await categoryApiContext.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == id);
            if(result != null)
            {
                categoryApiContext.RegisterUsers.Remove(result);
                await categoryApiContext.SaveChangesAsync();
            }

        }

        public bool Exists(int id)
        {
            return categoryApiContext.CartItems.Count(e => e.Id == id) > 0;
        }

    }
}
