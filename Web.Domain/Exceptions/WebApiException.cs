using System.Net;

namespace Web.Domain.Exceptions
{
    public sealed class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsTransientError
        {
            get { return StatusCodesWorthRetrying.Contains(StatusCode); }
        }

        private static readonly List<HttpStatusCode> StatusCodesWorthRetrying = new List<HttpStatusCode>
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        public WebApiException()
        {
        }

        public WebApiException(string message)
            : base(message)
        {
        }

        public WebApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public WebApiException(HttpStatusCode status, string message) : base(message)
        {
            StatusCode = status;
        }

        public WebApiException(HttpStatusCode status, string message, Exception exception) : base(message, exception)
        {
            StatusCode = status;
        }
    }
}