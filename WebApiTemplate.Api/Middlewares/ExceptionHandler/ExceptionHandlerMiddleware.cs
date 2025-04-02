using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Security.Authentication;
using WebApiTemplate.Domain.Exceptions;

namespace WebApiTemplate.Api.Middlewares.ExceptionHandler
{
    /// <summary>
    /// Middleware for handling exceptions in the API.
    /// </summary>
    /// <remarks>
    /// The <see cref="ExceptionHandlerMiddleware"/> provides centralized exception handling
    /// for the API by catching unhandled exceptions, logging the details, and sending appropriate
    /// error responses based on the type of exception thrown. It supports both development and production environments,
    /// and it customizes the response based on the exception type.
    /// </remarks>
    public static class ExceptionHandlerMiddleware
    {
        /// <summary>
        /// Custom exception handler middleware for handling and logging exceptions.
        /// </summary>
        /// <param name="isDevelopment">Boolean indicating whether the environment is development.</param>
        /// <param name="logger">The logger instance used to log the exceptions.</param>
        /// <returns>An action to be executed within the <see cref="IApplicationBuilder"/> pipeline that handles exceptions.</returns>
        /// <remarks>
        /// This method will catch unhandled exceptions in the application, log the details of the exception,
        /// and return a structured response to the client with the exception's details.
        /// Depending on the exception type, it will return the corresponding HTTP status code and error message.
        /// In development environments, more detailed information, such as stack trace and path, will be included.
        /// </remarks>
        public static Action<IApplicationBuilder> CustomExceptionHandlerMiddleware(bool isDevelopment, ILogger logger)
        {
            return applicationBuilder => applicationBuilder.Run(async httpContext =>
            {
                IExceptionHandlerPathFeature? exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                Exception specificException = exceptionHandlerPathFeature!.Error;
                logger.LogError("Api endpoint {Path} failed with unhandled exception: {SpecificException}", exceptionHandlerPathFeature.Path, specificException.Message);

                ExceptionHandlerResponse? responseObject = new ExceptionHandlerResponse
                {
                    Status = false,
                    ErrorCode = HttpStatusCode.InternalServerError,
                    Message = specificException.Message,
                    InnerExceptionMessage = specificException.InnerException?.Message,
                    Path = isDevelopment ? exceptionHandlerPathFeature.Path : string.Empty,
                    StackTrace = isDevelopment ? specificException.StackTrace : string.Empty,
                };

                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Determine the appropriate error code based on the exception type
                switch (specificException)
                {
                    case ArgumentException _:
                    case InvalidOperationException _:
                    case InvalidCredentialException _:
                        responseObject.ErrorCode = HttpStatusCode.BadRequest;
                        statusCode = HttpStatusCode.BadRequest;
                        break;

                    case AuthenticationException _:
                        responseObject.ErrorCode = HttpStatusCode.Unauthorized;
                        statusCode = HttpStatusCode.Unauthorized;
                        break;

                    case ForbiddenException _:
                        responseObject.ErrorCode = HttpStatusCode.Forbidden;
                        statusCode = HttpStatusCode.Forbidden;
                        break;

                    case NotFoundException _:
                        responseObject.ErrorCode = HttpStatusCode.NotFound;
                        statusCode = HttpStatusCode.NotFound;
                        break;

                    case ConflictException _:
                        responseObject.ErrorCode = HttpStatusCode.Conflict;
                        statusCode = HttpStatusCode.Conflict;
                        break;

                    case UnhandledException _:
                        responseObject.ErrorCode = HttpStatusCode.InternalServerError;
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }

                string result = JsonConvert.SerializeObject(responseObject);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)statusCode;
                await httpContext.Response.WriteAsync(result);
            });
        }

        /// <summary>
        /// Checks whether the application is running in a production environment.
        /// </summary>
        /// <param name="env">The <see cref="IWebHostEnvironment"/> to check the environment of.</param>
        /// <param name="environmentName">The environment name to check against.</param>
        /// <returns>True if the application is running in a production environment, otherwise false.</returns>
        /// <remarks>
        /// This method determines whether the current environment is a production environment based on the environment name.
        /// It is used to configure different exception handling behaviors based on the environment.
        /// </remarks>
        public static bool IsProductionEnvironment(IWebHostEnvironment env, string environmentName)
        {
            return env.IsProduction() || environmentName.Contains("Production", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Response object for handling exception details in the middleware.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExceptionHandlerResponse"/> object encapsulates the details of the exception,
        /// such as the error code, message, inner exception message, stack trace, and correlation ID.
        /// This object is returned in the response to the client when an exception occurs.
        /// </remarks>
        public class ExceptionHandlerResponse
        {
            /// <summary>
            /// Unique identifier for the request, used for logging purposes.
            /// </summary>
            public Guid CorrelationId { get; set; } = Guid.NewGuid();

            /// <summary>
            /// The HTTP status code that corresponds to the exception's error.
            /// </summary>
            public HttpStatusCode ErrorCode { get; set; }

            /// <summary>
            /// The message of the inner exception, if any.
            /// </summary>
            public string? InnerExceptionMessage { get; set; }

            /// <summary>
            /// A descriptive message for the exception.
            /// </summary>
            public string? Message { get; set; }

            /// <summary>
            /// The path of the API endpoint that caused the exception.
            /// </summary>
            public string? Path { get; set; }

            /// <summary>
            /// The stack trace of the exception.
            /// </summary>
            public string? StackTrace { get; set; }

            /// <summary>
            /// A boolean indicating the success status of the request (false if an exception occurred).
            /// </summary>
            public bool Status { get; set; } = true;
        }
    }
}