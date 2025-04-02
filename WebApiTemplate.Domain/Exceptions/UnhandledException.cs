namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an unexpected or unhandled error occurs during application execution.
    /// </summary>
    /// <remarks>
    /// This exception is typically used to capture unexpected errors that were not explicitly handled,
    /// allowing for improved error logging and debugging.
    /// </remarks>
    public sealed class UnhandledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates an unhandled exception with a default error message indicating an unexpected error.
        /// </remarks>
        public UnhandledException()
            : base("An unexpected error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the unhandled error.</param>
        /// <remarks>
        /// This constructor allows specifying a custom error message to provide more details about the unexpected error.
        /// </remarks>
        public UnhandledException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">The message that describes the unhandled error.</param>
        /// <param name="innerException">The underlying exception that caused the unhandled error.</param>
        /// <remarks>
        /// This constructor provides additional context by linking the unhandled exception to another exception,
        /// helping to trace the root cause of the issue.
        /// </remarks>
        public UnhandledException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}