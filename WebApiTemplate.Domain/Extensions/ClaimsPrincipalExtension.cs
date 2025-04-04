using System.Security.Claims;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Domain.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="ClaimsPrincipal"/> class to retrieve user session details.
    /// This class is responsible for extracting user-specific information (such as user ID, email, roles, permissions, and tenant ID)
    /// from the claims associated with an authenticated user. The extensions are useful for managing authorization, user context, and tenant-specific data.
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        private const string Permission = nameof(Permission); // Constant for permission claim type
        private const string TenantIdClaim = "tenantId"; // Constant for tenant ID claim type

        /// <summary>
        /// Retrieves user session details from the <see cref="ClaimsPrincipal"/> object.
        /// This includes the user's email, user ID, roles, permissions, and tenant ID.
        /// The method ensures the user session is populated even when some claims might be missing,
        /// returning a default session if essential claims (such as user ID) are not found.
        /// </summary>
        /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> representing the authenticated user.</param>
        /// <returns>A <see cref="UserSession"/> object containing user-specific details like email, user ID, roles, permissions, and tenant ID.</returns>
        /// <remarks>
        /// If the <paramref name="claimsPrincipal"/> is null or lacks required claims (such as user ID),
        /// the method will return a default <see cref="UserSession"/> object.
        /// </remarks>
        public static UserSession GetUserSession(this ClaimsPrincipal claimsPrincipal)
        {
            // Return an empty UserSession if claimsPrincipal is null
            if (claimsPrincipal == null) return new UserSession();

            // Retrieve the user's email and user ID from the claims
            string? userEmail = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.Sid);

            // Return an empty session if user ID or Email is missing
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userEmail))
            {
                return new UserSession();
            }

            // Parse and retrieve tenant ID from claims (default to "0" if not present)
            int tenantId = int.TryParse(claimsPrincipal.FindFirstValue(TenantIdClaim), out int parsedTenantId) ? parsedTenantId : 0;

            // Retrieve roles and permissions
            List<string> roles = claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            List<string> permissions = claimsPrincipal.FindAll(Permission).Select(c => c.Value).ToList();

            // Return a populated UserSession object
            return new UserSession
            {
                UserId = userId,
                Email = userEmail,
                Roles = roles,
                Permissions = permissions,
                TenantId = tenantId
            };
        }
    }
}