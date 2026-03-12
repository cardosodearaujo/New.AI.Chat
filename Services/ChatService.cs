using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Pgvector.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

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
            var relevantDocument = new StreamingKernelContentItemCollection(); /*await _aiDbContext.DbSetKDInformation
                                                     .OrderBy(F => F.Embedding.L2Distance(searchVector))
                                                     .Take(3)
                                                     .ToListAsync();*/

            var context = new StringBuilder();
            foreach (var document in relevantDocument)
            {
                //context.AppendLine($"Arquivo: {document.Font}");
                //context.AppendLine(document.ContentText);
                //context.AppendLine($"");
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
                    //ReferenceFiles = relevantDocument.Select(F => F.Font).ToList(),
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


/*using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using Pgvector.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace New.AI.Chat.Services
{
    public class ChatService : DefaultService<PromptDTO, PromptResponseDTO>, IChatService
    {
        private const string MASTER_PROMPT = @"
            Você é um Arquiteto de Software Sênior e Engenheiro de Dados.
            Abaixo, você receberá blocos completos de código-fonte (Contexto Expandido).
            Responda à pergunta do utilizador detalhadamente, utilizando APENAS o contexto fornecido.
            Se a resposta exigir a citação de código, apresente o código.
            Se a resposta não estiver no contexto, diga claramente: 'Não encontrei a resposta nos módulos mapeados'.
            Não alucine ou invente regras de negócio.

            CONTEXTO ARQUITETURAL ENCONTRADO:
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
            if (string.IsNullOrWhiteSpace(prompt?.Message))
            {
                AddError("A mensagem do utilizador não pode ser vazia!");
            }
        }

        protected override async Task DoProcess(PromptDTO prompt)
        {
            // 1. Gera o vetor da pergunta
            var messageVectorResult = await _vectorGenerator.GenerateAsync([prompt.Message]);
            var searchVector = new Pgvector.Vector(messageVectorResult[0].Vector.ToArray());

            // 2. Extrai possíveis termos técnicos (ex: FrmVendas, txt_CodCli, CalcularImposto)
            var termosTecnicos = ExtrairTermosTecnicos(prompt.Message);

            // Conjunto para armazenar os IDs únicos dos Pais encontrados nas 3 buscas
            var parentIdsEncontrados = new HashSet<Guid>();

            // --- EIXO 1: Busca Semântica na Lente Grande Angular (PAI) ---
            var idsPaiSemanticos = await _aiDbContext.KnowledgeDocumentParents
                .OrderBy(p => p.Embedding.L2Distance(searchVector))
                .Take(2) // Pega os 2 módulos mais semanticamente alinhados
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var id in idsPaiSemanticos) parentIdsEncontrados.Add(id);

            // --- EIXO 2: Busca Semântica na Lente de Aumento (FILHO) ---
            var idsPaiViaFilhoSemantico = await _aiDbContext.KnowledgeDocumentChildren
                .OrderBy(c => c.Embedding.L2Distance(searchVector))
                .Take(3) // Pega as 3 lógicas/métodos mais parecidos e sobe para o Pai
                .Select(c => c.ParentId)
                .ToListAsync();

            foreach (var id in idsPaiViaFilhoSemantico) parentIdsEncontrados.Add(id);

            // --- EIXO 3: Busca Textual Exata na Lente de Aumento (FILHO Lexical) ---
            if (termosTecnicos.Any())
            {
                foreach (var termo in termosTecnicos)
                {
                    // ILIKE usa o índice pg_trgm no PostgreSQL para buscas ultra rápidas
                    var idsPaiViaLexical = await _aiDbContext.KnowledgeDocumentChildren
                        .Where(c => EF.Functions.ILike(c.ContentText, $"%{termo}%"))
                        .Take(2)
                        .Select(c => c.ParentId)
                        .ToListAsync();

                    foreach (var id in idsPaiViaLexical) parentIdsEncontrados.Add(id);
                }
            }

            // 3. Hidratação: Resgata o texto completo (800 tokens) dos pais encontrados
            var documentosRelevantes = await _aiDbContext.KnowledgeDocumentParents
                .Where(p => parentIdsEncontrados.Contains(p.Id))
                .ToListAsync();

            if (!documentosRelevantes.Any())
            {
                AddError("Nenhum contexto relevante foi encontrado na base de código.");
                return;
            }

            // 4. Constrói o Super Contexto
            var context = new StringBuilder();
            foreach (var document in documentosRelevantes)
            {
                context.AppendLine($"--- Início do Arquivo/Módulo: {document.Font} ---");
                context.AppendLine(document.ContentText);
                context.AppendLine($"--- Fim do Arquivo/Módulo ---");
                context.AppendLine();
            }

            var fullPrompt = string.Format(MASTER_PROMPT, context.ToString());

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(fullPrompt);
            chatHistory.AddUserMessage(prompt.Message);

            // 5. Chamada ao LLM
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);

            if (response != null)
            {
                Data = new PromptResponseDTO
                {
                    Response = response.Content,
                    ReferenceFiles = documentosRelevantes.Select(f => f.Font).Distinct().ToList(),
                    DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };
            }
            else
            {
                AddError("A IA não retornou nenhuma resposta válida.");
            }
        }

        /// <summary>
        /// Método privado para extrair padrões de código da pergunta do usuário.
        /// Identifica CamelCase, PascalCase, snake_case ou palavras maiores que 5 caracteres.
        /// </summary>
        private List<string> ExtrairTermosTecnicos(string pergunta)
        {
            var termos = new HashSet<string>();

            // Regex para capturar palavras que parecem código (ex: CalcularDIFAL, id_cliente)
            var regexCodigo = new Regex(@"\b([a-z]+[A-Z][a-zA-Z]*|[A-Z][a-zA-Z]*[A-Z][a-zA-Z]*|[a-zA-Z]+_[a-zA-Z0-9_]+)\b");
            var matches = regexCodigo.Matches(pergunta);

            foreach (Match match in matches)
            {
                termos.Add(match.Value);
            }

            // Fallback: se não achar padrão de código, pega palavras-chave longas
            if (!termos.Any())
            {
                var palavras = pergunta.Split(new[] { ' ', '.', ',', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
                var ignoreList = new[] { "como", "onde", "qual", "quem", "para", "porque", "sobre", "arquivo", "classe", "metodo" };

                foreach (var palavra in palavras)
                {
                    if (palavra.Length > 5 && !ignoreList.Contains(palavra.ToLower()))
                    {
                        termos.Add(palavra);
                    }
                }
            }

            return termos.ToList();
        }
    }
}*/