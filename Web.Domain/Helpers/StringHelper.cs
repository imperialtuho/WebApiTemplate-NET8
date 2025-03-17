using System.Text.RegularExpressions;

namespace Web.Domain.Helpers
{
    public static class StringHelper
    {
        public static string ToSlug(string input)
        {
            // Convert to lowercase
            string result = input.ToLower();

            // Replace spaces with hyphens
            result = result.Replace(' ', '-');

            // Remove any characters that are not alphanumeric or hyphens
            result = Regex.Replace(result, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens in a row
            result = Regex.Replace(result, @"-+", "-");

            // Trim any leading or trailing hyphens
            result = result.Trim('-');

            return result;
        }
    }
}