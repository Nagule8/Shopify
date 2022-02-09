using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Repositories;
using ShopApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShopApi_Tests.Repositories
{
    public class ImageRepositoryTest
    {
        private readonly CategoryApiContext context;
        private readonly Mock<IDistributedCache> cache = new();

        private readonly Random rand = new();

        public ImageRepositoryTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get Image Test
        [Fact]
        public async Task GetImage_WithExistingImage_ReturnsImage()
        {
            //Arrange
            var expectedImage = RandomImage();

            context.Images.Add(expectedImage);

            await context.SaveChangesAsync();

            var imageRepositoryStub = new ImageRepository(context, cache.Object);

            //Act
            var res = imageRepositoryStub.DisplayImage(expectedImage.Name);

            //Assert
            Assert.Equal(expectedImage.DataFiles, res);
        }

        //Upload image
        [Fact]
        public async Task UploadImage_WithIFormFile_ReturnsImageEntity()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            var imageRepositoryStub = new ImageRepository(context, cache.Object);

            //Act
            var res = await imageRepositoryStub.UploadImage(file);

            //Assert
            Assert.IsType<Image>(res);
        }

        //Image Exist Test
        [Fact]
        public async Task CheckImage_WithExistingCategory_ReturnsTrue()
        {
            //Arrange
            Image existingImage = RandomImage();

            context.Images.Add(existingImage);

            await context.SaveChangesAsync();

            var imageRepoStub = new ImageRepository(context, cache.Object);

            //Act
            var res = imageRepoStub.ImageExists(existingImage.Name);

            //Assert
            Assert.True(res);

        }

        //Image Exist Test
        [Fact]
        public async Task CheckImage_WithExistingCategory_ReturnsFalse()
        {
            //Arrange
            Image existingImage = RandomImage();

            context.Images.Add(existingImage);

            await context.SaveChangesAsync();

            var imageRepoStub = new ImageRepository(context, cache.Object);

            //Act
            var res = imageRepoStub.ImageExists(Guid.NewGuid().ToString());

            //Assert
            Assert.False(res);

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
