using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : DefaultController<FileQueryDTO, bool>
    {
        private readonly IFileService _fileService;
        public FileController(ILogger<DefaultController<FileQueryDTO, bool>> logger, IFileService fileService) : base(logger)
        {
            _fileService = fileService;
        }

        [HttpGet("exists")]
        public async Task<Results<Ok<bool>, BadRequest<IList<string>>>> Exists([FromQuery] FileQueryDTO file)
        {
            return await Process(_fileService, file);
        }
    }
}
