

using Microsoft.AspNetCore.Mvc;
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
    public class CategoryRepositoryTest
    {
        private readonly CategoryApiContext context;
        private readonly Mock<IDistributedCache> cache = new();

        private readonly Random rand = new();

        public CategoryRepositoryTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get all Categories test
        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            //Arrange
            var categories = new List<Category>()
            {
                RandomCategory(), RandomCategory(), RandomCategory()
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            IEnumerable<Category> result = await categoryRepoStub.Get();

            //Assert
            Assert.Equal(categories.Count, result.Count());

        }

        //Get specific category test
        [Fact]
        public async Task GetCategory_Existing_ReturnsExistingCategory()
        {
            //Arrange
            var expectedCategory = RandomCategory();

            context.Categories.Add(expectedCategory);

            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            Category result = await categoryRepoStub.GetSpecific(expectedCategory.Id);

            //Assert
            Assert.IsType<Category>(result);
            Assert.Equal(expectedCategory, result);
        }

        //Get category by name
        [Fact]
        public async Task GetCategory_WithExistingCategory_ReturnsCategoryByName()
        {
            //Arrange
            var expectedCategory = RandomCategory();

            context.Categories.Add(expectedCategory);

            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            Category result = await categoryRepoStub.GetCategoryByName(expectedCategory.Name);

            //Assert
            Assert.IsType<Category>(result);
            Assert.Equal(expectedCategory, result);
        }

        //Add category test
        [Fact]
        public async Task AddCategory_WithCategoryToCreate_ReturnsCreatedCategoryegory()
        {
            //Arrange
            var categoryRepoStub = new CategoryRepository(context, cache.Object);
            var newCategory = new Category()
            {
                Name = Guid.NewGuid().ToString(),
                Sorting = 0
            };

            //Act
            var res = await categoryRepoStub.Add(newCategory);

            //Assert
            Assert.IsType<Category>(res);

        }

        //Update Category Test
        [Fact]
        public async Task UpdateCategory_WithExistingCategory_ReturnsUpdatedCategory()
        {
            //Arrange
            Category existingCategory = RandomCategory();

            context.Categories.Add(existingCategory);

            await context.SaveChangesAsync();

            var categoryId = existingCategory.Id;
            var categoryToUpdate = new Category()
            {
                Id = categoryId,
                Name = Guid.NewGuid().ToString()
            };

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            var result = await categoryRepoStub.Update(categoryToUpdate);

            //Assert
            Assert.IsType<Category>(result);

        }

        //Update not existing Category Test
        [Fact]
        public async Task UpdateCategory_WithoutExistingCategory_ReturnsNull()
        {
            //Arrange
            Category existingCategory = RandomCategory();

            context.Categories.Add(existingCategory);

            await context.SaveChangesAsync();

            var categoryToUpdate = new Category()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString()
            };

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            var result = await categoryRepoStub.Update(categoryToUpdate);

            //Assert
            Assert.Null(result);

        }

        //Delete category test
        [Fact]
        public async Task DeleteCategory_WithExistingCategory_ReutrnsDeletedCategory()
        {
            //Arrange
            Category existingCategory = RandomCategory();

            context.Categories.Add(existingCategory);

            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            await categoryRepoStub.Delete(existingCategory.Id);

            var result = await categoryRepoStub.GetSpecific(existingCategory.Id);

            //Assert
            Assert.Null(result);
        }

        //Category Exist Test
        [Fact]
        public async Task CheckCategory_WithExistingCategory_ReturnsBool()
        {
            //Arrange
            Category existingCategory = RandomCategory();

            context.Categories.Add(existingCategory);

            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            var res = context.Categories.Count(e => e.Id == existingCategory.Id) > 0;

            //Assert
            Assert.True(res);

        }

        //Category Exist Test
        [Fact]
        public async Task CheckCategory_WithoutExistingCategory_ReturnsBool()
        {
            //Arrange
            Category existingCategory = RandomCategory();

            context.Categories.Add(existingCategory);

            await context.SaveChangesAsync();

            var categoryRepoStub = new CategoryRepository(context, cache.Object);

            //Act
            var res = context.Categories.Count(e => e.Id == rand.Next(100)) > 0;

            //Assert
            Assert.False(res);

        }

        private Category RandomCategory()
        {
            return new()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString(),
                Sorting = 0
            };
        }
    }
}
