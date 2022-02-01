using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopApi.Controllers;
using ShopApi.Interface;
using ShopApi.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ShopApi_Tests
{
    public class CategoriesControllerTest
    {
        private readonly Mock<ICommonRepository<Category>> commonRepositoryStub = new();
        private readonly Mock<ICategoryRepository> categoryRepositoryStub = new();

        private readonly Random rand = new(); 

        //Get specific category test
        [Fact]
        public async Task GetCategory_WithUnexistingItem_ReturnsNotFound()
        {
            //Arrange
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync((Category)null);

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);

            //Act
            var result = await controller.GetCategory(rand.Next(100));

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //Get specific category test
        [Fact]
        public async Task GetCategory_WithExistingItem_ReturnsExpectedCategory()
        {
            //Arrange
            var expectedCategory = RandomCategory();

            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync(expectedCategory);

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);

            //Act
            var result = await controller.GetCategory(rand.Next(100));

            //Assert
            Assert.IsType<ActionResult<Category>>(result);
            /*Assert.IsType<Category>(result.Value);
            var dto = (result as ActionResult<Category>).Value;
            
            Assert.Equal(expectedCategory.Id, dto.Id);
            Assert.Equal(expectedCategory.Name, dto.Name);*/
        }

        //Get all categories test
        [Fact]
        public async Task GetCategories_WithExistingItem_ReturnsAllCategory()
        {
            //Arrange
            var expectedCategories = new[] { RandomCategory(), RandomCategory(), RandomCategory() };

            commonRepositoryStub.Setup(repo => repo.Get())
                .ReturnsAsync(expectedCategories);

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);
            //Act
            var result = await controller.GetCategories();
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //Get mtching categories test
        [Fact]
        public async Task GetCategories_WithMatchingCategories_ReturnsMatchingCategories()
        {
            //Arrange
            var expectedCategories = new[] {
                new Category(){Name = "Medicine"},
                new Category(){Name = "Sports"},
                new Category(){Name = "Electronics"},
            };

            var nameToMatch = "Sports";

            commonRepositoryStub.Setup(repo => repo.Get())
                .ReturnsAsync(expectedCategories);

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);
            //Act
            var result = await controller.GetCategoryMatchingName(nameToMatch);
            //Assert
            result.Should().OnlyContain(
                item => item.Name == expectedCategories[1].Name || item.Name == expectedCategories[2].Name
            );
        }

        //Create category test
        [Fact]
        public async Task CreateCategory_WithCategoryToCreate_ReturnsCreatedCategory()
        {
            //Arrange
            var newCategory = new Category()
            {
                Name = Guid.NewGuid().ToString()
            };

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);
            //Act
            var result = await controller.PostCategory(It.IsAny<Category>());
            //Assert
            Assert.IsType<ActionResult<Category>>(result);
        }

        //Update category test
        [Fact]
        public async Task UpdateCategory_WithExistingCategory_ReturnsUpdatedCategory()
        {
            //Arrange
            Category existingCategory = RandomCategory();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingCategory);

            var categoryId = existingCategory.Id;
            var categoryToUpdate = new Category()
            {
                Name = Guid.NewGuid().ToString(),
            };

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);
            //Act
            var result = await controller.PutCategory(categoryId, categoryToUpdate);
            //Assert
            Assert.IsType<ActionResult<Category>>(result);
        }

        //Delete category test
        [Fact]
        public async Task DeleteCategory_WithExistingCategory_ReturnsDeletedCategory()
        {
            //Arrange
            Category existingCategory = RandomCategory();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingCategory);

            var controller = new CategoriesController(commonRepositoryStub.Object, categoryRepositoryStub.Object);

            //Act
            var result = await controller.DeleteCategory(existingCategory.Id);

            //Assert
            Assert.IsType<ActionResult<Category>>(result);
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
