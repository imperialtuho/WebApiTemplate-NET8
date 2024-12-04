using System.Text.RegularExpressions;

namespace Web.Domain.Helpers
{
    public static class ReadingTimeEstimatorHelper
    {
        private const int AverageWordsPerMinute = 200; // You can adjust this value

        public static int EstimateMinutesToRead(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return 1; // Default minimum reading time
            }

            // Split content into words
            int wordCount = CountWords(content);

            // Calculate reading time
            double minutes = (double)wordCount / AverageWordsPerMinute;

            // Round up to the nearest whole number
            return Math.Max(1, (int)Math.Ceiling(minutes)); // Ensure at least 1 minute
        }

        private static int CountWords(string content)
        {
            // Use regular expressions to match words
            var words = Regex.Matches(content, @"\b\w+\b");
            return words.Count;
        }
    }
}