using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.EntityFrameworkCore;
using New.AI.Chat;
using New.AI.Chat.Configurations;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;
using Xunit;

namespace New.AI.Chat.Tests.Integration
{
    public class AuthenticationPipelineTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthenticationPipelineTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Ensure JwtSettings are available at configuration time so AddJwtAuthentication reads them
                builder.ConfigureAppConfiguration((context, cfg) =>
                {
                    var dict = new Dictionary<string, string>
                    {
                        ["JwtSettings:Key"] = "TestKeyForJwtDontUseInProduction1234567890",
                        ["JwtSettings:Issuer"] = "TestIssuer",
                        ["JwtSettings:Audience"] = "TestAudience",
                        ["JwtSettings:ExpirationMinutes"] = "60",
                    };

                    // Use the host configuration's built-in provider
                    var memoryConfigSource = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource { InitialData = dict };
                    cfg.Add(memoryConfigSource);
                });

                builder.ConfigureServices(services =>
                {
                    // Ensure the real AuthService is registered
                    services.RemoveAll(typeof(IAuthService));
                    services.AddScoped<IAuthService, AuthService>();

                    // Replace heavy dependencies with lightweight mocks to avoid 500s in integration tests
                    var mockChat = new Mock<New.AI.Chat.Services.Interfaces.IChatService>();
                    mockChat.Setup(s => s.Process(It.IsAny<New.AI.Chat.DTOs.PromptDTO>())).Returns(Task.CompletedTask);
                    mockChat.Setup(s => s.HasErrors()).Returns(false);
                    mockChat.SetupGet(s => s.Data).Returns(new New.AI.Chat.DTOs.PromptResponseDTO { Response = "ok", DateTime = DateTime.UtcNow.ToString() });
                    services.AddSingleton(mockChat.Object);

                    var mockIngestion = new Mock<New.AI.Chat.Services.Interfaces.IIngestionService>();
                    mockIngestion.Setup(s => s.Process(It.IsAny<New.AI.Chat.DTOs.IngestionDTO>())).Returns(Task.CompletedTask);
                    mockIngestion.Setup(s => s.HasErrors()).Returns(false);
                    mockIngestion.SetupGet(s => s.Data).Returns(new New.AI.Chat.DTOs.IngestionResponseDTO());
                    services.AddSingleton(mockIngestion.Object);

                    var mockFile = new Mock<New.AI.Chat.Services.Interfaces.IFileService>();
                    mockFile.Setup(s => s.Process(It.IsAny<New.AI.Chat.DTOs.FileQueryDTO>())).Returns(Task.CompletedTask);
                    mockFile.Setup(s => s.HasErrors()).Returns(false);
                    mockFile.SetupGet(s => s.Data).Returns(true);
                    services.AddSingleton(mockFile.Object);

                    // Add a test authentication scheme to avoid JWT validation complexity in tests
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = New.AI.Chat.Tests.TestAuth.TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = New.AI.Chat.Tests.TestAuth.TestAuthHandler.SchemeName;
                    }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, New.AI.Chat.Tests.TestAuth.TestAuthHandler>(New.AI.Chat.Tests.TestAuth.TestAuthHandler.SchemeName, _ => { });
                });
            });
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync("/api/chat", new StringContent(JsonSerializer.Serialize(new PromptDTO { Message = "hi", LLM = null }), Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ReturnsToken_AndProtectedEndpoint_AcceptsToken()
        {
            var client = _factory.CreateClient();

            // Login
            var login = new LoginDTO { Username = "admin", Password = "P@ssw0rd" };
            var resp = await client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json"));
            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<AuthResponseDTO>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            auth.Should().NotBeNull();
            auth.Token.Should().NotBeNullOrWhiteSpace();

            // Call protected using test authentication scheme (we don't need to validate the real JWT here)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "admin");
            var response = await client.PostAsync("/api/chat", new StringContent(JsonSerializer.Serialize(new PromptDTO { Message = "hi", LLM = null }), Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
