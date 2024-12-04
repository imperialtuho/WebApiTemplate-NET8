namespace Web.Domain.Exceptions
{
    public sealed class ForbiddenException : Exception
    {
        public ForbiddenException(string? message = "You don't have permission to perform this action") : base(message)
        {
        }

        public ForbiddenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}