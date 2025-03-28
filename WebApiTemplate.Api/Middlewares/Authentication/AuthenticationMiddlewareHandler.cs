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
using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Helpers;

namespace WebApiTemplate.Api.Middlewares.Authentication
{
    /// <summary>
    /// Middleware handler for authentication, responsible for validating JWT tokens.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    /// <param name="applicationSettings">The application settings.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="cache">The memory cache.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public class AuthenticationMiddlewareHandler(
        IOptionsMonitor<AuthenticationMiddlewareOptions> options,
        IOptions<ApplicationSettings> applicationSettings,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory) : AuthenticationHandler<AuthenticationMiddlewareOptions>(options, logger, encoder)
    {
        /// <summary>
        /// Logger instance for authentication events.
        /// </summary>
        private readonly ILogger _logger = logger.CreateLogger<AuthenticationMiddlewareHandler>();

        /// <summary>
        /// The identity service URL.
        /// </summary>
        public static string? IdentityUrl { get; set; }

        /// <summary>
        /// Cache key for JWT settings.
        /// </summary>
        private const string CacheKey = nameof(JwtSettings);

        /// <summary>
        /// The unauthorized access message.
        /// </summary>
        private const string Unauthorized = nameof(Unauthorized);

        /// <summary>
        /// The Bearer token prefix.
        /// </summary>
        private const string Bearer = nameof(Bearer);

        /// <summary>
        /// Default JSON serialization options.
        /// </summary>
        private readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Optional: ignore case in property names
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Handles authentication for incoming requests.
        /// </summary>
        /// <returns>An authentication result indicating success or failure.</returns>
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
        /// Validates the provided JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>An authentication result.</returns>
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
        /// Retrieves identity information from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <param name="isRetry">Indicates if this is a retry attempt.</param>
        /// <returns>A task representing the operation, containing the claims identity.</returns>
        /// <exception cref="NotFoundException">Thrown if JWT settings are not found.</exception>
        /// <exception cref="SecurityTokenExpiredException">Thrown if the token is expired.</exception>
        /// <exception cref="UnhandledException">Thrown for unhandled exceptions.</exception>
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
        /// Retrieves JWT settings from memory cache or the identity service.
        /// </summary>
        /// <returns>A task representing the operation, containing the JWT settings.</returns>
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

                var passwordDto = new PasswordDto()
                {
                    Password = password
                };

                var content = new StringContent(JsonSerializer.Serialize(passwordDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{IdentityUrl}settings/jwt", content);

                if (response.IsSuccessStatusCode)
                {
                    string settingsJson = await response.Content.ReadAsStringAsync();
                    JwtSettings? jwtSettings = JsonSerializer.Deserialize<JwtSettings>(settingsJson, JsonSerializerOptions);

                    if (jwtSettings != null)
                    {
                        cache.Set(CacheKey, jwtSettings, TimeSpan.FromDays(1));

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
        /// Removes cached JWT settings from memory.
        /// </summary>
        public void RemoveJwtSettingsCache()
        {
            cache.Remove(CacheKey);
        }
    }
}