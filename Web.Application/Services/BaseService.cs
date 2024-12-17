using AutoMapper;
using Microsoft.AspNetCore.Http;
using Web.Domain.Common;
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
        /// Checks the action is being performed by admin or its owner.
        /// </summary>
        /// <param name="ownerId">The data's ownerId.</param>
        /// <returns>True if action is performed by admin or the ownerId matched with its own data.</returns>
        protected bool IsCurrentPerformingOperationValid(string? ownerId = null)
        {
            UserSession? loginSession = LoginSession;

            // if the action performs by admins -> valid
            if (loginSession is not null && IsActionPerformByAdmin(loginSession))
            {
                return true;
            }

            // ownerId is null -> invalid
            if (ownerId == null)
            {
                return false;
            }

            // Checks owner's data to its action. If owner's data matched with the provided ownerId -> valid
            if (!string.IsNullOrEmpty(ownerId) && ownerId.Equals(loginSession?.UserId))
            {
                return true;
            }

            return false;
        }
    }
}