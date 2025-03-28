using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using WebApiTemplate.Domain.Constants;
using WebApiTemplate.Domain.Exceptions;

namespace WebApiTemplate.Infrastructure.Repositories.ExternalProviders
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

        /// <summary>
        /// Sends an asynchronous HTTP request and processes the response.
        /// </summary>
        /// <typeparam name="T">The expected response type.</typeparam>
        /// <param name="method">HTTP method (GET, POST, etc.).</param>
        /// <param name="requestUri">The API endpoint.</param>
        /// <param name="content">The request payload (optional).</param>
        /// <param name="headers">Custom request headers.</param>
        /// <param name="description">Description for logging purposes.</param>
        /// <param name="contentType">The content type of the request.</param>
        /// <param name="ignoreChecking">Whether to bypass error handling.</param>
        /// <param name="jsonOptions">Custom JSON serializer options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response.</returns>
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

        /// <summary>
        /// Handles unsuccessful HTTP responses by logging the error and throwing an exception if required.
        /// </summary>
        /// <param name="response">The HTTP response message received from the API.</param>
        /// <param name="description">A brief description of the request for logging purposes.</param>
        /// <param name="ignoreChecking">If true, the method will not throw an exception for unsuccessful responses.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="WebApiException">Thrown if the response is not successful and <paramref name="ignoreChecking"/> is false.</exception>
        private static async Task HandleUnsuccessfulResponse(HttpResponseMessage response, string description, bool ignoreChecking)
        {
            string content = await response.Content.ReadAsStringAsync();
            string warning = $"Call failed: {description}. Status: {response.StatusCode}, Response: {content}";

            if (!ignoreChecking)
            {
                throw new WebApiException(response.StatusCode, warning);
            }
        }

        /// <summary>
        /// Deserializes a given string into an object of type <typeparamref name="T"/>.
        /// Automatically detects whether the data is in XML or JSON format and deserializes accordingly.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="data">The serialized string data.</param>
        /// <param name="options">Optional JSON serializer options for deserialization.</param>
        /// <returns>An instance of <typeparamref name="T"/> if deserialization is successful; otherwise, <c>null</c>.</returns>
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

        /// <summary>
        /// Serializes an object into an XML string representation.
        /// </summary>
        /// <param name="content">The object to be serialized.</param>
        /// <returns>A string containing the XML representation of the object.</returns>
        private static string SerializeToXml(object content)
        {
            var serializer = new XmlSerializer(content.GetType());
            using var stream = new StringWriter();
            serializer.Serialize(stream, content);

            return stream.ToString();
        }

        /// <summary>
        /// Serializes an object into a URL-encoded form string.
        /// Converts the object into key-value pairs and encodes them in a format suitable for HTTP form submission.
        /// </summary>
        /// <param name="content">The object to be serialized.</param>
        /// <returns>A URL-encoded string representing the object's key-value pairs.</returns>
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
        /// Sends an asynchronous HTTP GET request to the specified URI and returns the response deserialized into the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the response content will be deserialized.</typeparam>
        /// <param name="uri">The target URI for the GET request.</param>
        /// <param name="headers">Optional HTTP headers to include in the request.</param>
        /// <param name="description">A description of the request, used for logging or debugging purposes.</param>
        /// <param name="contentType">The content type expected in the response (default is JSON).</param>
        /// <param name="ignoreChecking">Indicates whether to ignore response validation checks.</param>
        /// <param name="jsonOptions">Optional JSON serialization options for deserialization.</param>
        /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an instance of <typeparamref name="T"/>
        /// if the request is successful, or null if the response is empty.
        /// </returns>
        public Task<T?> GetAsync<T>(Uri uri, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<T>(HttpMethod.Get, uri, null, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends an asynchronous HTTP POST request to the specified URI with the given content and returns the response deserialized into the specified type.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request body content to be sent.</typeparam>
        /// <typeparam name="TResponse">The type to which the response content will be deserialized.</typeparam>
        /// <param name="uri">The target URI for the POST request.</param>
        /// <param name="content">The request body content to be sent.</param>
        /// <param name="headers">Optional HTTP headers to include in the request.</param>
        /// <param name="description">A description of the request, used for logging or debugging purposes.</param>
        /// <param name="contentType">The content type of the request body (default is JSON).</param>
        /// <param name="ignoreChecking">Indicates whether to ignore response validation checks.</param>
        /// <param name="jsonOptions">Optional JSON serialization options for deserialization.</param>
        /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an instance of <typeparamref name="TResponse"/>
        /// if the request is successful, or null if the response is empty.
        /// </returns>
        public Task<TResponse?> PostAsync<TRequest, TResponse>(Uri uri, TRequest content, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<TResponse>(HttpMethod.Post, uri, content, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends an asynchronous HTTP PUT request to the specified URI with the given content and returns the response deserialized into the specified type.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request body content to be sent.</typeparam>
        /// <typeparam name="TResponse">The type to which the response content will be deserialized.</typeparam>
        /// <param name="uri">The target URI for the PUT request.</param>
        /// <param name="content">The request body content to be sent.</param>
        /// <param name="headers">Optional HTTP headers to include in the request.</param>
        /// <param name="description">A description of the request, used for logging or debugging purposes.</param>
        /// <param name="contentType">The content type of the request body (default is JSON).</param>
        /// <param name="ignoreChecking">Indicates whether to ignore response validation checks.</param>
        /// <param name="jsonOptions">Optional JSON serialization options for deserialization.</param>
        /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an instance of <typeparamref name="TResponse"/>
        /// if the request is successful, or null if the response is empty.
        /// </returns>
        public Task<TResponse?> PutAsync<TRequest, TResponse>(Uri uri, TRequest content, Dictionary<string, string>? headers = null, string description = "", string contentType = HttpContentTypeConstants.Json, bool ignoreChecking = false, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default) =>
            SendAsync<TResponse>(HttpMethod.Put, uri, content, headers, description, contentType, ignoreChecking, jsonOptions, cancellationToken);

        /// <summary>
        /// Sends an asynchronous HTTP DELETE request to the specified URI.
        /// </summary>
        /// <param name="uri">The target URI for the DELETE request.</param>
        /// <param name="headers">Optional HTTP headers to include in the request.</param>
        /// <param name="description">A description of the request, used for logging or debugging purposes.</param>
        /// <param name="ignoreChecking">Indicates whether to ignore response validation checks.</param>
        /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. If the request is successful, the response is processed,
        /// but no specific return value is expected.
        /// </returns>
        public Task DeleteAsync(Uri uri, Dictionary<string, string>? headers = null, string description = "", bool ignoreChecking = false, CancellationToken cancellationToken = default) =>
            SendAsync<object>(HttpMethod.Delete, uri, headers: headers, description: description, ignoreChecking: ignoreChecking, cancellationToken: cancellationToken);

        /// <summary>
        /// Generates an authorization header dictionary containing a Bearer token.
        /// </summary>
        /// <returns>
        /// A dictionary with the "Authorization" header set to a Bearer token retrieved from the current user's claims.
        /// </returns>
        protected Dictionary<string, string> AuthorizationRequestHeader()
        {
            return new Dictionary<string, string>()
            {
                { nameof(Authorization), $"Bearer {AccessToken()}"}
            };
        }

        /// <summary>
        /// Retrieves the access token from the current HTTP context.
        /// </summary>
        /// <returns>
        /// The access token as a string if available; otherwise, an empty string.
        /// </returns>
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