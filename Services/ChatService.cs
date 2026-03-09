using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Pgvector.EntityFrameworkCore;
using System.Text;

namespace New.AI.Chat.Services
{
    public class ChatService : DefaultService<PromptDTO, PromptResponseDTO>, IChatService
    {
        private const string MASTER_PROMPT = @"
            Você é um assistente de desenvolvimento de software sênior.
            Responda à pergunta do utilizador utilizando APENAS o contexto de código fornecido abaixo.
            Se a resposta não estiver no contexto, diga 'Não encontrei a resposta nos arquivos fornecidos'.
            Não invente código ou regras de negócio que não estejam no contexto.

            CONTEXTO ENCONTRADO NO BANCO DE DADOS:
            {0}";

        private readonly AIDbContext _aiDbContext;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _vectorGenerator;
        private readonly IChatCompletionService _chatService;               

        public ChatService(
            Kernel kernel,
            AIDbContext aiDbContext) : base()
        {
            _aiDbContext = aiDbContext;
            _vectorGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();             
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
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
            var messageVector = await _vectorGenerator.GenerateAsync(new[] { prompt.Message });
            var searchVector = new Pgvector.Vector(messageVector.FirstOrDefault().Vector.ToArray());
            var relevantDocument = await _aiDbContext.KnowledgeDocumentDbSet
                                                     .OrderBy(F => F.Embedding.L2Distance(searchVector))
                                                     .Take(3)
                                                     .ToListAsync();

            var context = new StringBuilder();
            foreach (var document in relevantDocument)
            {
                context.AppendLine($"Arquivo: {document.Font}");
                context.AppendLine(document.ContentText);
                context.AppendLine($"");
            }

            var fullPrompt = string.Format(MASTER_PROMPT, context);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(fullPrompt);
            chatHistory.AddUserMessage(prompt.Message);

            var reponse = await _chatService.GetChatMessageContentAsync(chatHistory);

            if (reponse != null)
            {
                Data = new PromptResponseDTO
                {
                    Response = reponse.Content,
                    ReferenceFiles = relevantDocument.Select(F => F.Font).ToList(),
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
