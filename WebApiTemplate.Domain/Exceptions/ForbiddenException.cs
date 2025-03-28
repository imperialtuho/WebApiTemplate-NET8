namespace WebApiTemplate.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a user attempts to perform an action they are not authorized for.
    /// </summary>
    public sealed class ForbiddenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a default error message.
        /// </summary>
        public ForbiddenException()
            : base("You don't have permission to perform this action")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ForbiddenException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and a reference to the inner exception that caused this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that caused the current exception.</param>
        public ForbiddenException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}