using System.Text.RegularExpressions;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for string manipulation.
    /// </summary>
    /// <remarks>
    /// This class includes methods for transforming and sanitizing strings for use in various contexts, such as creating
    /// URL-friendly slugs from user input. It provides methods to manipulate strings safely and predictably for different use cases.
    /// </remarks>
    public static class StringHelper
    {
        /// <summary>
        /// Converts a string into a URL-friendly slug.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <returns>A slugified version of the input string.</returns>
        /// <remarks>
        /// This method converts the input string to lowercase, replaces spaces with hyphens,
        /// removes non-alphanumeric characters (except hyphens), and trims any leading or trailing hyphens.
        /// The result is a URL-friendly string that can be used in web addresses.
        /// </remarks>
        public static string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Convert to lowercase to ensure consistency for URL
            string result = input.ToLowerInvariant();

            // Replace spaces with hyphens
            result = result.Replace(' ', '-');

            // Remove any characters that are not alphanumeric or hyphens
            result = Regex.Replace(result, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            result = Regex.Replace(result, @"-+", "-");

            // Trim any leading or trailing hyphens
            return result.Trim('-');
        }

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="source">The source string to compare.</param>
        /// <param name="comparor">The comparator string.</param>
        /// <param name="IsSensitiveCompared">If true, apply exact Unicode comparison. Otherwise, ignore case.</param>
        /// <returns><c>true</c> if the strings are equal based on the specified comparison; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method compares two strings using either a case-sensitive or case-insensitive comparison.
        /// By default, it performs a case-insensitive comparison.
        /// </remarks>
        public static bool IsEquals(string? source, string? comparor, bool IsSensitiveCompared = false)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(comparor))
            {
                return false;
            }

            if (IsSensitiveCompared)
            {
                return source.Equals(comparor, StringComparison.Ordinal);
            }

            return source.Equals(comparor, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Extracts the file extension from the full file name.
        /// </summary>
        /// <param name="fullName">The full name of the file, including the file extension.</param>
        /// <returns>The file extension, or an empty string if there is no extension.</returns>
        /// <remarks>
        /// This method checks for the last occurrence of a period (.) and returns the string following it.
        /// If no period is found or if the period is the last character, it returns an empty string.
        /// </remarks>
        public static string GetExtensionByFileFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty; // Return an empty string if Name is null, empty, or whitespace
            }

            int lastIndex = fullName.LastIndexOf('.');

            if (lastIndex == -1 || lastIndex == fullName.Length - 1)
            {
                return string.Empty; // Return an empty string if there's no '.' or it's the last character
            }

            return fullName[(lastIndex + 1)..]; // Return the extension after the last '.'
        }
    }
}