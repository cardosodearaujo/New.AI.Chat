using Microsoft.SemanticKernel;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class ChatService : DefaultService<PromptDTO, ResponseDTO>, IChatService
    {
        private readonly Kernel _kernel;

        public ChatService(Kernel kernel) : base()
        {
            _kernel = kernel;
        }        

        protected override async Task Validate(PromptDTO prompt)
        {
            if (prompt is null)
            {
                AddError("Mensagem vazia!");
            }
            else
            {
                if (string.IsNullOrEmpty(prompt.Message))
                {
                    AddError("Mensagem vazia!");
                }
            }
        }

        protected override async Task DoProcess(PromptDTO prompt)
        {
            var result = await _kernel.InvokePromptAsync(prompt.Message);

            if (result != null)
            {
                Data = new ResponseDTO 
                {
                    Response = result.ToString(),
                    DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };
            }
            else
            {
                AddError("Erro ao processar a mensagem.");
            }
        }
    }
}
