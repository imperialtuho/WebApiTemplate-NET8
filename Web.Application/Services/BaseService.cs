using AutoMapper;
using Microsoft.AspNetCore.Http;
using Web.Domain.Common;
using Web.Domain.Exceptions;
using Web.Domain.Extensions;
using Web.Domain.SharedKernel;

namespace Web.Application.Services
{
    public class BaseService
    {
        private UserSession? _userSession;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMapper _mapper;

        public BaseService(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected int? TenantIdentify => _httpContextAccessor.GetTenantIdentify();
        public int? TenantId => LoginSession?.TenantId ?? TenantIdentify;

        public UserSession? LoginSession
        {
            get => _userSession ?? _httpContextAccessor?.GetUserSession();
            set
            {
                _userSession = value;
            }
        }

        /// <summary>
        /// Checking the action is performed by admin or not.
        /// </summary>
        /// <param name="currentUser">The currentUser.</param>
        /// <returns>True/False based on currentUser.</returns>
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
        /// Checking current performing operation.
        /// </summary>
        /// <param name="ownerId">The ownerId.</param>
        /// <exception cref="ForbiddenException">When current operation is invalid/forbidden.</exception>
        protected void CheckingCurrentPerformingOperation(string? ownerId = null)
        {
            UserSession? loginSession = LoginSession;
            string message = $"You're not allowed to perform this action";

            // if the action is not performing by admins -> forbidden
            if (loginSession is null && !IsActionPerformByAdmin(loginSession))
            {
                throw new ForbiddenException(message);
            }

            if (string.IsNullOrEmpty(ownerId) || !ownerId.Equals(loginSession?.UserId))
            {
                throw new ForbiddenException(message);
            }
        }
    }
}