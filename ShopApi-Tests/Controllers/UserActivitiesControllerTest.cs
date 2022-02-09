using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using ShopApi.Controllers;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShopApi_Tests.Controllers
{
    public class UserActivitiesControllerTest
    {
        private readonly CategoryApiContext context;

        private readonly Random rand = new();

        public UserActivitiesControllerTest()
        {
            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );

            context = new CategoryApiContext(dbOptions.Options);
        }

        //Get UserActivities Test
        [Fact]
        public async Task GetUserActivities_WithWxistingData_ReturnsUserActivities()
        {
            //Arrange
            var userActivities = new List<UserActivity>()
            {
                RandomUserActivity(), RandomUserActivity()
            };

            context.UserActivity.AddRange(userActivities);
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            ActionResult<IEnumerable<UserActivity>> result = await controller.GetUserActivity();

            //Assert
            Assert.IsType<ActionResult<IEnumerable<UserActivity>>>(result);
            Assert.Equal(userActivities, result.Value.ToList());

        }

        //Get specific UserActivity Test
        [Fact]
        public async Task GetUserActivity_WithWxistingData_ReturnsUserActivity()
        {
            //Arrange
            var userActivities = RandomUserActivity();

            context.UserActivity.Add(userActivities);
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            ActionResult<UserActivity> result = await controller.GetUserActivity( userActivities.Id );

            //Assert
            Assert.IsType<ActionResult<UserActivity>>(result);
            Assert.Equal(result.Value.UserId, userActivities.UserId);

        }

        //Get specific UserActivity Test
        [Fact]
        public async Task GetUserActivity_WithoutWxistingData_ReturnsNotFound()
        {
            //Arrange
            var userActivities = RandomUserActivity();

            context.UserActivity.Add(userActivities);
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            ActionResult<UserActivity> result = await controller.GetUserActivity(rand.Next(100));

            //Assert
            Assert.IsType<ActionResult<UserActivity>>(result);
            Assert.Null(result.Value);
        }

        //Create user activity test
        [Fact]
        public async Task CreateUserActivity_ReturnsCreatedUserActivity()
        {
            //Arrange
            UserActivity userActivities = new()
            {
                UserId = rand.Next(100),
                Username = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Method = Guid.NewGuid().ToString()
            };

            var controller = new UserActivitiesController(context);

            //Act
            ActionResult<UserActivity> result = await controller.PostUserActivity(userActivities);

            //Assert
            Assert.IsType<ActionResult<UserActivity>>(result);
            Assert.NotNull(result.Result);
        }

        //Update user activity Test
        [Fact]
        public async Task UpdateUserActivity_WithoutExistingUserActivity_ReturnNotFound()
        {
            //Arrange
            var userActivities = RandomUserActivity();

            context.UserActivity.Add(RandomUserActivity());
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            var result = await controller.PutUserActivity(userActivities.Id, userActivities);

            //Assert
            Assert.IsType<NoContentResult>(result);

        }

        //Update user activity Test
        [Fact]
        public async Task UpdateUserActivity_WithoutExistingUserActivity_ReturnsBadRequest()
        {
            //Arrange
            var userActivities = RandomUserActivity();

            UserActivity updateData = new()
            {
                Id = userActivities.Id,
                UserId = rand.Next(100),
                Username = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Method = Guid.NewGuid().ToString()
            };

            context.UserActivity.Add(userActivities);
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            var result = await controller.PutUserActivity(rand.Next(100), updateData);

            //Assert
            Assert.IsType<BadRequestResult>(result);

        }

        //Delete user activity Test
        [Fact]
        public async Task DelteUserActivity_WithExistingUerActivity_ReturnsNoContent()
        {
            //Arrange
            var userActivities = RandomUserActivity();

            context.UserActivity.Add(userActivities);
            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            var result = await controller.DeleteUserActivity(userActivities.Id);

            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<NoContentResult>(result);

        }

        //Category Exist Test
        [Fact]
        public async Task CheckUserActivity_WithoutExistingUserActivity_ReturnTrue()
        {
            //Arrange
            UserActivity existingUserActivity = RandomUserActivity();

            context.UserActivity.Add(existingUserActivity);

            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            var res = controller.UserActivityExists(existingUserActivity.Id);

            //Assert
            Assert.True(res);

        }

        //Category Exist Test
        [Fact]
        public async Task CheckUserActivity_WithoutExistingUserActivity_ReturnFalse()
        {
            //Arrange
            UserActivity existingUserActivity = RandomUserActivity();

            context.UserActivity.Add(existingUserActivity);

            await context.SaveChangesAsync();

            var controller = new UserActivitiesController(context);

            //Act
            var res = controller.UserActivityExists(rand.Next(100));

            //Assert
            Assert.False(res);

        }

        private UserActivity RandomUserActivity()
        {
            return new()
            {
                Id = rand.Next(100),
                UserId = rand.Next(100),
                Username = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Method = Guid.NewGuid().ToString()
            };
        }
    }
}
