using Web.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Web.Domain.Constants;

namespace Web.Infrastructure.Repositories.ExternalProviders
{
    public abstract class WebApiClient
    {
        protected readonly ILogger<WebApiClient> _logger;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _defaultJsonOptions;

        protected WebApiClient(
            ILogger<WebApiClient> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;

            // Initialize default JSON serializer options
            _defaultJsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        protected async Task<T?> SendAsync<T>(
            HttpMethod method,
            Uri requestUri,
            object? content = null,
            Dictionary<string, string>? headers = null,
            string description = "",
            string contentType = HttpContentTypeConstants.Json,
            bool ignoreChecking = false,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            using HttpClient? httpClient = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(method, requestUri);

            jsonOptions ??= _defaultJsonOptions;

            if (content != null)
            {
                request.Content = contentType switch
                {
                    HttpContentTypeConstants.Json => new StringContent(
                        content is string strContent ? strContent : JsonSerializer.Serialize(content, jsonOptions),
                        Encoding.UTF8,
                        contentType),

                    HttpContentTypeConstants.TextXml => new StringContent(SerializeToXml(content), Encoding.UTF8, contentType),

                    HttpContentTypeConstants.UrlEncoded => new StringContent(
                        SerializeToFormUrlEncoded(content),
                        Encoding.UTF8,
                        contentType),

                    _ => throw new NotSupportedException($"Content type '{contentType}' is not supported."),
                };
            }

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            try
            {
                using HttpResponseMessage? response = await httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleUnsuccessfulResponse(response, description, ignoreChecking);
                }

                string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return typeof(T) == typeof(string)
                    ? (T?)(object)responseString
                    : Deserialize<T>(responseString, jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Method} call to {Uri}. Description: {Description}", method, requestUri, description);
                throw new UnhandledException($"Failed to complete {method} request to {requestUri}: {ex.Message}");
            }
        }

        private static async Task HandleUnsuccessfulResponse(HttpResponseMessage response, string description, bool ignoreChecking)
        {
            string content = await response.Content.ReadAsStringAsync();
            string warning = $"Call failed: {description}. Status: {response.StatusCode}, Response: {content}";

            if (!ignoreChecking)
            {
                throw new WebApiException(response.StatusCode, warning);
            }
        }

        private T? Deserialize<T>(string data, JsonSerializerOptions? options)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return default;
            }

            if (data.TrimStart().StartsWith('<'))
            {
                var serializer = new XmlSerializer(typeof(T));
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
                return (T?)serializer.Deserialize(stream);
            }

            return JsonSerializer.Deserialize<T>(data, options);
        }

        private static string SerializeToXml(object content)
        {
            var serializer = new XmlSerializer(content.GetType());
            using var stream = new StringWriter();
            serializer.Serialize(stream, content);

            return stream.ToString();
        }

        private static string SerializeToFormUrlEncoded(object content)
        {
            if (content is not IDictionary<string, object> dictionary)
            {
                dictionary = content.GetType()
                    .GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(content) ?? "");
            }

            return string.Join("&", dictionary.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? "")}"));
        }

        /// <summary>
        /// Sends HTTP GET request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="description"></param>
        /// <param name="contentType"></param>
        /// <param name="ignoreChecking"></param>
        /// <param name="jsonOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return data type of T on successfully performing HTTP GET action.</returns>

        public Task<T?> GetAsync<T>(Uri uri, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<T>(HttpMethod.Get, uri, null, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends HTTP POST request.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="description"></param>
        /// <param name="contentType"></param>
        /// <param name="ignoreChecking"></param>
        /// <param name="jsonOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return result data typeof(TResponse) on successfully performing HTTP POST action.</returns>
        public Task<TResponse?> PostAsync<TRequest, TResponse>(Uri uri, TRequest content, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<TResponse>(HttpMethod.Post, uri, content, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends HTTP DELETE request.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="description"></param>
        /// <param name="contentType"></param>
        /// <param name="ignoreChecking"></param>
        /// <param name="jsonOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse?> PutAsync<TRequest, TResponse>(Uri uri, TRequest content, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<TResponse>(HttpMethod.Put, uri, content, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends HTTP DELETE request.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="description"></param>
        /// <param name="ignoreChecking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task DeleteAsync(Uri uri, Dictionary<string, string>? headers = null, string description = "", bool ignoreChecking = false, CancellationToken cancellationToken = default) =>
            SendAsync<object>(HttpMethod.Delete, uri, headers: headers, description: description, ignoreChecking: ignoreChecking, cancellationToken: cancellationToken);

        protected Dictionary<string, string> AuthorizationRequestHeader()
        {
            return new Dictionary<string, string>()
            {
                { nameof(Authorization), $"Bearer {AccessToken()}"}
            };
        }

        protected string AccessToken()
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == $"{nameof(AccessToken)}")?.Value ?? string.Empty;
            }

            return string.Empty;
        }
    }
}