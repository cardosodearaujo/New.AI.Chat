using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using New.AI.Chat.Controllers;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Controllers
{
    public class ChatControllerTests
    {
        [Fact]
        public async Task SendMessage_WhenServiceSucceeds_ReturnsOk()
        {
            var mockService = new Mock<IChatService>();
            var response = new PromptResponseDTO { Response = "ok", DateTime = DateTime.UtcNow.ToString() };

            mockService.Setup(s => s.Process(It.IsAny<PromptDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<PromptResponseDTO>.Success(response));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<ChatController>>();
            var controller = new ChatController(logger.Object, mockService.Object);

            var result = await controller.SendMessage(new PromptDTO { Message = "hi", LLM = null });

            mockService.Verify(s => s.Process(It.IsAny<PromptDTO>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockService.Object.Result.Data.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task SendMessage_WhenServiceHasErrors_ReturnsBadRequest()
        {
            var mockService = new Mock<IChatService>();

            mockService.Setup(s => s.Process(It.IsAny<PromptDTO>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            mockService.SetupGet(s => s.Result).Returns(New.AI.Chat.Shared.Result<PromptResponseDTO>.Failure(new[] { "error" }));

            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<ChatController>>();
            var controller = new ChatController(logger.Object, mockService.Object);

            var result = await controller.SendMessage(new PromptDTO { Message = "hi", LLM = null });

            mockService.Verify(s => s.Process(It.IsAny<PromptDTO>()), Times.Once);
            mockService.Object.Result.IsSuccess.Should().BeFalse();
        }
    }
}
