namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an unexpected or unhandled error occurs during application execution.
    /// </summary>
    public sealed class UnhandledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class with a default error message.
        /// </summary>
        public UnhandledException()
            : base("An unexpected error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnhandledException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledException"/> class with a specified error message and a reference to the inner exception that caused this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that caused the current exception.</param>
        public UnhandledException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}