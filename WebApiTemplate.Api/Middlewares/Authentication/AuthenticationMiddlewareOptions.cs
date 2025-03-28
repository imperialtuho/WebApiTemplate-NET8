using Microsoft.AspNetCore.Authentication;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Options for configuring the authentication middleware.
    /// </summary>
    public class AuthenticationMiddlewareOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether token validation is enabled.
        /// </summary>
        public bool EnableTokenValidation { get; set; } = true;

        /// <summary>
        /// Gets or sets the allowed issuer for JWT validation.
        /// </summary>
        public string AllowedIssuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the allowed audience for JWT validation.
        /// </summary>
        public string AllowedAudience { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the signing key for JWT validation.
        /// </summary>
        public string SigningKey { get; set; } = string.Empty;
    }
}