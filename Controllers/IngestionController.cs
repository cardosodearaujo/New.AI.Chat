using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    /// <summary>
    /// Controlador responsável por expor endpoints para ingestão de arquivos e dados,
    /// permitindo que clientes enviem conteúdo a ser transformado e armazenado de forma assíncrona.
    /// </summary>
    /// <remarks>
    /// Utiliza a abstração <see cref="IIngestionService"/> para processar os itens ingeridos.
    /// Herda de <see cref="DefaultController{TRequest, TResponse}"/> para aplicar comportamento
    /// comum de validação e tratamento de respostas.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class IngestionController : DefaultController<IngestionDTO, IngestionResponseDTO>
    {
        /// <summary>
        /// Serviço injetado responsável pelo processamento e transformação dos arquivos/dados ingeridos.
        /// </summary>
        private readonly IIngestionService _ingestionService;

        /// <summary>
        /// Cria uma nova instância do <see cref="IngestionController"/>.
        /// </summary>
        /// <param name="logger">Instância de <see cref="ILogger{T}"/> fornecida pelo contêiner de DI para logging.</param>
        /// <param name="ingestionService">Serviço de ingestão injetado que contém a lógica de processamento.</param>
        public IngestionController(
            ILogger<DefaultController<IngestionDTO, IngestionResponseDTO>> logger,
            IIngestionService ingestionService
            ) : base(logger)
        {
            _ingestionService = ingestionService;
        }

        /// <summary>
        /// Recebe os dados de ingestão (por exemplo, arquivos e metadados) e inicia o processamento.
        /// </summary>
        /// <remarks>
        /// Este endpoint deve ser chamado via HTTP POST. O processamento é realizado de forma assíncrona
        /// e a validação/normalização dos dados é delegada ao método <see cref="DefaultController{TRequest, TResponse}.Process"/>.
        /// </remarks>
        /// <param name="ingestion">Objeto contendo os arquivos e/ou metadados a serem ingeridos, conforme o contrato <see cref="IngestionDTO"/>.</param>
        /// <returns>
        /// Retorna um objeto <see cref="Results"/> que contém:
        /// - <see cref="Ok{T}"/> com <see cref="IngestionResponseDTO"/> em caso de sucesso;
        /// - <see cref="BadRequest{T}"/> com uma lista de mensagens de erro em caso de falha de validação.
        /// </returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<Results<Ok<IngestionResponseDTO>, BadRequest<IList<string>>>> Ingestion([FromBody]IngestionDTO ingestion)
        {
            return await Process(_ingestionService, ingestion);
        }
    }
}
