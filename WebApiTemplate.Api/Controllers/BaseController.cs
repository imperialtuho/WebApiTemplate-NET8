namespace WebApiTemplate.Api.Controllers
{
    /// <summary>
    /// Serves as a base API controller, providing common functionality for derived controllers.
    /// </summary>
    /// <remarks>
    /// This base controller ensures that all derived controllers inherit essential features such as authorization
    /// and standardized response handling.
    /// </remarks>
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Generates a standardized HTTP response with the specified data and status code.
        /// </summary>
        /// <typeparam name="T">The type of the response data.</typeparam>
        /// <param name="data">The response data to include in the response body.</param>
        /// <param name="statusCode">The HTTP status code of the response. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the response data and the specified HTTP status code.
        /// </returns>
        /// <remarks>
        /// This method provides a consistent response format across all controllers that inherit from <see cref="BaseController"/>.
        /// It converts the given status code to an integer and returns the corresponding HTTP response.
        /// </remarks>
        protected IActionResult Result<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return base.StatusCode((int)statusCode, data);
        }
    }
}