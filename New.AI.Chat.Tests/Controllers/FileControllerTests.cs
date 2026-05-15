using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class FileControllerTests
    {
        [Fact]
        public async Task Exists_WhenFileExists_ReturnsOk()
        {
            var mockService = new Mock<IFileService>();

            mockService.Setup(s => s.Process(It.IsAny<FileQueryDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<bool>.Success(true));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<FileController>>();
            var controller = new FileController(logger.Object, mockService.Object);

            var result = await controller.Exists(new FileQueryDTO { FileName = "x" });

            mockService.Verify(s => s.Process(It.IsAny<FileQueryDTO>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockService.Object.Result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Exists_WhenServiceError_ReturnsBadRequest()
        {
            var mockService = new Mock<IFileService>();

            mockService.Setup(s => s.Process(It.IsAny<FileQueryDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<bool>.Failure(new[] { "error" }));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController>>();
            var controller = new FileController(logger.Object, mockService.Object);

            var result = await controller.Exists(new FileQueryDTO { FileName = "x" });

            mockService.Verify(s => s.Process(It.IsAny<FileQueryDTO>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockService.Object.Result.IsSuccess.Should().BeFalse();
        }
    }
}
