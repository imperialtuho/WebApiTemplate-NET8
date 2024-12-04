using System.Security.Claims;
using Web.Domain.SharedKernel;

namespace Web.Domain.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        private const string Permission = nameof(Permission);

        public static UserSession GetUserSession(this ClaimsPrincipal claimsPrincipal)
        {
            string? userEmail = claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value;
            string? userId = claimsPrincipal?.FindFirst(ClaimTypes.Sid)?.Value;
            string? tenantId = claimsPrincipal?.FindFirst("tenantId")?.Value ?? "0";
            var userSession = new UserSession();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return userSession;
            }

            List<string>? roles = claimsPrincipal!.Claims.Where(c => c.Type.Equals(ClaimTypes.Role)).Select(c => c.Value).ToList();
            List<string>? permissions = claimsPrincipal!.Claims.Where(c => c.Type.Equals(Permission)).Select(c => c.Value).ToList();

            return new UserSession()
            {
                Email = userEmail,
                UserId = userId,
                Roles = roles,
                Permissions = permissions,
                TenantId = int.Parse(tenantId)
            };
        }
    }
}