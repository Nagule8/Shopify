using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShopApi_Tests.Repositories
{
    public class CartItemRepositoryTest
    {
        private readonly CategoryApiContext context;
        private readonly Mock<IDistributedCache> cache = new();

        private readonly Random rand = new();

        public CartItemRepositoryTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get cartitems Test
        [Fact]
        public async Task GetCartItem_WithExistingCartItems_ReturnsExistingItems_BasedUserId()
        {
            //Arrange
            var expectedCartItems = new List<CartItem>() { RandomCartItem(), RandomCartItem(), RandomCartItem() };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.AddRange(expectedCartItems);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.GetCartItems(10);

            //Assert
            Assert.Equal(expectedCartItems.Count, result.Count());

        }

        //Get cartitem by item id Test
        [Fact]
        public async Task GetCartItem_WithExistingCartItems_ReturnsExistingItems_BasedItemId()
        {
            //Arrange
            CartItem expectedCartItems = new()
            {
                Id = rand.Next(100),
                ItemId = 20,
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(expectedCartItems);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.GetCartItemByItemId(20);

            //Assert
            Assert.IsType<CartItem>(result);
            Assert.Equal(expectedCartItems.Id, result.Id);

        }

        //Get specific cartitem Test
        [Fact]
        public async Task GetCartItem_WithExistingCartItem_ReutrnsCartItem()
        {
            //Arrange
            var id = rand.Next(100);
            CartItem expectedCartItems = new()
            {
                Id = id,
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(expectedCartItems);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.GetSpecific(id);

            //Assert
            Assert.IsType<CartItem>(result);
            Assert.Equal(expectedCartItems.Id, result.Id);
        }

        //Add item to the cart test
        [Fact]
        public async Task AddItemToCart_WithExistingCart_ReutrnsItemAddedToCart()
        {
            //Arrange
            CartItem newCartItems = new()
            {
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.Add(newCartItems);

            //Assert
            Assert.Equal(newCartItems, result);
        }

        //Update item in the cart test
        [Fact]
        public async Task UpdateItemToCart_WithExistingCart_ReutrnsItemAddedToCart()
        {
            //Arrange
            CartItem existingCartItem = new CartItem()
            {
                Id = 30,
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            CartItem newCartItem = new()
            {
                Id = 30,
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(existingCartItem);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.Update(newCartItem);

            //Assert
            Assert.IsType<CartItem>(result);
            //Assert.Equal(newCartItem, result);
            Assert.Equal(existingCartItem, result);
        }

        //Update cart item test
        [Fact]
        public async Task UpdateItemToCart_WithoutExistingCart_ReutrnsNull()
        {
            //Arrange
            CartItem existingCartItem = new CartItem()
            {
                Id = 31,
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            CartItem newCartItems = new()
            {
                Id = 30,
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(existingCartItem);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.Update(newCartItems);

            //Assert
            Assert.Null(result);
        }

        //Increase Quantity int the cart item test
        [Fact]
        public async Task IncreaseQuantity_WithExistingCartItem_ReutrnsUpdatedCartItem()
        {
            //Arrange
            var Quantity = rand.Next(100);
            CartItem existingCartItem = new()
            {
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(existingCartItem);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            var result = await cartitemRepositoryStub.IncreaseQuantity(existingCartItem.Id, Quantity);

            //Assert
            Assert.Equal(Quantity, result.Quantity);
        }

        //Delete CAet item
        [Fact]
        public async Task DeleteCartItem_WithExistingCartItem_ReutrnNull()
        {
            //Arrange
            var Quantity = rand.Next(100);
            CartItem existingCartItem = new()
            {
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
            RegisterUser user = new()
            {
                Id = 10,
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = ShopApi.Entity.Role.User,
                Password = Guid.NewGuid().ToString()
            };

            context.CartItems.Add(existingCartItem);
            context.RegisterUsers.Add(user);

            await context.SaveChangesAsync();

            var cartitemRepositoryStub = new CartItemRepository(context, cache.Object);

            //Act
            await cartitemRepositoryStub.Delete(existingCartItem.Id);

            var result = await cartitemRepositoryStub.GetSpecific(existingCartItem.Id);

            //Assert
            Assert.Null(result);
        }

        //Cart item Exist Test
        [Fact]
        public async Task CheckCartItem_WithExistingCartItem_ReturnsTrue()
        {
            //Arrange
            CartItem existingCartItem = RandomCartItem();

            context.CartItems.Add(existingCartItem);

            await context.SaveChangesAsync();

            var cartItemRepoStub = new CartItemRepository(context, cache.Object);

            //Act
            var res = cartItemRepoStub.Exists(existingCartItem.Id);

            //Assert
            Assert.True(res);

        }

        //Cart item not Exist Test
        [Fact]
        public async Task CheckCartItem_WithExistingCartItem_ReturnsFalse()
        {
            //Arrange
            CartItem existingCartItem = RandomCartItem();

            context.CartItems.Add(existingCartItem);

            await context.SaveChangesAsync();

            var cartItemRepoStub = new CartItemRepository(context, cache.Object);

            //Act
            var res = cartItemRepoStub.Exists(rand.Next(100));

            //Assert
            Assert.False(res);

        }

        private CartItem RandomCartItem()
        {
            return new()
            {
                Id = rand.Next(100),
                ItemId = rand.Next(100),
                ItemName = Guid.NewGuid().ToString(),
                Quantity = rand.Next(),
                Price = rand.Next(),
                ImageName = Guid.NewGuid().ToString(),
                RegisterUserId = 10
            };
        }
    }
}
