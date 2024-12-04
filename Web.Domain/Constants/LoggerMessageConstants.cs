namespace Web.Domain.Constants
{
    /// <summary>
    /// Error log message with 3 params where {0} is the service's name, {1} is the function's name, and {2} is the log message.
    /// <br></br>
    /// For example: ErrorLogMessage: `Call to {0}-{1} error with exception message: {2}`.
    /// </summary>
    public static class LoggerMessageConstants
    {
        public const string ErrorLogMessage = "Call to {0}-{1} error with exception message: {2}";
        public const string WarningLogMessage = "Call to {0}-{1} with warning message: {2}";
        public const string InfomationLogMessage = "Call to {0}-{1} with information: {2}";
    }
}