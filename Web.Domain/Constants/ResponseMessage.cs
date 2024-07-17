namespace Web.Domain.Constants
{
    public static class ResponseMessage
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string UnknownError = "Unexpected error occured.";
        public const string NotFound = "Not Found.";
        public const string Conflict = "Conflict";
        public const string SystemError = "An error occurred in the system";
        public const string BadRequest = "The request is invalid";
        public const string InvalidOperation = "The operation is not allowed";
        public const string InvalidArgument = "The arguments are not valid";
        public const string InvalidCredentialException = "Invalid credential";
        public const string AuthenticationException = "Authentication failed";
        public const string EmptyOrNullException = "{0} can not be empty or null";
    }
}