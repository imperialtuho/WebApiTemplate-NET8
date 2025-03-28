using System.Security.Claims;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Domain.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        private const string Permission = nameof(Permission);
        private const string TenantIdClaim = "tenantId";

        /// <summary>
        /// Retrieves user session details from the claims principal.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal representing the authenticated user.</param>
        /// <returns>A <see cref="UserSession"/> object containing user details.</returns>
        public static UserSession GetUserSession(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) return new UserSession();

            string? userEmail = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.Sid);
            string? tenantIdStr = claimsPrincipal.FindFirstValue(TenantIdClaim) ?? "0";

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UserSession();
            }

            int tenantId = int.TryParse(tenantIdStr, out int parsedTenantId) ? parsedTenantId : 0;

            List<string> roles = claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            List<string> permissions = claimsPrincipal.FindAll(Permission).Select(c => c.Value).ToList();

            return new UserSession
            {
                Email = userEmail,
                UserId = userId,
                Roles = roles,
                Permissions = permissions,
                TenantId = tenantId
            };
        }
    }
}