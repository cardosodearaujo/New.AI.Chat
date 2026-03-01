using Microsoft.SemanticKernel;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class ChatService : IChatService
    {
        private readonly Kernel _kernel;

        public ChatService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<DefaultDTO<ResponseDTO>> SendMessage(PromptDTO prompt)
        {
            var response = new DefaultDTO<ResponseDTO>();

            try
            {
                if (prompt is null)
                {
                    response.AddError("Mensagem vazia!");
                }
                else
                {
                    if (string.IsNullOrEmpty(prompt.Message))
                    {
                        response.AddError("Mensagem vazia!");
                    }
                }

                if (!response.HasErrors())
                {
                    var result = await _kernel.InvokePromptAsync(prompt.Message);

                    if (result != null)
                    {
                        response.Data = new ResponseDTO { Response = result.ToString() };
                        response.Success = true;
                    }
                    else
                    {
                        response.AddError("Erro ao processar a mensagem.");
                    }
                }

            }
            catch(Exception ex)
            {
                response.AddError(ex.Message);
            }
            
            return response;
        }
    }
}
