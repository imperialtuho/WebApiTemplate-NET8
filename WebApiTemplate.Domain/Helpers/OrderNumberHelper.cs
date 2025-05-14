using System.Security.Cryptography;
using System.Text;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides functionality to generate and validate human-readable order numbers
    /// based on a combination of entity type, station identifier, current date, cryptographically secure random values,
    /// and a checksum for basic error detection.
    /// </summary>
    /// <remarks>
    /// This static helper class is intended for generating public-facing order numbers
    /// that are unique, non-sequential, and safe for external exposure, such as
    /// for customers, employees, and system integrations.
    ///
    /// The order numbers are generated in the following format:
    /// <code>
    /// {EntityType}-{StationId}-{yyyyMMdd}-{RandomDigits}-{Checksum}
    /// Example: R-X5-20250507-82314-96
    /// </code>
    ///
    /// The checksum is calculated as a modulo 100 of the sum of all alphabetic and numeric characters,
    /// helping to detect simple typos or transmission errors.
    ///
    /// This class does not guarantee absolute uniqueness across distributed systems
    /// and should be used alongside database constraints or additional collision checks if needed.
    /// </remarks>
    public static class OrderNumberHelper
    {
        /// <summary>
        /// Generates a new order number using the specified entity type and station identifier.
        /// </summary>
        /// <param name="entityType">
        /// A single-character or short string representing the type of entity (e.g., 'R' for order, 'C' for customer).
        /// </param>
        /// <param name="stationId">
        /// A string representing the identifier of the station, server, store code or system generating the order number (e.g., 'X5').
        /// </param>
        /// <returns>
        /// A fully formatted order number string that includes the entity type, station ID, current date, random component, and checksum.
        /// </returns>
        public static string GenerateCode(string entityType, string stationId)
        {
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomPart = GenerateSecureRandomNumber(5); // 5 digits
            string baseId = $"{entityType}-{stationId}-{datePart}-{randomPart}";
            string checksum = CalculateChecksum(baseId).ToString().PadLeft(2, '0');
            return $"{baseId}-{checksum}";
        }

        /// <summary>
        /// Validates a generated order number by recalculating its checksum and comparing it to the provided checksum.
        /// </summary>
        /// <param name="fullOrderNumber">
        /// The full order number string to validate, including all segments and the checksum.
        /// </param>
        /// <returns>
        /// <c>true</c> if the order number's checksum matches the calculated checksum; otherwise, <c>false</c>.
        /// </returns>
        public static bool ValidateCode(string fullOrderNumber)
        {
            if (string.IsNullOrWhiteSpace(fullOrderNumber))
                return false;

            string[]? parts = fullOrderNumber.Split('-');
            if (parts.Length != 5)
                return false;

            string withoutChecksum = $"{parts[0]}-{parts[1]}-{parts[2]}-{parts[3]}";
            string checksumStr = parts[4];

            if (!int.TryParse(checksumStr, out int providedChecksum))
                return false;

            int calculatedChecksum = CalculateChecksum(withoutChecksum);

            return providedChecksum == calculatedChecksum;
        }

        /// <summary>
        /// Calculates the checksum value for a given string based on a simple summation
        /// of alphanumeric characters, followed by a modulo 100 operation.
        /// </summary>
        /// <param name="input">
        /// The input string (excluding the checksum) for which to calculate the checksum.
        /// </param>
        /// <returns>
        /// An integer representing the checksum value (0-99).
        /// </returns>
        private static int CalculateChecksum(string input)
        {
            int sum = 0;
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                    sum += c - '0';
                else if (char.IsLetter(c))
                    sum += char.ToUpper(c) - 'A' + 1;
            }
            return sum % 100;
        }

        /// <summary>
        /// Generates a cryptographically secure random numeric string of a specified length.
        /// </summary>
        /// <param name="length">
        /// The number of digits to generate.
        /// </param>
        /// <returns>
        /// A string containing only numeric digits.
        /// </returns>
        private static string GenerateSecureRandomNumber(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");

            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            foreach (var b in bytes)
            {
                // Map byte (0-255) into 0-9
                sb.Append(b % 10);
            }

            return sb.ToString();
        }
    }
}