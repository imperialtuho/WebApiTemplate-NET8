namespace WebApiTemplate.Api.Controllers
{
    /// <summary>
    /// Provides a base API controller with common functionality for derived controllers.
    /// </summary>
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Returns a standardized HTTP response with the given data and status code.
        /// </summary>
        /// <typeparam name="T">The type of the response data.</typeparam>
        /// <param name="data">The response data to be returned.</param>
        /// <param name="statusCode">The HTTP status code of the response. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        /// <returns>An <see cref="IActionResult"/> containing the response data and status code.</returns>
        protected IActionResult Result<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return base.StatusCode((int)statusCode, data);
        }
    }
}