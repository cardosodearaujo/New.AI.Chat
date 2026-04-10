using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();
            var expected = new AuthResponseDTO { Token = "abc", ExpiresAt = DateTime.UtcNow.AddMinutes(60) };

            mockService.Setup(s => s.Process(It.IsAny<LoginDTO>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);
            mockService.Setup(s => s.HasErrors()).Returns(false);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<AuthController>>();
            var controller = new AuthController(logger.Object, mockService.Object);

            // Act
            var result = await controller.Login(new LoginDTO { Username = "admin", Password = "P@ssw0rd" });

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            mockService.Setup(s => s.Process(It.IsAny<LoginDTO>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(true);
            var messages = new List<string> { "Credenciais inválidas." };
            mockService.Setup(s => s.Messages).Returns(messages);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<AuthController>>();
            var controller = new AuthController(logger.Object, mockService.Object);

            // Act
            var result = await controller.Login(new LoginDTO { Username = "bad", Password = "bad" });

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized!.Value.Should().BeEquivalentTo(messages);
        }
    }
}
