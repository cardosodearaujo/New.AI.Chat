using FluentAssertions;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class AuthenticationLogsControllerTests
    {
        [Fact]
        public async Task GetAll_WithValidLogs_ReturnsOkWithLogsList()
        {
            // Arrange
            var mockService = new Mock<IGetAuthenticationLogsService>();
            var logs = new List<AuthenticationLogResponseDTO>
            {
                new AuthenticationLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Username = "admin",
                    LoginDateTime = DateTime.UtcNow,
                    TokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    IsSuccessful = true
                },
                new AuthenticationLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Username = "user",
                    LoginDateTime = DateTime.UtcNow.AddMinutes(-5),
                    TokenExpiresAt = DateTime.UtcNow.AddMinutes(55),
                    IsSuccessful = false
                }
            };

            var expected = new GetAuthenticationLogsResponseDTO
            {
                Logs = logs,
                TotalRecords = logs.Count
            };

            mockService.Setup(s => s.Process(It.IsAny<object>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);
            mockService.Setup(s => s.HasErrors()).Returns(false);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<object, GetAuthenticationLogsResponseDTO>>>();
            var controller = new AuthenticationLogsController(logger.Object, mockService.Object);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
            var ok = actionResult as Microsoft.AspNetCore.Mvc.OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAll_WithError_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<IGetAuthenticationLogsService>();
            var errors = new List<string> { "Erro ao buscar logs de autenticação" };

            mockService.Setup(s => s.Process(It.IsAny<object>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(true);
            mockService.Setup(s => s.Messages).Returns(errors);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<object, GetAuthenticationLogsResponseDTO>>>();
            var controller = new AuthenticationLogsController(logger.Object, mockService.Object);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAll_WithEmptyLogsList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var mockService = new Mock<IGetAuthenticationLogsService>();
            var expected = new GetAuthenticationLogsResponseDTO
            {
                Logs = new List<AuthenticationLogResponseDTO>(),
                TotalRecords = 0
            };

            mockService.Setup(s => s.Process(It.IsAny<object>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);
            mockService.Setup(s => s.HasErrors()).Returns(false);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<object, GetAuthenticationLogsResponseDTO>>>();
            var controller = new AuthenticationLogsController(logger.Object, mockService.Object);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
            var ok = actionResult as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var data = ok!.Value as GetAuthenticationLogsResponseDTO;
            data.Should().NotBeNull();
            data!.Logs.Should().BeEmpty();
            data.TotalRecords.Should().Be(0);
        }

        [Fact]
        public async Task GetAll_WithSuccessfulAndFailedLogins_ReturnsCorrectFlags()
        {
            // Arrange
            var mockService = new Mock<IGetAuthenticationLogsService>();
            var successLog = new AuthenticationLogResponseDTO
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Username = "admin",
                LoginDateTime = DateTime.UtcNow,
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
                IsSuccessful = true
            };

            var failedLog = new AuthenticationLogResponseDTO
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty,
                Username = "invalid",
                LoginDateTime = DateTime.UtcNow.AddMinutes(-1),
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(59),
                IsSuccessful = false
            };

            var expected = new GetAuthenticationLogsResponseDTO
            {
                Logs = new List<AuthenticationLogResponseDTO> { successLog, failedLog },
                TotalRecords = 2
            };

            mockService.Setup(s => s.Process(It.IsAny<object>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Data).Returns(expected);
            mockService.Setup(s => s.HasErrors()).Returns(false);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<object, GetAuthenticationLogsResponseDTO>>>();
            var controller = new AuthenticationLogsController(logger.Object, mockService.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            var actionResult = await controller.GetAll();
            var okResult = actionResult as Microsoft.AspNetCore.Mvc.OkObjectResult;
            okResult.Should().NotBeNull();
            var data = okResult!.Value as GetAuthenticationLogsResponseDTO;
            data.Should().NotBeNull();
            data!.Logs[0].IsSuccessful.Should().BeTrue();
            data.Logs[1].IsSuccessful.Should().BeFalse();
        }
    }
}
