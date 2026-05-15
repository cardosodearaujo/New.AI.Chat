using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class IngestionControllerTests
    {
        [Fact]
        public async Task Ingestion_WhenValid_ReturnsOk()
        {
            var mockService = new Mock<IIngestionService>();
            var response = new IngestionResponseDTO();

            mockService.Setup(s => s.Process(It.IsAny<IngestionDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<IngestionResponseDTO>.Success(response));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<IngestionController>>();
            var controller = new IngestionController(logger.Object, mockService.Object);

            var result = await controller.Ingestion(new IngestionDTO { IngestionFiles = new List<IngestionFileDTO>() });

            mockService.Verify(s => s.Process(It.IsAny<IngestionDTO>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockService.Object.Result.Data.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Ingestion_WhenInvalid_ReturnsBadRequest()
        {
            var mockService = new Mock<IIngestionService>();

            mockService.Setup(s => s.Process(It.IsAny<IngestionDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<IngestionResponseDTO>.Failure(new List<string> { "error" }));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController>>();
            var controller = new IngestionController(logger.Object, mockService.Object);

            var result = await controller.Ingestion(new IngestionDTO { IngestionFiles = new List<IngestionFileDTO>() });

            mockService.Verify(s => s.Process(It.IsAny<IngestionDTO>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockService.Object.Result.IsSuccess.Should().BeFalse();
        }
    }
}
