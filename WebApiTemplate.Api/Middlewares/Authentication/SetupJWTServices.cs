using WebApiTemplate.Application.Configurations.Settings;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Set up JWT services.
    /// </summary>
    public static class SetupJwtServices
    {
        /// <summary>
        /// Adds JWT services into ServiceCollection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddJwtServices(this IServiceCollection services, IConfiguration configuration)
        {
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>($"{nameof(JwtSettings)}:IdentityUrl");

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { });
        }
    }
}