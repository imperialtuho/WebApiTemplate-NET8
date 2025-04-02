namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for data validation, such as checking if a string is a valid Base64-encoded string.
    /// </summary>
    public static class CheckingHelper
    {
        /// <summary>
        /// Checks if the given string is a valid Base64-encoded string.
        /// </summary>
        /// <param name="source">The string to check. This should be a Base64-encoded string.</param>
        /// <returns>
        /// <c>true</c> if the string is a valid Base64-encoded string; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The method uses <see cref="Convert.TryFromBase64String"/> to attempt converting the string into a byte array.
        /// If the conversion is successful, the string is considered valid Base64.
        /// </remarks>
        public static bool IsBase64String(string source)
        {
            // Allocate a buffer to hold the decoded bytes
            Span<byte> buffer = new Span<byte>(new byte[source.Length]);

            // Try to decode the Base64 string
            return Convert.TryFromBase64String(source, buffer, out _);
        }
    }
}