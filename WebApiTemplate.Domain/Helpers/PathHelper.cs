namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides utility methods for working with file paths and directories.
    /// This class includes methods for checking path validity, ensuring directory existence,
    /// combining paths for URLs, and working with specific folders like the "Assets" folder.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Determines whether a specified path is within a given root directory.
        /// This method checks if the <paramref name="newPath"/> is a subdirectory or the same as the <paramref name="rootPath"/>.
        /// </summary>
        /// <param name="rootPath">The root directory path to check against.</param>
        /// <param name="newPath">The path to check if it is within the root.</param>
        /// <returns><c>true</c> if <paramref name="newPath"/> is within or equal to <paramref name="rootPath"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method normalizes both paths to their full absolute form before performing the check.
        /// The comparison is case-insensitive.
        /// </remarks>
        public static bool IsPathWithinRoot(string rootPath, string newPath)
        {
            // Normalize paths
            string rootFullPath = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string newFullPath = Path.GetFullPath(newPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Check if the new path starts with the root path
            return newFullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures that the specified directory exists.
        /// If the directory does not exist, it will be created.
        /// </summary>
        /// <param name="path">The directory path to check or create.</param>
        /// <remarks>
        /// If the directory already exists, no action will be taken.
        /// </remarks>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Retrieves the path to the "Assets" folder relative to the executing assembly.
        /// If the folder does not exist, it will be created.
        /// </summary>
        /// <returns>The full path to the "Assets" folder.</returns>
        /// <remarks>
        /// This method calculates the "Assets" folder path relative to the application's base directory,
        /// and ensures that the folder exists by creating it if necessary.
        /// </remarks>
        public static string GetAssetsFolderPath()
        {
            // Calculate the path to the 'Assets' folder relative to the executing assembly
            string basePath = AppContext.BaseDirectory;
            string assetsFolderPath = Path.GetFullPath(Path.Combine(basePath, "../../../Assets"));

            // Creates path if the folder does not exist
            if (!Directory.Exists(assetsFolderPath))
            {
                Directory.CreateDirectory(assetsFolderPath);
            }

            return assetsFolderPath;
        }

        /// <summary>
        /// Validates the given folder name.
        /// Ensures that the name is not null, empty, or contains invalid characters.
        /// </summary>
        /// <param name="name">The folder name to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the name is invalid or contains forbidden characters.</exception>
        /// <remarks>
        /// The method checks for both null/empty values and invalid characters specific to file/folder naming conventions.
        /// </remarks>
        public static void ValidateFolderName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} is required and cannot be empty or whitespace.");
            }

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("The folder name contains invalid characters.");
            }
        }

        /// <summary>
        /// Combines multiple URL parts into a single, properly formatted URL.
        /// </summary>
        /// <param name="uriParts">An array of URI parts to combine.</param>
        /// <returns>A single string representing the combined URL.</returns>
        /// <remarks>
        /// The method ensures that each part is correctly formatted, trimming redundant slashes from the start or end of each part.
        /// It also replaces backslashes with forward slashes for URL compatibility.
        /// </remarks>
        public static string CombineUrl(params string[] uriParts)
        {
            string text = string.Empty;

            if (uriParts != null && uriParts.Length > 0)
            {
                char[] trimChars = ['\\', '/'];
                text = (uriParts[0] ?? string.Empty).TrimEnd(trimChars);

                for (int i = 1; i < uriParts.Length; i++)
                {
                    text = $"{text.TrimEnd(trimChars)}/{(uriParts[i] ?? string.Empty).TrimStart(trimChars)}";
                }
            }

            return text.Replace("\\", "/");
        }
    }
}