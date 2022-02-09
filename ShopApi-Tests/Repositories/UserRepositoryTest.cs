using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ShopApi_Tests.Repositories
{
    public class UserRepositoryTest
    {
        private readonly CategoryApiContext context;

        private readonly Random rand = new();

        public UserRepositoryTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get all users test
        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            //Arrange
            var users = new List<RegisterUser>()
            {
                RandomUser(), RandomUser(), RandomUser()
            };

            context.RegisterUsers.AddRange(users);
            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            IEnumerable<RegisterUser> result = await userRepoStub.Get();

            //Assert
            Assert.Equal(users.Count, result.Count());

        }

        //Get specific user test
        [Fact]
        public async Task GetUser_WithExistingUser_ReturnUser()
        {
            //Arrange
            var user = RandomUser();

            context.RegisterUsers.Add(user);
            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            RegisterUser result = await userRepoStub.GetSpecific(user.Id);

            //Assert
            Assert.IsType<RegisterUser>(result);
            Assert.Equal(user, result);
        }

        //Get User Id Test
        [Fact]
        public async Task GetUserId_WithExistingUser_ReturnsUserId()
        {
            //Arrange
            var user = RandomUser();

            context.RegisterUsers.Add(user);
            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            int result = await userRepoStub.GetUserId(user.UserName);

            //Assert
            Assert.IsType<int>(result);
            Assert.Equal(user.Id, result);
        }

        //Get User Id Test
        [Fact]
        public async Task GetUserId_WithoutExistingUser_ReturnsValueZero()
        {
            //Arrange
            var user = RandomUser();
            var unExistUserName = Guid.NewGuid().ToString();

            context.RegisterUsers.Add(user);
            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            int result = await userRepoStub.GetUserId(unExistUserName);

            //Assert
            Assert.IsType<int>(result);
            Assert.Equal(0, result);
        }

        //Get User by username Test
        [Fact]
        public async Task GetUser_WithExistingUser_ReturnsExistingUser()
        {
            //Arrange
            var user = RandomUser();

            context.RegisterUsers.Add(user);
            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            RegisterUser result = await userRepoStub.GetUserByName(user.UserName);

            //Assert
            Assert.IsType<RegisterUser>(result);
            Assert.Equal(user, result);
        }

        //Create User Test
        [Fact]
        public async Task AddUser_WithUserToCreate_ReturnsAddedUser()
        {
            //Arrange
            var userRepoStub = new UserRepository(context);
            var newUser = new RegisterUser()
            {
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            //Act
            var res = await userRepoStub.Add(newUser);

            //Assert
            Assert.IsType<RegisterUser>(res);
            Assert.Equal(res.UserName, newUser.UserName);
        }

        //Update existing user Test
        [Fact]
        public async Task UpdateUser_WithExistingUser_ReturnsUpdatedUser()
        {
            //Arrange
            var userRepoStub = new UserRepository(context);

            var existingUser = RandomUser();
            var updateUser = new RegisterUser()
            {
                Id = existingUser.Id,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.RegisterUsers.Add(existingUser);
            await context.SaveChangesAsync();

            //Act
            var res = await userRepoStub.Update(updateUser);

            //Assert
            Assert.IsType<RegisterUser>(res);
            Assert.Equal(res.UserName, updateUser.UserName);
        }

        //Update not existing user Test
        [Fact]
        public async Task UpdateUser_WithoutExistingUser_ReturnsNullValue()
        {
            //Arrange
            var userRepoStub = new UserRepository(context);

            var existingUser = RandomUser();
            var updateUser = new RegisterUser()
            {
                Id = rand.Next(100),
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.RegisterUsers.Add(existingUser);
            await context.SaveChangesAsync();

            //Act
            var res = await userRepoStub.Update(updateUser);

            //Assert
            Assert.Null(res);
        }

        //Delete category test
        [Fact]
        public async Task DeleteUser_WithExistingUser_ReutrnsNull()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();

            context.RegisterUsers.Add(existingUser);

            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            await userRepoStub.Delete(existingUser.Id);

            var result = await userRepoStub.GetSpecific(existingUser.Id);

            //Assert
            Assert.Null(result);
        }

        //User Exist Test
        [Fact]
        public async Task CheckUser_WithExistinUser_ReturnsTrue()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();

            context.RegisterUsers.Add(existingUser);

            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            var res = context.RegisterUsers.Count(e => e.Id == existingUser.Id) > 0;

            //Assert
            Assert.True(res);

        }

        //User not Exist Teat
        [Fact]
        public async Task CheckUser_WithoutExistinUser_ReturnsFalse()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();

            context.RegisterUsers.Add(existingUser);

            await context.SaveChangesAsync();

            var userRepoStub = new UserRepository(context);

            //Act
            var res = context.RegisterUsers.Count(e => e.Id == rand.Next(100)) > 0;

            //Assert
            Assert.False(res);

        }

        private RegisterUser RandomUser()
        {
            return new()
            {
                Id = rand.Next(100),
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };
        }
    }
}
