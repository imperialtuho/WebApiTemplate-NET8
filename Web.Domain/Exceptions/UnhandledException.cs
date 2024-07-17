namespace Web.Domain.Exceptions
{
    public sealed class UnhandledException : Exception
    {
        public UnhandledException(string? message = "Unexpected error occured.")
            : base(message)
        {
        }

        public UnhandledException(string? message, Exception? innerException) : base(message, innerException)
        { }
    }
}