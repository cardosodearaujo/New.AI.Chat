using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    /// <summary>
    /// Fornece endpoints de API para operações de chat, permitindo que clientes enviem mensagens e recebam respostas
    /// de forma assíncrona.
    /// </summary>
    /// <remarks>Este controller utiliza o IChatService para processar mensagens de chat e retorna resultados indicando
    /// sucesso ou falha. Foi projetado para integração com aplicações cliente que exigem funcionalidade de chat via
    /// HTTP. Todas as ações são executadas de forma assíncrona para suportar interações escaláveis e responsivas.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : DefaultController<PromptDTO, PromptResponseDTO>
    {
        /// <summary>
        /// Serviço injetado responsável pelo processamento das mensagens de chat e geração de respostas.
        /// </summary>
        private readonly IChatService _chatService;

        /// <summary>
        /// Cria uma nova instância de <see cref="ChatController"/>.
        /// </summary>
        /// <param name="logger">Instância de <see cref="ILogger{ChatController}"/> fornecida pelo contêiner de DI para registro de logs.</param>
        /// <param name="chatService">Serviço de chat injetado que contém a lógica de processamento das mensagens.</param>
        public ChatController(
            ILogger<ChatController> logger,
            IChatService chatService) : base(logger)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Processa uma mensagem de prompt e retorna o resultado, que pode ser uma resposta bem-sucedida ou uma lista de
        /// erros de validação.
        /// </summary>
        /// <remarks>Este método é destinado a ser chamado via uma requisição HTTP POST. Certifique-se de que a
        /// mensagem esteja no formato esperado para evitar erros de validação.</remarks>
        /// <param name="message">A mensagem de prompt a ser processada. Deve conter dados válidos conforme definido pelo esquema PromptDTO.</param>
        /// <returns>Um objeto Results contendo ou um Ok com o PromptResponseDTO processado, ou um BadRequest com uma lista de mensagens de erro de validação.</returns>
        [HttpPost]
        public async Task<Results<Ok<PromptResponseDTO>, BadRequest<IList<string>>>> SendMessage(PromptDTO message)
        {
            return await Process(_chatService, message);
        }
    }
}