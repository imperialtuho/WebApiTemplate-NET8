using System.Text.RegularExpressions;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides a utility to estimate the reading time of a given text based on word count.
    /// </summary>
    /// <remarks>
    /// This helper class calculates the estimated reading time by counting the number of words in the provided text
    /// and dividing it by the reading speed (in words per minute). It ensures that even for empty content or very short
    /// texts, the reading time is at least 1 minute. The default reading speed is set to 200 words per minute, but a custom
    /// speed can be specified.
    /// </remarks>
    public static class ReadingTimeEstimatorHelper
    {
        private const int DefaultWordsPerMinute = 200; // Default reading speed (words per minute)

        /// <summary>
        /// Estimates the time (in minutes) required to read the given content.
        /// </summary>
        /// <param name="content">The text content to analyze.</param>
        /// <param name="wordsPerMinute">Optional parameter to set a custom reading speed (default is 200 WPM).</param>
        /// <returns>The estimated reading time in minutes, ensuring at least 1 minute.</returns>
        /// <remarks>
        /// This method calculates the reading time by dividing the word count by the given words per minute.
        /// If the content is empty or whitespace, it returns a default of 1 minute.
        /// </remarks>
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
        /// Counts the number of words in the given text using a regular expression.
        /// </summary>
        /// <param name="content">The text to analyze.</param>
        /// <returns>The number of words in the content.</returns>
        /// <remarks>
        /// This method uses a regular expression to count words consisting of alphanumeric characters.
        /// It treats consecutive alphanumeric characters as a single word and ignores punctuation.
        /// </remarks>
        public static int CountWords(string content)
        {
            // Matches any word consisting of alphanumeric characters
            return Regex.Matches(content, @"\b\w+\b").Count;
        }
    }
}