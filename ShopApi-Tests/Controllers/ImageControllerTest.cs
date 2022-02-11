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
    public class ImageControllerTest
    {
        private readonly Mock<IImageRepository> imageRepositoryStub = new();
        private readonly Mock<IJwtUtils> jwtUtils = new();

        private readonly Random rand = new();

        //Get specific Image test
        [Fact]
        public async Task GetImage_WithUnexistingImage_ReturnsNotFound()
        {
            //Arrange
            imageRepositoryStub.Setup(repo => repo.DisplayImage(It.IsAny<string>()))
                .Returns((byte[])null);

            var controller = new ImagesController(imageRepositoryStub.Object);

            //Act
            var result = await controller.GetImage(It.IsAny<string>());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //Get specific Image test
        [Fact]
        public async Task GetImage_WithExistingImage_ReturnsExpectedImage()
        {
            //Arrange
            var expectedImage = RandomImage();

            imageRepositoryStub.Setup(repo => repo.DisplayImage(It.IsAny<string>()))
                .Returns((expectedImage.DataFiles));

            var controller = new ImagesController(imageRepositoryStub.Object);

            //Act
            var result = await controller.GetImage(Guid.NewGuid().ToString());
            
            //Assert
            Assert.IsType<ActionResult<byte[]>>(result);
            /*Assert.IsType<Item>(result.Value);
            var dto = (result as ActionResult<Item>).Value;
            
            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);*/
        }

        //Create Image test
        [Fact]
        public async Task CreateImage_WithImageToCreate_ReturnsCreatedImage()
        {
            //Arrange
            var controller = new ImagesController(imageRepositoryStub.Object);
            
            //Act
            var result = await controller.PostImage(It.IsAny<IFormFile>());

            //Assert
            Assert.IsType<ActionResult<Image>>(result);
        }

        //Create Image test
        [Fact]
        public async Task CreateImage_WithoutImageToCreate_ReturnsCreatedImage()
        {
            //Arrange
            var controller = new ImagesController(imageRepositoryStub.Object);

            //Act
            var result = await controller.PostImage(null);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        private Image RandomImage()
        {
            return new()
            {
                Id = rand.Next(100),
                Name = Guid.NewGuid().ToString(),
                FileType = Guid.NewGuid().ToString(),
                DataFiles = It.IsAny<byte[]>()
            };
        }
    }
}
