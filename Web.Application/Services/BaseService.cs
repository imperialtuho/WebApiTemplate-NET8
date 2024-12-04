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
    }
}