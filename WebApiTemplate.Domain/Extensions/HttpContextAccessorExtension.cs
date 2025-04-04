using Microsoft.AspNetCore.Http;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Domain.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IHttpContextAccessor"/> to retrieve user session and tenant information.
    /// This class includes methods for accessing the authenticated user session and extracting the tenant ID from the HTTP request headers.
    /// </summary>
    public static class HttpContextAccessorExtension
    {
        /// <summary>
        /// Retrieves the user session from the HTTP context.
        /// This extension method uses the authenticated user's claims to construct a <see cref="UserSession"/> object.
        /// If the user is not authenticated or if the user session is not available, an empty <see cref="UserSession"/> is returned.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
        /// <returns>A <see cref="UserSession"/> object containing the authenticated user's details, or an empty session if not available.</returns>
        /// <remarks>
        /// This method checks the <see cref="HttpContext"/> of the current request to retrieve the user session.
        /// If the user is authenticated, their details will be used to populate the session. Otherwise, a default, empty session is returned.
        /// </remarks>
        public static UserSession GetUserSession(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.GetUserSession() ?? new UserSession();
        }
    }
}