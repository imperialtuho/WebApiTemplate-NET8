namespace Web.Api.Middlewares.Authentication
{
    public static class SetupJWTServices
    {
        public static void AddJWTServices(this IServiceCollection services, IConfiguration configuration)
        {
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>("Jwt:IdentityUrl");

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { });
        }
    }
}