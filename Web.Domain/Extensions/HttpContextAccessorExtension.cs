using Microsoft.AspNetCore.Http;
using Web.Domain.SharedKernel;

namespace Web.Domain.Extensions
{
    public static class HttpContextAccessorExtension
    {
        private const string DEFAULT_TENANT_ID = "0";

        public static UserSession GetUserSession(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.GetUserSession() ?? new UserSession();
        }

        public static int? GetTenantIdentify(this IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                return int.Parse(httpContextAccessor?.HttpContext?.Request?.Headers["TenantIdentify"].ToString() ?? DEFAULT_TENANT_ID);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}