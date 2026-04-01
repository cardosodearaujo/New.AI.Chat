using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Extensions;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class Quen7bLLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Você é um Arquiteto de Software Sênior e Especialista em ERP (.NET/C#, Delphi, typescript, javascript e Banco de Dados).
            Sua missão é responder à pergunta do utilizador baseando-se ESTRITAMENTE no contexto técnico extraído do sistema, fornecido abaixo.

            REGRAS DE OURO DA ARQUITETURA:
            1. ZERO ALUCINAÇÃO: Sua resposta deve derivar exclusivamente do bloco <contexto>. Se a resposta para a pergunta não estiver lá, você DEVE responder exatamente: 'As informações mapeadas neste contexto não contêm a resposta para esta solicitação.' Não deduza regras de negócio externas.
            2. EVIDÊNCIA EM CÓDIGO: Sempre que explicar uma regra de negócio, valide sua explicação exibindo o trecho de código exato do contexto usando blocos Markdown (ex: ```csharp, ```sql ou ```html).
            3. RASTREABILIDADE: Inicie sua resposta citando o nome do Módulo/Arquivo onde você encontrou a resposta (que estará no cabeçalho de cada bloco do contexto).
            4. DIDÁTICA SÊNIOR: Seja direto, técnico e profissional. Explique o 'porquê' o código faz aquilo, e não apenas 'o que' ele faz.

            <contexto>
            {0}
            </contexto>";

        public LLMEnum LLM => LLMEnum.Qwen7b;

        public LLMParametersDTO Parameters
        {
            get =>
                new LLMParametersDTO
                {
                    TakeLowGranularitySemanticIDs = 1,
                    TakeLowGranularityWithHighGranularitySemanticIDs = 2,
                    TakeLowGranularityWithHighGranularityLexicalIDs = 2
                };
        }

        private readonly Kernel _kernel;

        public Quen7bLLMService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> BuildPromptResponse(string userPrompt, string systemPrompt = "")
        {
            var finalSystemPrompt = string.Format(DEFAULT_PROMPT, systemPrompt);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(finalSystemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.Qwen15.GetDescription())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
