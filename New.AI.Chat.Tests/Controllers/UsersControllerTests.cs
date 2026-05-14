using FluentAssertions;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetAll_WithValidUsers_ReturnsOkWithUsersList()
        {
            // Arrange
            var mockService = new Mock<New.AI.Chat.Services.Interfaces.IGetUsersService>();
            var users = new List<UserResponseDTO>
            {
                new UserResponseDTO
                {
                    Id = Guid.NewGuid(),
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    Username = "admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new UserResponseDTO
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test User",
                    Email = "user@example.com",
                    Username = "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var expected = new GetUsersResponseDTO { Users = users };

            mockService.Setup(s => s.Process(null)).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<New.AI.Chat.Controllers.UsersController>>();
            var controller = new UsersController(
                logger.Object,
                mockService.Object,
                Mock.Of<New.AI.Chat.Services.Interfaces.IGetUserByIdService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.ICreateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IUpdateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IDeleteUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IChangeUserPasswordService>());

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
            var ok = actionResult as Microsoft.AspNetCore.Mvc.OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAll_WithError_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<New.AI.Chat.Services.Interfaces.IGetUsersService>();
            mockService.Setup(s => s.Process(null)).Throws(new InvalidOperationException("Erro ao buscar usuários"));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<New.AI.Chat.Controllers.UsersController>>();
            var controller = new UsersController(
                logger.Object,
                mockService.Object,
                Mock.Of<New.AI.Chat.Services.Interfaces.IGetUserByIdService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.ICreateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IUpdateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IDeleteUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IChangeUserPasswordService>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.GetAll());
        }

        [Fact]
        public async Task GetAll_WithEmptyUserList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var mockService = new Mock<New.AI.Chat.Services.Interfaces.IGetUsersService>();
            var expected = new GetUsersResponseDTO { Users = new List<UserResponseDTO>() };

            mockService.Setup(s => s.Process(null)).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<New.AI.Chat.Controllers.UsersController>>();
            var controller = new UsersController(
                logger.Object,
                mockService.Object,
                Mock.Of<New.AI.Chat.Services.Interfaces.IGetUserByIdService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.ICreateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IUpdateUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IDeleteUserService>(),
                Mock.Of<New.AI.Chat.Services.Interfaces.IChangeUserPasswordService>());

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
            var ok = actionResult as Microsoft.AspNetCore.Mvc.OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(expected);
        }
    }
}
