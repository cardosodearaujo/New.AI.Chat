using Microsoft.AspNetCore.Http.HttpResults;
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

        protected async Task<IActionResult> Process<E,S>(IDefaultService<E, S> service, E entry)
        {
            try
            {
                if (service != null)
                {
                    await service.Process(entry);

                    if (!service.HasErrors())
                    {
                        _logger.Log(LogLevel.Information, $"Saída 200 (OK): {service.Data}");
                        return new OkObjectResult(service.Data);
                    }
                    else if (service.HasErrors())
                    {
                        _logger.Log(LogLevel.Information, $"Saída 400 (BadRequest): {service.Data}");
                        return new BadRequestObjectResult(service.Messages);
                    }
                }

                _logger.Log(LogLevel.Information, $"Saída 400 (BadRequest): {service.Data}");
                return new BadRequestObjectResult((IList<string>)new List<string> { "Retorno inválido!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Saída 400 (BadRequest): Ocorreu um erro inesperado: {ex.Message}");
                return new BadRequestObjectResult((IList<string>)new List<string> { "Ocorreu um erro ao processar a solicitação. Consulte os logs." });
            }
        }
    }
}
