using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiTemplate.Application.Configurations.Settings;
using WebApiTemplate.Application.Dtos.Author;
using WebApiTemplate.Application.Interfaces.ExternalProviders;
using WebApiTemplate.Domain.Exceptions;

namespace WebApiTemplate.Infrastructure.Repositories.ExternalProviders.IdentityApi
{
    public class IdentityApi(ILogger<IdentityApi> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<IdentityApiSettings> identityApiSettings) : WebApiClient(logger, httpClientFactory, httpContextAccessor), IIdentityApi
    {
        private readonly IdentityApiSettings _identityApiSettings = identityApiSettings.Value;

        /// <summary>
        /// Get user by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{UserDto}.</returns>
        public async Task<AuthorDto?> GetUserByIdAsync(string id)
        {
            string message = $"Call to {nameof(GetUserByIdAsync)}. with id: {id}";
            logger.LogInformation(message);

            try
            {
                string providerName = _identityApiSettings.ProviderName;
                string requestUri = $"{_identityApiSettings.Url}";

                FluentUriBuilder request = new FluentUriBuilder(requestUri).AppendPath("Users").AppendPath(id);

                var response = await GetAsync<AuthorDto>(
                    uri: request.Uri,
                    headers: AuthorizationRequestHeader(),
                    description: $"{providerName} {nameof(GetUserByIdAsync)} {requestUri}",
                    cancellationToken: CancellationToken.None);

                return response;
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetUserByIdAsync)} Unknown error encountered";
                logger.LogError(ex, message);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Get users by ids async.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task{UserDto}.</returns>
        public async Task<IList<AuthorDto>?> GetUserByIdsAsync(IList<string> ids)
        {
            string message = $"Call to {nameof(GetUserByIdsAsync)} with ids: {string.Join(",", ids)}";
            logger.LogInformation(message);

            try
            {
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

                // Send the HTTP request and get the response
                var response = await GetAsync<IList<AuthorDto>>(
                    uri: request.Uri,
                    headers: AuthorizationRequestHeader(),
                    description: $"{providerName} {nameof(GetUserByIdsAsync)} {requestUri}",
                    cancellationToken: CancellationToken.None);

                return response;
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetUserByIdsAsync)} encountered an error.";
                logger.LogError(ex, message);

                throw new UnhandledException($"{message} Details: {ex.Message}");
            }
        }
    }
}