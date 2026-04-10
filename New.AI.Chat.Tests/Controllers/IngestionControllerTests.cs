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

            mockService.Setup(s => s.Process(It.IsAny<IngestionDTO>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(false);
            mockService.Setup(s => s.Data).Returns(response);

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<IngestionController>>();
            var controller = new IngestionController(logger.Object, mockService.Object);

            var result = await controller.Ingestion(new IngestionDTO { IngestionFiles = new List<IngestionFileDTO>() });

            mockService.Verify(s => s.Process(It.IsAny<IngestionDTO>()), Times.Once);
            mockService.Object.Data.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Ingestion_WhenInvalid_ReturnsBadRequest()
        {
            var mockService = new Mock<IIngestionService>();

            mockService.Setup(s => s.Process(It.IsAny<IngestionDTO>())).Returns(Task.CompletedTask);
            mockService.Setup(s => s.HasErrors()).Returns(true);
            mockService.Setup(s => s.Messages).Returns(new List<string> { "error" });

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<DefaultController<IngestionDTO, IngestionResponseDTO>>>();
            var controller = new IngestionController(logger.Object, mockService.Object);

            var result = await controller.Ingestion(new IngestionDTO { IngestionFiles = new List<IngestionFileDTO>() });

            mockService.Verify(s => s.Process(It.IsAny<IngestionDTO>()), Times.Once);
            mockService.Object.HasErrors().Should().BeTrue();
        }
    }
}
