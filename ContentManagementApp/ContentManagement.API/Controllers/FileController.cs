using ContentManagement.API.Services.Contracts;
using ContentManagement.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IO.Pipelines;
using ILogger = Serilog.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public FileController(IFileService fileService, ILogger logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetFileAsStream")]
        public async Task GetFileAsStream()
        {
            string filePath = HttpContext.Request.Headers["FilePath"];

            Response.StatusCode = 200;

            Response.Headers.Append(HeaderNames.ContentDisposition, $"attachment; filename=\"{Path.GetFileName(filePath)}\"");

            Response.Headers.Append(HeaderNames.ContentType, "application/octet-stream");

            string fullPath = _fileService.CreateFullPath(filePath);

            var inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var outputStream = Response.Body;

            int bufferSize = 1 << 10;
            var buffer = new byte[bufferSize];

            while (true)
            {
                var bytesRead = await inputStream.ReadAsync(buffer, 0, bufferSize);
                if (bytesRead == 0)
                {
                    break;
                }
                await outputStream.WriteAsync(buffer, 0, bufferSize);
            }
            await outputStream.FlushAsync();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetFileAsByteArray")]
        public async Task<ActionResult<FileByteArrayDTO>> GetFileAsByteArray()
        {
            try
            {
                string filePath = HttpContext.Request.Headers["FilePath"];

                byte[]? file = await _fileService.GetFileAsByteArrayAsync(filePath);
                FileByteArrayDTO fileByteArrayDTO = new FileByteArrayDTO { File = file, FilePath = filePath };

                if (file is null)
                {
                    return NoContent();
                }

                return Ok(fileByteArrayDTO);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("SaveFileAsByteArray")]
        public async Task<ActionResult<bool>> SaveFileAsByteArray([FromBody] FileByteArrayDTO fileByteArrayDTO)
        {
            try
            {
                byte[] file = fileByteArrayDTO.File;
                string filePath = fileByteArrayDTO.FilePath;

                bool fileSaved = await _fileService.SaveFileFromByteArray(file, filePath);

                if (fileSaved)
                {
                    return Ok(true);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("SaveFileAsStream")]
        public async Task<ActionResult<bool>> SaveFileAsStream()
        {
            try
            {
                //Read file path from header
                string? filePath = null;
                var headers = HttpContext.Request.Headers;
                if (headers.ContainsKey("FilePath"))
                {
                    filePath = headers["FilePath"];

                    if (filePath is null || String.IsNullOrWhiteSpace(filePath))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "Path Missing");
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Header Missing");
                }

                _fileService.CreateSubFolders(filePath);
                string fullPath = _fileService.CreateFullPath(filePath);

                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024, true))
                {
                    await Request.Body.CopyToAsync(fs, 1024);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }


        }

    }
}
