using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;

namespace New.AI.Chat.Controllers
{
    public class DefaultController<R> : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public DefaultController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        protected async Task<Results<Ok<R>, BadRequest<IList<string>>>> Result(DefaultDTO<R> response)
        {
            if (response != null)
            {
                if (response.Success && response.Data != null)
                {
                    _logger.Log(LogLevel.Information, $"Saida 200 (OK): {response.Data}");
                    return TypedResults.Ok(response.Data);
                }
                else if (response.HasErrors())
                {
                    _logger.Log(LogLevel.Information, $"Saida 400 (BadRequest): {response.Data}");
                    return TypedResults.BadRequest(response?.ErrorMessages);
                }
            }

            _logger.Log(LogLevel.Information, $"Saida 400 (BadRequest): {response.Data}");
            return TypedResults.BadRequest((IList<string>)new List<string> { "Retorno inválido!" });  
        }
    }
}
