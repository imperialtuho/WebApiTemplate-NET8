using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Domain.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IHttpContextAccessor"/> to retrieve user session and tenant information.
    /// This class includes methods for accessing the authenticated user session and extracting the tenant ID from the HTTP request headers.
    /// </summary>
    public static class HttpContextAccessorExtension
    {
        private const string TenantHeaderKey = "TenantIdentify";

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

        /// <summary>
        /// Retrieves the tenant identifier from the request headers.
        /// The method looks for the "TenantIdentify" header, parses its value, and returns it as an integer.
        /// If the header is missing or the value cannot be parsed as an integer, `null` is returned.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
        /// <returns>The tenant ID as an integer if present and valid, otherwise `null`.</returns>
        /// <remarks>
        /// This method checks the HTTP request headers for the "TenantIdentify" header and attempts to parse its value.
        /// If the header is not found or the value is not a valid integer, the method returns `null`.
        /// </remarks>
        public static int? GetTenantIdentify(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext?.Request?.Headers.TryGetValue(TenantHeaderKey, out StringValues tenantIdValue) == true && int.TryParse(tenantIdValue, out int tenantId))
            {
                return tenantId; // Valid Tenant ID
            }

            return null; // Missing or Invalid Tenant ID
        }
    }
}