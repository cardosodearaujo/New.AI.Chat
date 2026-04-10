using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace New.AI.Chat.Tests.TestAuth;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Expect header: Authorization: Test <username>
        if (!Request.Headers.TryGetValue("Authorization", out var header))
        {
            return Task.FromResult(AuthenticateResult.Fail("No Authorization Header"));
        }

        var headerVal = header.ToString();
        if (string.IsNullOrWhiteSpace(headerVal))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

        // header format: "Test username" or just "Test"
        var parts = headerVal.Split(' ', 2);
        if (parts.Length == 0 || !string.Equals(parts[0], SchemeName, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Scheme"));

        var username = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1] : "test-user";

        var claims = new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.NameIdentifier, username) };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
