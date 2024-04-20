using ContentManagement.API.Services.Contracts;
using ContentManagement.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using ILogger = Serilog.ILogger;

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorVisualContentController : ControllerBase
    {
        private readonly IAuthorVisualContentService _authorVisualContentService;
        private readonly ILogger _logger;


        public AuthorVisualContentController(IAuthorVisualContentService authorVisualContentService, ILogger logger)
        {
            _authorVisualContentService = authorVisualContentService;
            _logger = logger;
        }

        // GET: api/<AuthorVisualContentController>
        [HttpGet("getall/{authorid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AuthorVisualContentDTO>>> GetAll(int authorId)
        {
            try
            {
                IEnumerable<AuthorVisualContentDTO> authorVisualContents = await _authorVisualContentService.GetAllAuthorVisualContent(authorId);
                if (authorVisualContents is null || authorVisualContents.Count() == 0)
                {
                    return NoContent();
                }
                return Ok(authorVisualContents);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        // GET api/<AuthorVisualContentController>/5
        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<ActionResult<AuthorVisualContentDTO>> Get(int id)
        {
            try
            {
                AuthorVisualContentDTO authorVisualContent = await _authorVisualContentService.GetAuthorVisualContent(id);
                if (authorVisualContent.IsHttpLink == default)
                {
                    return NoContent();
                }
                return Ok(authorVisualContent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        // POST api/<AuthorVisualContentController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AuthorVisualContentDTO>> Post([FromBody] AuthorVisualContentDTO? authorVisualContentDTO)
        {
            try
            {
                string? user = GetAuthorisedUserEmail(HttpContext);
                if (user is not null && String.IsNullOrWhiteSpace(user) == false)
                {
                    authorVisualContentDTO = await _authorVisualContentService.AddAuthorVisualContent(authorVisualContentDTO, user);
                    if (authorVisualContentDTO is null)
                    {
                        return BadRequest("Unexpected Error");
                    }

                    return CreatedAtAction(nameof(Get), new { id = authorVisualContentDTO.Id }, authorVisualContentDTO);
                }
                else
                {
                    return BadRequest("User not found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                if (ex.Message.Contains("Validation Errors"))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error!");
            }
        }

        // PUT api/<AuthorVisualContentController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Put(int id, [FromBody] AuthorVisualContentDTO authorVisualContent)
        {
            try
            {
                bool updated = await _authorVisualContentService.UpdateAuthorVisualContent(authorVisualContent);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                if (ex.Message.Contains("Validation Error"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        // DELETE api/<AuthorVisualContentController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                bool deleted = await _authorVisualContentService.DeleteAuthorVisualContent(id);

                if (deleted == false)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to delete content");
                }
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                if (ex.Message.Contains("Validation Error"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                else if (ex.Message.Contains("Not Allowed"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        private static string? GetAuthorisedUserEmail(HttpContext ctx)
        {
            var userIdentity = ctx.User.Identity as ClaimsIdentity;
            if (userIdentity.IsAuthenticated)
            {
                string? email = userIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return email;
            }

            return null;

        }
    }
}
