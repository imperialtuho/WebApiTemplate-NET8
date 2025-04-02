using System.Net;

namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during web API execution.
    /// </summary>
    /// <remarks>
    /// This exception is used to handle API-specific errors, including status code tracking and transient error detection.
    /// </remarks>
    public sealed class WebApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the error.
        /// </summary>
        /// <remarks>
        /// The HTTP status code provides information about the type of error that occurred during API execution.
        /// </remarks>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Determines whether the error is transient and worth retrying.
        /// </summary>
        /// <remarks>
        /// A transient error may resolve itself upon retry, typically due to temporary service disruptions.
        /// </remarks>
        public bool IsTransientError => TransientStatusCodes.Contains(StatusCode);

        /// <summary>
        /// HTTP status codes that indicate a transient error, which might be worth retrying.
        /// </summary>
        private static readonly HashSet<HttpStatusCode> TransientStatusCodes = new()
        {
            HttpStatusCode.RequestTimeout,        // 408
            HttpStatusCode.InternalServerError,  // 500
            HttpStatusCode.BadGateway,          // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout     // 504
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class.
        /// </summary>
        public WebApiException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the API error.</param>
        /// <remarks>
        /// This constructor allows specifying a custom error message to provide more context about the API failure.
        /// </remarks>
        public WebApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class with a specified HTTP status code and error message.
        /// </summary>
        /// <param name="status">The HTTP status code associated with the error.</param>
        /// <param name="message">The message that describes the API error.</param>
        /// <remarks>
        /// This constructor allows specifying both an error message and a status code to categorize API failures.
        /// </remarks>
        public WebApiException(HttpStatusCode status, string message)
            : base(message)
        {
            StatusCode = status;
        }
    }
}