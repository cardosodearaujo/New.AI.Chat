using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.Exceptions;

namespace New.AI.Chat.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();
        int status = StatusCodes.Status500InternalServerError;
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException vex:
                status = StatusCodes.Status400BadRequest;
                var validation = new ValidationProblemDetails();
                validation.Title = "Erros de validação";
                validation.Extensions["messages"] = vex.Errors ?? new List<string> { vex.Message } as object;
                problem = validation;
                break;

            case NotFoundException nf:
                status = StatusCodes.Status404NotFound;
                problem = new ProblemDetails
                {
                    Title = nf.Message,
                    Status = status
                };
                break;

            case UnauthorizedException ua:
                status = StatusCodes.Status401Unauthorized;
                problem = new ProblemDetails
                {
                    Title = ua.Message,
                    Status = status
                };
                break;

            case OperationCanceledException:
                status = 499; // client closed request
                problem = new ProblemDetails { Title = "Requisição cancelada", Status = status };
                break;

            default:
                problem = new ProblemDetails { Title = "Ocorreu um erro inesperado.", Status = status };
                break;
        }

        problem.Extensions["correlationId"] = correlationId;

        if (_env.IsDevelopment())
        {
            problem.Detail = exception.ToString();
        }

        _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
