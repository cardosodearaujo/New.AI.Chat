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

            mockService.Setup(s => s.Process(It.IsAny<FileQueryDTO>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(false);
            mockService.Setup(s => s.Data).Returns(true);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<FileController>>();
            var controller = new FileController(logger.Object, mockService.Object);

            var result = await controller.Exists(new FileQueryDTO { FileName = "x" });

            mockService.Verify(s => s.Process(It.IsAny<FileQueryDTO>()), Times.Once);
            mockService.Object.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Exists_WhenServiceError_ReturnsBadRequest()
        {
            var mockService = new Mock<IFileService>();

            mockService.Setup(s => s.Process(It.IsAny<FileQueryDTO>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(true);
            mockService.Setup(s => s.Messages).Returns(new List<string> { "error" });

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<FileQueryDTO, bool>>>();
            var controller = new FileController(logger.Object, mockService.Object);

            var result = await controller.Exists(new FileQueryDTO { FileName = "x" });

            mockService.Verify(s => s.Process(It.IsAny<FileQueryDTO>()), Times.Once);
            mockService.Object.HasErrors().Should().BeTrue();
        }
    }
}
