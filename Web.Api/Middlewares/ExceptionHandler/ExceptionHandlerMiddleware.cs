using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Security.Authentication;

namespace Web.Api.Middlewares.ExceptionHandler
{
    public static class ExceptionHandlerMiddleware
    {
        public static Action<IApplicationBuilder> CustomExceptionHandlerMiddleware(bool isDevelopment, ILogger logger)
        {
            return applicationBuilder => applicationBuilder.Run(async httpContext =>
            {
                var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                Exception specificException = exceptionHandlerPathFeature!.Error;
                logger.LogError("Api endpoint {Path} failed with unhandled exception: {SpecificException}", exceptionHandlerPathFeature.Path, specificException.Message);

                var responseObject = new ExceptionHandlerResponse
                {
                    Status = false,
                    ErrorCode = HttpStatusCode.InternalServerError,
                    Message = specificException.Message,
                    InnerExceptionMessage = specificException.InnerException?.Message,
                    Path = isDevelopment ? exceptionHandlerPathFeature.Path : string.Empty,
                    StackTrace = isDevelopment ? specificException.StackTrace : string.Empty,
                };

                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

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

        public static bool IsProductionEnvironment(IWebHostEnvironment env, string environmentName)
        {
            return env.IsProduction() || environmentName.Contains("Production", StringComparison.OrdinalIgnoreCase);
        }

        public class ExceptionHandlerResponse
        {
            /// <summary>
            /// Unique Identifier used by logging.
            /// </summary>
            public Guid CorrelationId { get; set; } = Guid.NewGuid();

            /// <summary>
            /// The ErrorCode.
            /// </summary>
            public HttpStatusCode ErrorCode { get; set; }

            /// <summary>
            /// The inner exception message.
            /// </summary>
            public string InnerExceptionMessage { get; set; }

            /// <summary>
            /// The Message.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// The error's Path.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// The StackTrace.
            /// </summary>
            public string StackTrace { get; set; }

            /// <summary>
            /// The Status.
            /// </summary>
            public bool Status { get; set; } = true;
        }
    }
}