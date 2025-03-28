using WebApiTemplate.Application.Configurations.Settings;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Provides authentication services configuration, including JWT and custom authentication schemes.
    /// </summary>
    public static class SetupAuthenticationServices
    {
        /// <summary>
        /// Registers authentication services, including JWT and custom authentication schemes, into the application's service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Application configuration settings.</param>
        public static void AddAthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load Identity URL from configuration
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>($"{nameof(JwtSettings)}:IdentityUrl");

            // Register custom authentication scheme
            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { 
                });
        }
    }
}