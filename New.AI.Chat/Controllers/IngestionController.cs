using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngestionController : DefaultController
    {
        private readonly IIngestionService _ingestionService;

        public IngestionController(
            ILogger<DefaultController> logger,
            IIngestionService ingestionService
            ) : base(logger)
        {
            _ingestionService = ingestionService;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<IngestionResponseDTO>> Ingestion([FromBody]IngestionDTO ingestion)
        {
            return await Process(_ingestionService, ingestion);
        }
    }
}
