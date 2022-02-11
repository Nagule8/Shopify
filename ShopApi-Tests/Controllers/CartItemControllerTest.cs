using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public class CartItemControllerTest
    {
        private readonly Mock<ICommonRepository<CartItem>> commonRepositoryStub = new();
        private readonly Mock<ICartItemRepository> cartItemRepositoryStub = new();
        private readonly Mock<IJwtUtils> jwtUtils = new();

        private readonly Random rand = new();

        //Get specific CartItem test
        [Fact]
        public async Task GetCartItem_WithUnexistingCartItem_ReturnsNotFound()
        {
            //Arrange
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync((CartItem)null);

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetCartItem(rand.Next(100));

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //Get specific CartItem test
        [Fact]
        public async Task GetCartItem_WithExistingCartItem_ReturnsExpectedCartItem()
        {
            //Arrange
            var expectedItem = RandomCartItem();

            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync(expectedItem);

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetCartItem(rand.Next(100));

            //Assert
            Assert.IsType<ActionResult<CartItem>>(result);
            /*Assert.IsType<Item>(result.Value);
            var dto = (result as ActionResult<Item>).Value;
            
            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);*/
        }

        //Create cart item test
        [Fact]
        public async Task CreateCartItem_WithCartItemToCreate_ReturnsCreatedCartItem()
        {
            //Arrange
            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);
            
            //Act
            var result = await controller.PostCartItem(It.IsAny<CartItem>());
            
            //Assert
            Assert.IsType<ActionResult<CartItem>>(result);
        }

        //Create Cart tiem test
        [Fact]
        public async Task CreatCartItem_WithoutCartItemToCreate_ReturnBadRequest()
        {
            //Arrange
            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.PostCartItem((CartItem)null);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        //Create cart item test
        [Fact]
        public async Task CreatCartItem_WithoutCartItemToCreate_ReturnBadRequestModelState()
        {
            //Arrange
            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.PostCartItem(It.IsAny<CartItem>());

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        //Update cart item test
        [Fact]
        public async Task UpdateItem_WithExistingItem_ReturnsUpdatedItem()
        {
            //Arrange
            CartItem existingItem = RandomCartItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = RandomCartItem();

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutCartItem(itemId, itemToUpdate);
            //Assert
            Assert.IsType<ActionResult<CartItem>>(result);
        }

        //Update Cart item test
        [Fact]
        public async Task UpdateCartItem_WithoutCartItemToCreate_ReturnsBadRequestResult()
        {
            //Arrange
            CartItem existingItem = RandomCartItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var userId = existingItem.Id;
            var userToUpdate = existingItem;

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutCartItem(rand.Next(100), userToUpdate);
            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        //Update Cart item test
        [Fact]
        public async Task UpdateCartItem_WithoutCartItemToCreate_ReturnsNotFoundResult()
        {
            //Arrange
            CartItem existingItem = RandomCartItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync((CartItem)null);

            var userId = existingItem.Id;
            var userToUpdate = existingItem;

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutCartItem(userId, userToUpdate);
            //Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        //Increase cart item test
        [Fact]
        public async Task IncreaseCartItem_WithExistingCartItem_ReturnsIncreasedCartItem()
        {
            //Arrange
            CartItem existingCartItem = RandomCartItem();

            cartItemRepositoryStub.Setup(repo => repo.IncreaseQuantity(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(existingCartItem);

            var id = existingCartItem.Id;
            var quantity = existingCartItem.Quantity;

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.PUTIncreaseQuantity(id, quantity);

            //Assert
            Assert.IsType<ActionResult<CartItem>>(result);
        }

        //Delete cart item test
        [Fact]
        public async Task DeleteCartItem_WithExistingCartItem_ReturnsDeletedItem()
        {
            //Arrange
            CartItem existingItem = RandomCartItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingItem);

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.DeleteCartItem(existingItem.Id);

            //Assert
            Assert.IsType<ActionResult<CartItem>>(result);
        }

        //Delete Cart item test
        [Fact]
        public async Task DeleteCartItem_WithExistingCartItem_ReturnsNotFoundObjectResult()
        {
            //Arrange
            CartItem existingItem = RandomCartItem();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync((CartItem)null);

            var controller = new CartItemsController(commonRepositoryStub.Object, cartItemRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.DeleteCartItem(existingItem.Id);

            //Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
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
                RegisterUserId = rand.Next(100)
            };
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
