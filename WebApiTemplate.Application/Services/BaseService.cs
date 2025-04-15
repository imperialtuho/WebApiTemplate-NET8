using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Extensions;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Application.Services
{
    /// <summary>
    /// Provides common service functionality for the application, including user session management, tenant identification, and admin checks.
    /// </summary>
    /// <remarks>
    /// This base service class serves as a foundation for other services in the application. It allows derived services to manage user sessions,
    /// tenant identification, and access control, specifically for checking if actions are performed by admin users.
    /// It also provides utility methods for validating whether the current operation is allowed for the current user based on ownership.
    /// </remarks>
    public class BaseService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor for retrieving user session and tenant information.</param>
        /// <param name="mapper">The AutoMapper instance for mapping objects.</param>
        public BaseService(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves the current user session associated with the active HTTP request.
        /// </summary>
        /// <value>
        /// Returns an instance of <see cref="UserSession"/> containing information about the authenticated user.
        /// </value>
        /// <remarks>
        /// This property provides access to user-specific session data extracted from the HTTP context,
        /// typically used for authentication, authorization, or auditing purposes.
        /// </remarks>
        public UserSession LoginSession => _httpContextAccessor.GetUserSession();

        /// <summary>
        /// Determines whether the action is performed by an admin user.
        /// </summary>
        /// <param name="currentUser">The current user session. If null, assumes a non-admin user.</param>
        /// <param name="tenantId">Optional tenant ID to check admin scope.</param>
        /// <returns>True if the user is a SuperAdmin or an Admin in the specified tenant; otherwise, false.</returns>
        /// <remarks>
        /// A user is considered an admin if they have the 'SuperAdmin' role, or the 'Admin' role within the same tenant.
        /// </remarks>
        protected static bool IsActionPerformedByAdmin(UserSession? currentUser = null, int? tenantId = null)
        {
            if (currentUser is null || currentUser.Roles is null)
                return false;

            // Check SuperAdmin role
            if (currentUser.Roles.Any(r => r == ApplicationDefaultRoleValue.SuperAdmin))
                return true;

            // Check Admin role with matching tenant
            if (currentUser.Roles.Any(r => r == ApplicationDefaultRoleValue.Admin) && currentUser.TenantId == tenantId)
                return true;

            return false;
        }

        /// <summary>
        /// Validates whether the current user is authorized to perform an operation based on role or ownership.
        /// </summary>
        /// <param name="ownerId">The ID of the resource owner.</param>
        /// <param name="tenantId">The tenant ID to validate against for tenant-scoped admin access.</param>
        /// <exception cref="ForbiddenException">Thrown when the user is unauthorized to perform the operation.</exception>
        /// <remarks>
        /// An action is allowed if the user is a SuperAdmin, an Admin within the same tenant, or the resource owner.
        /// </remarks>
        protected void CheckingCurrentPerformingOperation(string? ownerId = null, int? tenantId = null)
        {
            UserSession? loginSession = LoginSession ?? throw new ForbiddenException();

            // Admin check
            if (IsActionPerformedByAdmin(loginSession, tenantId))
                return;

            // Ownership check
            if (!string.IsNullOrEmpty(ownerId) && loginSession.UserId == ownerId && loginSession.TenantId == tenantId)
                return;

            throw new ForbiddenException();
        }
    }
}