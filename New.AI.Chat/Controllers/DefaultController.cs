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

                await service.Process(entry);

                if (!service.HasErrors())
                {
                    _logger.LogInformation("[{CorrelationId}] Saída 200 (OK): {Type}", correlationId, typeof(S).Name);
                    return Ok(service.Data);
                }

                _logger.LogInformation("[{CorrelationId}] Saída 400 (BadRequest) - service returned messages", correlationId);
                var validation = new ValidationProblemDetails { Title = "Erros de validação" };
                validation.Extensions["messages"] = service.Messages;
                return BadRequest(validation);
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
