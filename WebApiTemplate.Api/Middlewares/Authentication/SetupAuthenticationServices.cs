using WebApiTemplate.Application.Configurations.Settings;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Provides services for configuring authentication mechanisms in the API.
    /// This includes registration of JWT-based authentication and custom authentication schemes.
    /// </summary>
    /// <remarks>
    /// The <see cref="SetupAuthenticationServices"/> class handles the registration of authentication services for the application.
    /// It supports adding custom authentication schemes (like Basic authentication) and JWT authentication by configuring them
    /// via the application's service collection. The configurations are loaded from the app's settings file (e.g., appsettings.json).
    /// </remarks>
    public static class SetupAuthenticationServices
    {
        /// <summary>
        /// Registers authentication services, including JWT and custom authentication schemes, into the application's service collection.
        /// </summary>
        /// <param name="services">The service collection to add authentication services to.</param>
        /// <param name="configuration">The application's configuration settings, which are used to load authentication-related settings.</param>
        /// <remarks>
        /// This method configures the authentication services by registering a custom authentication scheme (e.g., Basic authentication)
        /// and loading the necessary settings for JWT authentication, including the identity URL from the configuration.
        /// </remarks>
        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load Identity URL from configuration for JWT settings
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>($"{nameof(JwtSettings)}:IdentityUrl");

            // Register custom authentication scheme (e.g., Basic authentication)
            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { });
        }
    }
}