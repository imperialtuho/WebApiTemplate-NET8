using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiTemplate.Application.Configurations.Settings;
using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Application.Interfaces.ExternalProviders;
using WebApiTemplate.Domain.Exceptions;

namespace WebApiTemplate.Infrastructure.Repositories.ExternalProviders.IdentityApi
{
    /// <summary>
    /// A service responsible for interacting with the Identity API to retrieve user information.
    /// This class extends <see cref="WebApiClient"/> to provide the base functionality for making HTTP requests.
    /// Implements <see cref="IIdentityApi"/> for defining the identity-related operations.
    /// </summary>
    public class IdentityApi : WebApiClient, IIdentityApi
    {
        private readonly IdentityApiSettings _identityApiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityApi"/> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{T}"/> instance for logging events.</param>
        /// <param name="httpClientFactory">Factory to create HTTP clients for making requests.</param>
        /// <param name="httpContextAccessor">Access to HTTP context, useful for headers and other context-related data.</param>
        /// <param name="identityApiSettings">Settings related to the Identity API, including URL and provider name.</param>
        public IdentityApi(
            ILogger<IdentityApi> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityApiSettings> identityApiSettings
        ) : base(logger, httpClientFactory, httpContextAccessor)
        {
            _identityApiSettings = identityApiSettings.Value;
        }

        /// <summary>
        /// Asynchronously retrieves a user by their ID from the Identity API.
        /// </summary>
        /// <param name="id">The user ID to retrieve.</param>
        /// <returns>A <see cref="Task{UserDto}"/> containing the user data, or null if the user is not found.</returns>
        /// <remarks>
        /// This method constructs the URI for the request, sends the request to the Identity API, and logs the activity.
        /// In case of an error, it logs the exception and throws an <see cref="UnhandledException"/>.
        /// </remarks>
        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            string message = $"Call to {nameof(GetUserByIdAsync)} with id: {id}";
            _logger.LogInformation(message);

            try
            {
                string providerName = _identityApiSettings.ProviderName;
                string requestUri = $"{_identityApiSettings.Url}";

                FluentUriBuilder request = new FluentUriBuilder(requestUri)
                    .AppendPath("Users")
                    .AppendPath(id);

                // Send the HTTP GET request and return the response
                var response = await GetAsync<UserDto>(
                    uri: request.Uri,
                    headers: AuthorizationRequestHeader(),
                    description: $"{providerName} {nameof(GetUserByIdAsync)} {requestUri}",
                    cancellationToken: CancellationToken.None
                );

                return response;
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetUserByIdAsync)} encountered an unknown error.";
                _logger.LogError(ex, message);
                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously retrieves multiple users by their IDs from the Identity API.
        /// </summary>
        /// <param name="ids">A list of user IDs to retrieve.</param>
        /// <returns>A <see cref="Task{IList{UserDto}}"/> containing the list of users.</returns>
        /// <remarks>
        /// This method validates that the list of IDs is not null or empty, constructs the request URI with query parameters,
        /// sends the request, and logs the activity. If an error occurs, it logs the exception and throws an <see cref="UnhandledException"/>.
        /// </remarks>
        public async Task<IList<UserDto>?> GetUserByIdsAsync(IList<string> ids)
        {
            string message = $"Call to {nameof(GetUserByIdsAsync)} with ids: {string.Join(",", ids)}";
            _logger.LogInformation(message);

            try
            {
                // Validate input
                if (ids == null || ids.Count == 0)
                {
                    throw new ArgumentException($"{nameof(ids)} cannot be null or empty.", nameof(ids));
                }

                string providerName = _identityApiSettings.ProviderName;
                string requestUri = $"{_identityApiSettings.Url}";

                // Build the request URI with query parameters
                var request = new FluentUriBuilder(requestUri)
                    .AppendPath("users")
                    .AddQueryParam(nameof(ids), string.Join(",", ids)); // Query param key: "ids", value: "id1,id2,id3"

                // Send the HTTP GET request and return the response
                var response = await GetAsync<IList<UserDto>>(
                    uri: request.Uri,
                    headers: AuthorizationRequestHeader(),
                    description: $"{providerName} {nameof(GetUserByIdsAsync)} {requestUri}",
                    cancellationToken: CancellationToken.None
                );

                return response;
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetUserByIdsAsync)} encountered an error.";
                _logger.LogError(ex, message);
                throw new UnhandledException($"{message} Details: {ex.Message}");
            }
        }
    }
}