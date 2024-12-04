namespace Web.Api.Controllers
{
    /// <summary>
    /// Base controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Return a result from performing action.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="data">The data will be returned</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>Return a result after performing controller's action.</returns>
        protected IActionResult Result<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return base.StatusCode((int)statusCode, data);
        }
    }
}