using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Domain.Extensions
{
    public static class HttpContextAccessorExtension
    {
        private const string TenantHeaderKey = "TenantIdentify";

        /// <summary>
        /// Retrieves the user session from the HTTP context.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <returns>A <see cref="UserSession"/> object containing user details.</returns>
        public static UserSession GetUserSession(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.GetUserSession() ?? new UserSession();
        }

        /// <summary>
        /// Retrieves the tenant identifier from the request headers.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <returns>
        /// The valid tenant ID as an integer, or `null` if the header is missing or contains an invalid value.
        /// </returns>
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