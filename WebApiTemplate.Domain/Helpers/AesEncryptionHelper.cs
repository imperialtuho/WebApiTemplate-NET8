using System.Security.Cryptography;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides AES-256 encryption and decryption using a password-based key derivation.
    /// The key is derived from the password using PBKDF2 (Rfc2898DeriveBytes) and a random salt.
    /// </summary>
    public static class AesEncryptionHelper
    {
        private const int SaltSize = 16; // 128-bit salt
        private const int Iterations = 100000; // The number of iterations for key derivation
        private const int AesKeySize = 32; // AES-256 key size (256 bits = 32 bytes)
        private const int IvSize = 16; // AES IV is always 16 bytes (128-bit block size)

        /// <summary>
        /// Encrypts a plain text string using AES-256 encryption with a password-derived key.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <param name="password">The password used to derive the encryption key.</param>
        /// <returns>A Base64-encoded encrypted string that includes the salt and IV.</returns>
        /// <remarks>
        /// The encrypted data consists of the salt, the IV, and the cipher text, all encoded in Base64.
        /// This ensures that both the salt and IV are included in the encrypted result,
        /// which are necessary for decryption.
        /// </remarks>
        public static string Encrypt(string plainText, string password)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt); // Generate a random salt

            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(AesKeySize); // Derive a 256-bit key

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.GenerateIV(); // Generate a random IV

            using var memoryStream = new MemoryStream();
            memoryStream.Write(salt, 0, salt.Length); // Store the salt at the beginning
            memoryStream.Write(aes.IV, 0, IvSize); // Store the IV after the salt

            // Create a CryptoStream to perform the encryption
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            // Return the encrypted data in Base64 format
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// Decrypts an AES-256 encrypted string using a password-derived key.
        /// </summary>
        /// <param name="cipherText">The Base64-encoded encrypted string.</param>
        /// <param name="password">The password used to derive the decryption key.</param>
        /// <returns>The decrypted plain text.</returns>
        /// <remarks>
        /// The cipher text should be Base64-encoded and include the salt and IV used during encryption.
        /// The method will extract these components, derive the decryption key, and then decrypt the data.
        /// </remarks>
        public static string Decrypt(string cipherText, string password)
        {
            byte[] bytes = Convert.FromBase64String(cipherText); // Decode the Base64 cipher text

            using MemoryStream memoryStream = new MemoryStream(bytes);

            // Extract the salt and IV from the beginning of the cipher text
            byte[] salt = new byte[SaltSize];
            _ = memoryStream.Read(salt, 0, SaltSize); // Read the salt

            byte[] iv = new byte[IvSize];
            _ = memoryStream.Read(iv, 0, IvSize); // Read the IV

            // Derive the AES key from the password using the salt
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] aesKey = deriveBytes.GetBytes(AesKeySize);

            using Aes aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv; // Set the IV for decryption
            aes.Mode = CipherMode.CBC;

            // Create a CryptoStream to perform the decryption
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);

            // Return the decrypted plain text
            return streamReader.ReadToEnd();
        }
    }
}