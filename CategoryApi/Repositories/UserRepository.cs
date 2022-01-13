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
        private readonly CategoryApiContext _context;
        public UserRepository(CategoryApiContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RegisterUser>> Get()
        {
            return await _context.RegisterUsers.ToListAsync();
        }

        public async Task<RegisterUser> GetSpecific(int id)
        {
            var res = await _context.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == id);
            return res;

        }
        public async Task<int> GetUserId(string username)
        {
            RegisterUser registerUser = await _context.RegisterUsers
                .FirstOrDefaultAsync(e => e.UserName == username);

            if (registerUser != null)
            {
                return (registerUser.Id);
            }

            return 0;

        }

        public async  Task<RegisterUser> GetUserByName(string username)
        {
            var res = await _context.RegisterUsers
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
            var res = await _context.RegisterUsers.AddAsync(user1);
            await _context.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<RegisterUser> Update(RegisterUser user)
        {
            var result = await _context.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == user.Id);

            if (result != null)
            {
                result.UserName = user.UserName;
                result.Email = user.Email;
                result.Role = user.Role;
                result.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }
        public async Task Delete(int id)
        {
            var result = await _context.RegisterUsers
                .FirstOrDefaultAsync(e => e.Id == id);
            if(result != null)
            {
                _context.RegisterUsers.Remove(result);
                await _context.SaveChangesAsync();
            }

        }

        public bool Exists(int id)
        {
            return _context.CartItems.Count(e => e.Id == id) > 0;
        }

    }
}
