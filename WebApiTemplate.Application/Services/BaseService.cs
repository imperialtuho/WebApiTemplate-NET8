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
        /// Checks if the action is being performed by an admin user.
        /// </summary>
        /// <param name="currentUser">The current user session to check. If null, it assumes a non-admin user.</param>
        /// <returns>True if the current user is an admin, otherwise false.</returns>
        /// <remarks>
        /// This method checks if the current user session has roles that include either 'SuperAdmin' or 'Admin'.
        /// If no user session is provided, it defaults to returning false.
        /// </remarks>
        protected static bool IsActionPerformByAdmin(UserSession? currentUser = null)
        {
            if (currentUser is null)
            {
                return false;
            }

            if (currentUser.Roles is not null && currentUser.Roles.Exists(r => r.Contains(ApplicationDefaultRoleValue.SuperAdmin) || r.Contains(ApplicationDefaultRoleValue.Admin)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the current operation is allowed based on ownership and user roles.
        /// </summary>
        /// <param name="ownerId">The owner ID of the resource being accessed. If null, the method checks if the current user owns the resource.</param>
        /// <exception cref="ForbiddenException">Thrown if the current user is not allowed to perform the action.</exception>
        /// <remarks>
        /// This method checks if the user performing the action is either an admin or the owner of the resource.
        /// If neither condition is true, a <see cref="ForbiddenException"/> is thrown to prevent unauthorized actions.
        /// </remarks>
        protected void CheckingCurrentPerformingOperation(string? ownerId = null)
        {
            UserSession loginSession = LoginSession;

            // If the action is not performed by admins -> forbidden
            if (loginSession is null && !IsActionPerformByAdmin(loginSession))
            {
                throw new ForbiddenException();
            }

            // If ownerId is specified, check if the current user owns the resource
            if (string.IsNullOrEmpty(ownerId) || !ownerId.Equals(loginSession?.UserId))
            {
                throw new ForbiddenException();
            }
        }
    }
}