using Microsoft.AspNetCore.Mvc;
using Web.Application.Dtos;
using Web.Application.Interfaces.Services;
using Web.Domain.Exceptions;

namespace Web.Api.Controllers
{
    /// <summary>
    /// The BlogsController.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="blogService">The blogService.</param>
    [ApiController]
    [Route("[controller]")]
    public class BlogsController(ILogger<BlogsController> logger, IBlogService blogService) : ControllerBase
    {
        /// <summary>
        /// Gets blog by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult{BlogDto}</returns>
        /// <exception cref="UnhandledException"></exception>
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await blogService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                string errorMsg = $"Call {nameof(BlogsController)}-{nameof(GetByIdAsync)} error with {ex.Message}";
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}