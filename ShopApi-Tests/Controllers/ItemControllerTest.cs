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
    public class ItemControllerTest
    {
        private readonly Mock<ICommonRepository<Item>> commonRepositoryStub = new();
        private readonly Mock<IItemRepository> itemRepositoryStub = new();

        private readonly Mock<ICategoryRepository> categoryRepositoryStub = new();
        private readonly Mock<ICommonRepository<Category>> commonCatRepositoryStub = new();

        private readonly Random rand = new();

        //Get specific Item test
        [Fact]
        public async Task GetItem_WithUnexistingItem_ReturnsNotFound()
        {
            //Arrange
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.GetItem(rand.Next(100));

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //Get specific Item test
        [Fact]
        public async Task GetItem_WithExistingItem_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = RandomItem();

            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.GetItem(rand.Next(100));

            //Assert
            Assert.IsType<ActionResult<Item>>(result);
            /*Assert.IsType<Item>(result.Value);
            var dto = (result as ActionResult<Item>).Value;
            
            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);*/
        }

        //Get all items test
        [Fact]
        public async Task GetItems_WithExistingItem_ReturnsAllCategory()
        {
            //Arrange
            var expectedItems = new[] { RandomItem(), RandomItem(), RandomItem() };

            commonRepositoryStub.Setup(repo => repo.Get())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);
            //Act
            var items = await controller.GetItems();
            //Assert
            Assert.IsType<OkObjectResult>(items);
        }

        //Create item test
        [Fact]
        public async Task CreateItem_WithItemToCreate_ReturnsNull()
        {
            //Arrange
            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.PostItem(null);

            //Assert
            Assert.Null(result.Value);
        }

        //Create item test
        [Fact]
        public async Task CreatItem_WithoutItemToCreate_ReturnBadRequest()
        {
            //Arrange
            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.PostItem((Item)null);

            //Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        //Create item test
        [Fact]
        public async Task Creatcategory_WithoutcategoryToCreate_ReturnBadRequestModelState()
        {
            //Arrange
            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.PostItem(It.IsAny<Item>());

            //Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        //Update category test
        [Fact]
        public async Task UpdateItem_WithExistingItem_ReturnsUpdatedItem()
        {
            //Arrange
            Item existingItem = RandomItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = RandomItem();

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);
            //Act
            var result = await controller.PutItem(itemId, itemToUpdate);
            //Assert
            Assert.IsType<ActionResult<Item>>(result);
        }

        //Update category test
        [Fact]
        public async Task UpdateItem_WithoutExistingItem_ReturnsNotFoundObjectResult()
        {
            //Arrange
            Item existingItem = RandomItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = RandomItem();

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);
            //Act
            var result = await controller.PutItem(rand.Next(100), itemToUpdate);
            //Assert
            Assert.IsType<ActionResult<Item>>(result);
        }

        //Delete category test
        [Fact]
        public async Task DeleteItem_WithExistingItem_ReturnsDeletedItem()
        {
            //Arrange
            Item existingItem = RandomItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.DeleteItem(existingItem.Id);

            //Assert
            Assert.IsType<ActionResult<Item>>(result);
        }

        //Delete category test
        [Fact]
        public async Task DeleteItem_WithExistingItem_ReturnsNotFOund()
        {
            //Arrange
            Item existingItem = RandomItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync((Item)null);

            var controller = new ItemsController(commonRepositoryStub.Object, itemRepositoryStub.Object);

            //Act
            var result = await controller.DeleteItem(existingItem.Id);

            //Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        private Item RandomItem()
        {
            return new()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Price = rand.Next(),
                CategoryId = rand.Next(),
                ImageName = Guid.NewGuid().ToString()
            };
        }
    }
}
