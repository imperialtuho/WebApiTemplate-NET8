using System.Net;

namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during web API execution.
    /// </summary>
    public sealed class WebApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Determines whether the error is transient and worth retrying.
        /// </summary>
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
        /// <param name="message">The message that describes the error.</param>
        public WebApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that caused this error.</param>
        public WebApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class with a specified HTTP status code and error message.
        /// </summary>
        /// <param name="status">The HTTP status code associated with the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public WebApiException(HttpStatusCode status, string message)
            : base(message)
        {
            StatusCode = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiException"/> class with a specified HTTP status code, error message, and inner exception.
        /// </summary>
        /// <param name="status">The HTTP status code associated with the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="exception">The inner exception that caused this error.</param>
        public WebApiException(HttpStatusCode status, string message, Exception exception)
            : base(message, exception)
        {
            StatusCode = status;
        }
    }
}