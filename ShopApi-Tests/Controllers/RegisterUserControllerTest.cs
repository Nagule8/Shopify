using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopApi.Controllers;
using ShopApi.Dtos;
using ShopApi.Interface;
using ShopApi.Models;
using System;
using System.Threading.Tasks;
using Xunit;


namespace ShopApi_Tests
{
    public class RegisterUserControllerTest
    {
        private readonly Mock<ICommonRepository<RegisterUser>> commonRepositoryStub = new();
        private readonly Mock<IUserRepository> userRepositoryStub = new();
        private readonly Mock<IJwtUtils> jwtUtils = new();

        private readonly Random rand = new();

        //Get specific user test
        [Fact]
        public async Task GetUser_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync((RegisterUser)null);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetRegisterUser(rand.Next(100));

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //Get specific user test
        [Fact]
        public async Task GetUser_WithExistingUser_ReturnsExpectedUser()
        {
            //Arrange
            var expectedUser = RandomUser();

            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>()))
                .ReturnsAsync(expectedUser);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetRegisterUser(rand.Next(100));

            //Assert
            Assert.IsType<ActionResult<RegisterUser>>(result);
            /*Assert.IsType<Item>(result.Value);
            var dto = (result as ActionResult<Item>).Value;

            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);*/
        }

        //Get all users test
        [Fact]
        public async Task GetCartItems_WithExistingCartItem_ReturnsAllCategory()
        {
            //Arrange
            var expectedItems = new[] { RandomUser(), RandomUser(), RandomUser() };

            commonRepositoryStub.Setup(repo => repo.Get()).ReturnsAsync(expectedItems);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);
            
            //Act
            var result = await controller.GetRegisterUsers();

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //Get user id test
        [Fact]
        public async Task GetUserId_WithExistingUser_ReturnsUserId()
        {
            //Arrange
            var userName = Guid.NewGuid().ToString();
            userRepositoryStub.Setup(repo => repo.GetUserId(userName)).ReturnsAsync(It.IsAny<int>());

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetUserId(userName);

            //Assert
            Assert.IsType<ActionResult<int>>(result);

        }

        //Get user id test
        [Fact]
        public async Task GetUserId_WithoutExistingUser_ReturnsNotFound()
        {
            //Arrange
            var userName = Guid.NewGuid().ToString();
            userRepositoryStub.Setup(repo => repo.GetUserId(userName)).ReturnsAsync(It.IsAny<int>());

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.GetUserId(Guid.NewGuid().ToString());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }

        //Login user test
        [Fact]
        public async Task LoginUser_WithExistingUser_ReturnsSuccessMessgae()
        {
            //Arrange
            LoginDto loginDto = new LoginDto()
            {
                Name = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };

            userRepositoryStub.Setup(repo => repo.GetUserByName(loginDto.Name)).ReturnsAsync(It.IsAny<RegisterUser>());
            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.Login(loginDto);

            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        //Login user test
        [Fact]
        public async Task LoginUser_WithoutExistingUser_ReturnsBadRequestObjectResult()
        {
            //Arrange
            LoginDto loginDto = new LoginDto()
            {
                Name = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };

            userRepositoryStub.Setup(repo => repo.GetUserByName(loginDto.Name)).ReturnsAsync((RegisterUser)null);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.Login(loginDto);

            //Assert
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        //Create users test
        [Fact]
        public async Task CreateUser_WithUserToCreate_ReturnsCreatedUser()
        {
            //Arrange
            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);
            
            //Act
            var result = await controller.PostRegisterUser(It.IsAny<RegisterUser>());
            
            //Assert
            Assert.IsType<ActionResult<RegisterUser>>(result);
        }

        //Create users test
        [Fact]
        public async Task CreateUser_WithoutUserToCreate_ReturnBadRequest()
        {
            //Arrange
            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.PostRegisterUser(null);

            //Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        //Create users test
        [Fact]
        public async Task CreateUser_WithoutUserToCreate_ReturnBadRequestModelState()
        {
            //Arrange
            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.PostRegisterUser(It.IsAny<RegisterUser>());

            //Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        //Update users test
        [Fact]
        public async Task UpdateUser_WithExistingUser_ReturnsUpdatedUser()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingUser);

            var userId = existingUser.Id;
            var userToUpdate = existingUser;

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutRegisterUser(userId, userToUpdate);
            //Assert
            Assert.IsType<ActionResult<RegisterUser>>(result);
        }

        //Update users test
        [Fact]
        public async Task UpdateUser_WithExistingUser_ReturnsBadRequestResult()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingUser);

            var userId = existingUser.Id;
            var userToUpdate = existingUser;

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutRegisterUser(rand.Next(100), userToUpdate);
            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        //Update users test
        [Fact]
        public async Task UpdateUser_WithExistingUser_ReturnsNotFoundResult()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync((RegisterUser)null);

            var userId = existingUser.Id;
            var userToUpdate = existingUser;

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);
            //Act
            var result = await controller.PutRegisterUser(userId, userToUpdate);
            //Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        //Delete users test
        [Fact]
        public async Task DeleteUser_WithExistingUser_ReturnsDeletedUser()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync(existingUser);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.DeleteRegisterUser(existingUser.Id);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //Delete users test
        [Fact]
        public async Task DeleteUser_WithoutExistingUser_ReturnsDeletedNotFoundResult()
        {
            //Arrange
            RegisterUser existingUser = RandomUser();
            commonRepositoryStub.Setup(repo => repo.GetSpecific(It.IsAny<int>())).ReturnsAsync((RegisterUser)null);

            var controller = new RegisterUsersController(commonRepositoryStub.Object, userRepositoryStub.Object, jwtUtils.Object);

            //Act
            var result = await controller.DeleteRegisterUser(existingUser.Id);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
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
