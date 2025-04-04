namespace WebApiTemplate.Domain.Constants
{
    /// <summary>
    /// Contains application-wide constants used for configuration and environment setup.
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// The key used for specifying allowed origins in CORS policy configuration.
        /// </summary>
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// The key used for storing the environment name in configuration settings.
        /// </summary>
        public const string EnvironmentName = "_environmentName";

        /// <summary>
        /// The environment variable name for the ASP.NET Core environment.
        /// </summary>
        public const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// The default environment name for development purposes.
        /// </summary>
        public const string DefaultEnvironmentName = "Development";
    }
}