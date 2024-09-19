using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    /// <summary>
    /// The BlogsController.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="blogService">The blogService.</param>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BlogsController(ILogger<BlogsController> logger, IBlogService blogService) : ControllerBase
    {
        /// <summary>
        /// Gets blog by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult{BlogDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogDto>> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await blogService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(GetByIdAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}