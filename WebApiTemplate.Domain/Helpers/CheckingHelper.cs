namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for data validation.
    /// </summary>
    public static class CheckingHelper
    {
        /// <summary>
        /// Checks if the given string is a valid Base64-encoded string.
        /// </summary>
        /// <param name="source">The string to check.</param>
        /// <returns>True if the string is valid Base64, otherwise false.</returns>
        public static bool IsBase64String(string source)
        {
            Span<byte> buffer = new Span<byte>(new byte[source.Length]);

            return Convert.TryFromBase64String(source, buffer, out _);
        }
    }
}