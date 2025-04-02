namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a requested resource is not found.
    /// </summary>
    /// <remarks>
    /// This exception is typically used to indicate that a resource does not exist or cannot be located.
    /// It is often used in cases where an entity is requested but is unavailable in the system.
    /// </remarks>
    public sealed class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates a not found exception with a default error message indicating a missing resource.
        /// </remarks>
        public NotFoundException()
            : base("The requested resource was not found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the not found error.</param>
        /// <remarks>
        /// This constructor allows specifying a custom error message to provide more context about the missing resource.
        /// </remarks>
        public NotFoundException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">The message that describes the not found error.</param>
        /// <param name="innerException">The underlying exception that caused the not found error.</param>
        /// <remarks>
        /// This constructor provides additional context by linking the not found exception to another exception,
        /// helping to trace the root cause of the issue.
        /// </remarks>
        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}