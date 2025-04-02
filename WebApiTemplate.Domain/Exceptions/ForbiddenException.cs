namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a user attempts to perform an action they are not authorized for.
    /// </summary>
    /// <remarks>
    /// This exception is typically used when an operation is forbidden due to insufficient permissions,
    /// such as accessing a restricted resource or performing an unauthorized action.
    /// </remarks>
    public sealed class ForbiddenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates a forbidden exception with a default error message indicating insufficient permissions.
        /// </remarks>
        public ForbiddenException()
            : base("You don't have permission to perform this action")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the forbidden error.</param>
        /// <remarks>
        /// This constructor allows specifying a custom error message to provide more details about the authorization failure.
        /// </remarks>
        public ForbiddenException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">The message that describes the forbidden error.</param>
        /// <param name="innerException">The underlying exception that caused the forbidden error.</param>
        /// <remarks>
        /// This constructor provides additional context by linking the forbidden exception to another exception,
        /// helping to trace the root cause of the authorization failure.
        /// </remarks>
        public ForbiddenException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}