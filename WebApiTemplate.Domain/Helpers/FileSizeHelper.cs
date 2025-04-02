namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for formatting file sizes in human-readable units.
    /// </summary>
    /// <remarks>
    /// This class includes a method that formats file sizes (in bytes) into a readable string, using common data units like B, KB, MB, GB, etc.
    /// It ensures that the file size is represented in the most appropriate unit based on the magnitude of the size.
    /// </remarks>
    public static class FileSizeHelper
    {
        /// <summary>
        /// Array of file size suffixes used for formatting the size into human-readable units.
        /// </summary>
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB", "GB (Geopbyte)" };

        /// <summary>
        /// Formats the file size from bytes to a more readable format, such as KB, MB, GB, etc.
        /// </summary>
        /// <param name="bytes">The file size in bytes to be formatted.</param>
        /// <returns>A string representing the file size in an appropriate unit, rounded to two decimal places.</returns>
        /// <exception cref="ArgumentException">Thrown if the file size is negative.</exception>
        /// <remarks>
        /// This method divides the file size by powers of 1024 to convert it into a larger unit, such as KB, MB, or GB, depending on the size.
        /// It ensures that the result is presented with two decimal places for better readability.
        /// </remarks>
        public static string FormatFileSize(long bytes)
        {
            if (bytes < 0) throw new ArgumentException("File size cannot be negative.", nameof(bytes));  // Throws an exception if the bytes are negative.
            if (bytes == 0) return "0 B";  // If file size is zero, return "0 B".

            // Calculate the order of magnitude based on logarithm, to determine the appropriate size suffix.
            int order = (int)Math.Floor(Math.Log(bytes, 1024));
            order = Math.Min(order, SizeSuffixes.Length - 1); // Prevents out-of-bounds access for larger values.

            // Adjust the size based on the calculated order, using the 1024 power scale.
            double adjustedSize = bytes / Math.Pow(1024, order);
            return $"{adjustedSize:F2} {SizeSuffixes[order]}";  // Format the size with two decimal places and append the appropriate suffix.
        }
    }
}