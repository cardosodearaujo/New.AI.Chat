using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ILogger<DefaultController> logger)
        {
            _logger = logger;
        }

        protected async Task<ActionResult<S>> Process<E, S>(IDefaultService<E, S> service, E entry, CancellationToken cancellationToken = default)
        {
            var correlationId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

            if (service == null)
            {
                _logger.LogWarning("[{CorrelationId}] Service instance is null", correlationId);
                var pd = new ValidationProblemDetails { Title = "Serviço inválido" };
                pd.Extensions["messages"] = new List<string> { "Serviço não fornecido" } as object;
                return BadRequest(pd);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await service.Process(entry, cancellationToken);

                // Prefer explicit Result<T> when available
                var result = service.Result;
                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("[{CorrelationId}] Saída 200 (OK): {Type}", correlationId, typeof(S).Name);
                        return Ok(result.Data);
                    }

                    if (result.IsNotFound)
                    {
                        _logger.LogInformation("[{CorrelationId}] Saída 404 (NotFound): {Type}", correlationId, typeof(S).Name);
                        var notFoundPd = new ProblemDetails { Title = "Não encontrado" };
                        notFoundPd.Extensions["messages"] = result.Errors ?? Enumerable.Empty<string>();
                        return NotFound(notFoundPd);
                    }

                    _logger.LogInformation("[{CorrelationId}] Saída 400 (BadRequest) - service returned Result failures", correlationId);
                    var validation = new ValidationProblemDetails { Title = "Erros de validação" };
                    validation.Extensions["messages"] = result.Errors ?? Enumerable.Empty<string>();
                    return BadRequest(validation);
                }

                // If Result is null, treat as internal server error (service should always set Result)
                _logger.LogError("[{CorrelationId}] Service did not set a Result; returning 500", correlationId);
                var pd = new ProblemDetails
                {
                    Title = "Ocorreu um erro ao processar a solicitação.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Serviço não forneceu resultado após processamento."
                };
                pd.Extensions["correlationId"] = correlationId;
                return StatusCode(StatusCodes.Status500InternalServerError, pd);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("[{CorrelationId}] Request cancelled", correlationId);
                var pd = new ProblemDetails { Title = "Request cancelled", Status = 499 };
                return StatusCode(499, pd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] Unexpected error while processing service {Service}", correlationId, service.GetType().FullName);
                var pd = new ProblemDetails
                {
                    Title = "Ocorreu um erro ao processar a solicitação.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Consulte os logs para mais detalhes."
                };
                pd.Extensions["correlationId"] = correlationId;
                return StatusCode(StatusCodes.Status500InternalServerError, pd);
            }
        }
    }
}
