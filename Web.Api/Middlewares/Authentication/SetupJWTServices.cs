using Web.Application.Configurations.Settings;

namespace Web.Api.Middlewares.Authentication
{
    public static class SetupJWTServices
    {
        public static void AddJWTServices(this IServiceCollection services, IConfiguration configuration)
        {
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>($"{nameof(JwtSettings)}:IdentityUrl");

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { });
        }
    }
}