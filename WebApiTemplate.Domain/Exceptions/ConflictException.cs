namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a conflict occurs during application execution.
    /// </summary>
    /// <remarks>
    /// This exception is typically used to indicate that a request could not be processed due to a conflict with the current state of the resource,
    /// such as when attempting to create a duplicate entity.
    /// </remarks>
    public sealed class ConflictException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates an empty conflict exception without any additional details.
        /// </remarks>
        public ConflictException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the conflict error.</param>
        /// <remarks>
        /// This constructor allows specifying a custom error message to provide more context about the conflict.
        /// </remarks>
        public ConflictException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">The message that describes the conflict error.</param>
        /// <param name="innerException">The underlying exception that caused the conflict.</param>
        /// <remarks>
        /// This constructor provides additional context by linking the conflict exception to another exception,
        /// helping to trace the root cause of the issue.
        /// </remarks>
        public ConflictException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}