using System.Text.RegularExpressions;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for string manipulation.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Converts a string into a URL-friendly slug.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <returns>A slugified version of the input string.</returns>
        public static string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Convert to lowercase
            string result = input.ToLowerInvariant();

            // Replace spaces with hyphens
            result = result.Replace(' ', '-');

            // Remove any characters that are not alphanumeric or hyphens
            result = Regex.Replace(result, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens in a row
            result = Regex.Replace(result, @"-+", "-");

            // Trim any leading or trailing hyphens
            return result.Trim('-');
        }
    }
}