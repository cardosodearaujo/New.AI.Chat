using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Extensions;
using New.AI.Chat.Services.Interfaces.AIInterfaces;

namespace New.AI.Chat.Services.AIServices
{
    public class GeminiFlashLLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Você é um Arquiteto de Software Sénior e Especialista em ERP (.NET/C#, SQL, Delphi, Javascript e HTML).
            Sua missão é responder à pergunta do utilizador baseando-se ESTRITAMENTE no contexto técnico extraído do sistema, fornecido abaixo.

            REGRAS DE OURO DA ARQUITETURA:
            1. ZERO ALUCINAÇÃO: A sua resposta deve derivar exclusivamente do bloco <contexto>. Se a resposta não estiver lá, responda exatamente: 'As informações mapeadas neste contexto não contêm a resposta para esta solicitação.'
            2. EVIDÊNCIA EM CÓDIGO: Sempre que explicar uma regra de negócio, valide a sua explicação exibindo o trecho de código exato do contexto usando blocos Markdown (ex: ```csharp).
            3. RASTREABILIDADE: Inicie a sua resposta citando o nome do Módulo/Ficheiro onde encontrou a resposta.
            4. DIDÁTICA SÉNIOR: Explique o 'porquê' da lógica, e não apenas o 'o que'. Seja direto e técnico.

            <contexto>
            {0}
            </contexto>";

        public LLMEnum LLM => LLMEnum.GeminiFlash;

        public LLMParametersDTO Parameters
        {
            get =>
                new LLMParametersDTO
                {
                    TakeLowGranularitySemanticIDs = 3,
                    TakeLowGranularityWithHighGranularitySemanticIDs = 5,
                    TakeLowGranularityWithHighGranularityLexicalIDs = 5
                };
        }

        private readonly Kernel _kernel;

        public GeminiFlashLLMService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> BuildPromptResponse(string userPrompt, string systemPrompt = "")
        {
            var finalSystemPrompt = string.Format(DEFAULT_PROMPT, systemPrompt);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(finalSystemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.GeminiFlash.GetDescription())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
