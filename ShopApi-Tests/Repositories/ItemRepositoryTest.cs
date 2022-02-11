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
    public class ItemRepositoryTest
    {
        private readonly CategoryApiContext context;
        private readonly Mock<IDistributedCache> cache = new();

        private readonly Random rand = new();

        public ItemRepositoryTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString() // Use GUID so every test will use a different db
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get all items
        [Fact]
        public async Task GetItems_WithExistingItems_ReturnsAllItems()
        {
            //Arrange
            var items = new List<Item>() {
                RandomItem(), RandomItem()
            };
            Category category = new()
            {
                Id = 1,
                Name = Guid.NewGuid().ToString(),
                Sorting = 0
            };

            context.Categories.Add(category);
            context.Items.AddRange(items);

            await context.SaveChangesAsync();

            var itemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            var result = await itemRepoStub.Get();

            //Assert
            Assert.Equal(items, result);

        }

        //Get specific item test
        [Fact]
        public async Task GetItem_Existing_ReturnsExistingItem()
        {
            //Arrange
            var expectedItem = RandomItem();
            Category category = new()
            {
                Id = 1,
                Name = Guid.NewGuid().ToString(),
                Sorting = 0
            };

            context.Categories.Add(category);
            context.Items.Add(expectedItem);

            await context.SaveChangesAsync();

            var itemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            Item result = await itemRepoStub.GetSpecific(expectedItem.Id);

            //Assert
            Assert.IsType<Item>(result);
            Assert.Equal(expectedItem, result);
        }

        //Get item by slug
        [Fact]
        public async Task GetItem_WithExistingItem_ReturnsItem()
        {
            //Arrange
            var expectedItem = RandomItem();
            Category category = new()
            {
                Id = 1,
                Name = Guid.NewGuid().ToString(),
                Sorting = 0
            };

            context.Categories.Add(category);
            context.Items.Add(expectedItem);

            await context.SaveChangesAsync();

            var itemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            Item result = await itemRepoStub.GetItemBySlug(expectedItem.Name);

            //Assert
            Assert.IsType<Item>(result);
            Assert.Equal(expectedItem, result);
        }

        //Add Item test
        [Fact]
        public async Task AddItem_WitItemToCreate_ReturnsCreatedItem()
        {
            //Arrange
            var itemRepoStub = new ItemRepository(context, cache.Object);
            var newItem = new Item()
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Price = rand.Next(),
                CategoryId = rand.Next(),
                ImageName = Guid.NewGuid().ToString()
            };

            //Act
            var res = await itemRepoStub.Add(newItem);

            //Assert
            Assert.IsType<Item>(res);

        }

        //Update Item Test
        [Fact]
        public async Task UpdateItem_WithExistingItem_ReturnsUpdatedItem()
        {
            //Arrange
            Item existingItem = RandomItem();
            
            context.Items.Add(existingItem);

            await context.SaveChangesAsync();

            var ItemId = existingItem.Id;
            var ItemToUpdate = new Item()
            {
                Id = ItemId,
                Name = Guid.NewGuid().ToString()
            };

            var ItemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            var result = await ItemRepoStub.Update(ItemToUpdate);

            //Assert
            Assert.IsType<Item>(result);

        }

        //Update not existing Item Test
        [Fact]
        public async Task UpdateItem_WithoutExistingItem_ReturnsNull()
        {
            //Arrange
            Item existingItem = RandomItem();

            context.Items.Add(existingItem);

            await context.SaveChangesAsync();


            var ItemToUpdate = new Item()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString()
            };

            var ItemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            var result = await ItemRepoStub.Update(ItemToUpdate);

            //Assert
            Assert.Null(result);
        }

        //Delete Item test
        [Fact]
        public async Task DeleteItem_WithExistingItem_ReutrnsDeletedItem()
        {
            //Arrange
            Item existingItem = RandomItem();

            context.Items.Add(existingItem);

            await context.SaveChangesAsync();

            var ItemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            await ItemRepoStub.Delete(existingItem.Id);

            var result = await ItemRepoStub.GetSpecific(existingItem.Id);

            //Assert
            Assert.Null(result);
        }

        //Item Exist Test
        [Fact]
        public async Task CheckItem_WithExistingItem_ReturnsBool()
        {
            //Arrange
            Item existingItem = RandomItem();

            context.Items.Add(existingItem);

            await context.SaveChangesAsync();

            var ItemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            var res = context.Items.Count(e => e.Id == existingItem.Id) > 0;

            //Assert
            Assert.True(res);

        }

        //Item Exist Test
        [Fact]
        public async Task CheckItem_WithoutExistingItem_ReturnsBool()
        {
            //Arrange
            Item existingItem = RandomItem();

            context.Items.Add(existingItem);

            await context.SaveChangesAsync();

            var ItemRepoStub = new ItemRepository(context, cache.Object);

            //Act
            var res = context.Items.Count(e => e.Id == rand.Next(100)) > 0;

            //Assert
            Assert.False(res);

        }


        private Item RandomItem()
        {
            return new()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Price = rand.Next(),
                CategoryId = 1,
                ImageName = Guid.NewGuid().ToString()
            };
        }

    }
}
