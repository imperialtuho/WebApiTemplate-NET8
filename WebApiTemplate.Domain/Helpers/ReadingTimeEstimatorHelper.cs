using System.Text.RegularExpressions;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides a utility to estimate the reading time of a given text.
    /// </summary>
    public static class ReadingTimeEstimatorHelper
    {
        private const int DefaultWordsPerMinute = 200; // Default reading speed

        /// <summary>
        /// Estimates the time (in minutes) required to read the given content.
        /// </summary>
        /// <param name="content">The text content to analyze.</param>
        /// <param name="wordsPerMinute">Optional parameter to set a custom reading speed (default is 200 WPM).</param>
        /// <returns>The estimated reading time in minutes (minimum of 1 minute).</returns>
        public static int EstimateMinutesToRead(string content, int wordsPerMinute = DefaultWordsPerMinute)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return 1; // Default minimum reading time
            }

            int wordCount = CountWords(content);
            double minutes = (double)wordCount / wordsPerMinute;

            return Math.Max(1, (int)Math.Ceiling(minutes)); // Ensure at least 1 minute
        }

        /// <summary>
        /// Counts the number of words in the given text.
        /// </summary>
        /// <param name="content">The text to analyze.</param>
        /// <returns>The number of words in the content.</returns>
        private static int CountWords(string content)
        {
            return Regex.Matches(content, @"\b\w+\b").Count;
        }
    }
}