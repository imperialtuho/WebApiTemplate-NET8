using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using WebApiTemplate.Application.Configurations.Settings;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Helpers;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationMiddlewareHandler"/> class, responsible for handling JWT authentication.
    /// </summary>
    /// <param name="options">Provides access to authentication options monitored for changes.</param>
    /// <param name="applicationSettings">Contains application-wide settings, including authentication configurations.</param>
    /// <param name="logger">Factory for creating loggers to record authentication events.</param>
    /// <param name="encoder">Encoder used for handling URL encoding in authentication operations.</param>
    /// <param name="cache">Memory cache used for storing authentication-related data, such as JWT settings.</param>
    /// <param name="httpClientFactory">Factory for creating HTTP clients to communicate with external authentication services.</param>
    /// <remarks>
    /// This constructor initializes dependencies required for JWT validation, including configuration management,
    /// logging, caching, and external service communication.
    /// </remarks>
    public class AuthenticationMiddlewareHandler(
        IOptionsMonitor<AuthenticationMiddlewareOptions> options,
        IOptions<ApplicationSettings> applicationSettings,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory) : AuthenticationHandler<AuthenticationMiddlewareOptions>(options, logger, encoder)
    {
        /// <summary>
        /// Logger instance for recording authentication-related events and errors.
        /// </summary>
        /// <remarks>
        /// This logger is used throughout the middleware to capture validation errors, token processing issues,
        /// and interactions with external authentication services.
        /// </remarks>
        private readonly ILogger _logger = logger.CreateLogger<AuthenticationMiddlewareHandler>();

        /// <summary>
        /// The base URL of the identity service used for authentication requests.
        /// </summary>
        /// <remarks>
        /// This URL is used when fetching authentication-related configurations or validating tokens
        /// against an external identity provider.
        /// </remarks>
        public static string? IdentityUrl { get; set; }

        /// <summary>
        /// The cache key used for storing and retrieving JWT settings from memory.
        /// </summary>
        /// <remarks>
        /// JWT settings are stored in memory cache to optimize performance and reduce
        /// the number of requests made to the identity service.
        /// </remarks>
        private const string CacheKey = nameof(JwtSettings);

        /// <summary>
        /// Predefined constant for representing an unauthorized access response.
        /// </summary>
        /// <remarks>
        /// This value is used to standardize authentication failure messages across the middleware.
        /// </remarks>
        private const string Unauthorized = nameof(Unauthorized);

        /// <summary>
        /// The prefix used to identify Bearer tokens in the Authorization header.
        /// </summary>
        /// <remarks>
        /// The authentication middleware extracts and validates tokens prefixed with this keyword.
        /// </remarks>
        private const string Bearer = nameof(Bearer);

        /// <summary>
        /// Default JSON serialization options used for processing authentication-related data.
        /// </summary>
        /// <remarks>
        /// These options ensure case-insensitive property matching and prevent null values from being included
        /// in serialized responses.
        /// </remarks>
        private readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Optional: ignore case in property names
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Asynchronously handles authentication for incoming requests by validating the provided JWT token.
        /// </summary>
        /// <returns>
        /// A task representing the authentication process, returning an <see cref="AuthenticateResult"/> that indicates success or failure.
        /// </returns>
        /// <remarks>
        /// This method extracts the token from the request's Authorization header, validates it, and returns an authentication ticket
        /// if the token is valid. If authentication fails, an appropriate failure result is returned.
        /// </remarks>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
            {
                return AuthenticateResult.Fail(Unauthorized);
            }

            string? authorizationHeader = value;

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith(Bearer, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail(Unauthorized);
            }

            string token = authorizationHeader.Substring(Bearer.Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail(Unauthorized);
            }

            try
            {
                return await ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ValidateTokenAsync)} failed!");

                return AuthenticateResult.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously validates a provided JWT token and constructs an authentication ticket.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>
        /// A task representing the validation process, returning an <see cref="AuthenticateResult"/>.
        /// </returns>
        /// <remarks>
        /// This method extracts claims from the token and associates them with an authentication ticket if the token is valid.
        /// If validation fails, an authentication failure result is returned.
        /// </remarks>
        private async Task<AuthenticateResult> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail(Unauthorized);
            }

            ClaimsIdentity identity = await GetIdentityFromTokenAsync(token);
            identity.AddClaim(new Claim("AccessToken", token));

            GenericPrincipal principal = new GenericPrincipal(identity, null);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// Asynchronously extracts identity information from a JWT token by validating its claims.
        /// </summary>
        /// <param name="token">The JWT token to process.</param>
        /// <param name="isRetry">Indicates whether this is a retry attempt after a failed validation due to expired settings.</param>
        /// <returns>
        /// A task representing the operation, returning a <see cref="ClaimsIdentity"/> containing the user's identity details.
        /// </returns>
        /// <exception cref="NotFoundException">Thrown if the JWT settings cannot be retrieved.</exception>
        /// <exception cref="SecurityTokenExpiredException">Thrown if the provided token has expired.</exception>
        /// <exception cref="UnhandledException">Thrown if an unexpected error occurs during token processing.</exception>
        /// <remarks>
        /// This method validates the token against issuer, audience, expiration, and signature key. If the token is valid,
        /// it extracts claims and returns the associated identity. If the validation fails due to an expired token and retrying
        /// is allowed, it refreshes the cached JWT settings and attempts validation again.
        /// </remarks>

        private async Task<ClaimsIdentity> GetIdentityFromTokenAsync(string token, bool isRetry = false)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                };

                JwtSettings jwtSettings = await GetJwtSettingsAsync() ?? throw new NotFoundException($"{nameof(JwtSettings)} is not found or not setup!");

                tokenValidationParameters.ValidateTokenReplay = true;
                tokenValidationParameters.ValidAudience = jwtSettings.Audience;
                tokenValidationParameters.ValidIssuer = jwtSettings.Issuer;
                tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)) { KeyId = jwtSettings.Kid };

                var tokenDecoder = new JwtSecurityTokenHandler();

                var jwtSecurityToken = (JwtSecurityToken)tokenDecoder.ReadToken(token);
                string tokenRaw = jwtSettings.EnableIdentityUrl ? token : jwtSecurityToken.RawData;

                ClaimsPrincipal principal = tokenDecoder.ValidateToken(tokenRaw, tokenValidationParameters, out _);

                return principal.Identities.First();
            }
            catch (SecurityTokenExpiredException ex)
            {
                string errorMessage = "Call to {0} failed with {1}";
                _logger.LogError(ex, errorMessage, nameof(GetIdentityFromTokenAsync), nameof(SecurityTokenExpiredException));

                if (!isRetry)
                {
                    RemoveJwtSettingsCache();
                    return await GetIdentityFromTokenAsync(token, true);
                }

                throw new SecurityTokenExpiredException();
            }
            catch (Exception ex)
            {
                string errorMessage = "Call to {0} failed with message: {1}";
                _logger.LogError(ex, errorMessage, nameof(GetIdentityFromTokenAsync), ex.Message);

                throw new UnhandledException();
            }
        }

        /// <summary>
        /// Asynchronously retrieves JWT settings from the memory cache or fetches them from the identity service if not cached.
        /// </summary>
        /// <returns>
        /// A task representing the operation, returning the <see cref="JwtSettings"/> if available; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method first checks for JWT settings in the in-memory cache. If not found, it makes an HTTP request
        /// to the identity service to fetch the settings, encrypting the request for security. Once retrieved, the settings
        /// are cached for future use.
        /// </remarks>
        public async Task<JwtSettings?> GetJwtSettingsAsync()
        {
            try
            {
                ApplicationSettings appSettings = applicationSettings.Value;

                if (cache.TryGetValue(CacheKey, out JwtSettings? cacheSettings))
                {
                    return cacheSettings;
                }

                using HttpClient client = httpClientFactory.CreateClient();

                string password = AesEncryptionHelper.Encrypt(appSettings.Password, appSettings.Password);

                StringContent? content = new StringContent(JsonSerializer.Serialize(new { Password = password }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri: $"{IdentityUrl}settings/jwt", content);

                if (response.IsSuccessStatusCode)
                {
                    string settingsJson = await response.Content.ReadAsStringAsync();
                    JwtSettings? jwtSettings = JsonSerializer.Deserialize<JwtSettings>(settingsJson, JsonSerializerOptions);

                    if (jwtSettings != null)
                    {
                        cache.Set(CacheKey, jwtSettings, TimeSpan.FromDays(365));

                        return jwtSettings;
                    }
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return default;
            }
        }

        /// <summary>
        /// Removes the cached JWT settings from memory to force re-fetching from the identity service.
        /// </summary>
        /// <remarks>
        /// This method is typically called when an authentication failure occurs due to outdated JWT settings.
        /// It ensures that subsequent authentication requests retrieve fresh settings from the identity service.
        /// </remarks>
        public void RemoveJwtSettingsCache()
        {
            cache.Remove(CacheKey);
        }
    }
}