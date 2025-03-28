using System.Security.Cryptography;

namespace WebApiTemplate.Domain.Helpers
{
    public static class AesEncryptionHelper
    {
        private const int SaltSize = 16; // 128-bit salt
        private const int Iterations = 100000;
        private const int AesKeySize = 32; // AES-256 key size
        private const int IvSize = 16; // AES IV is always 16 bytes (128-bit block size)

        public static string Encrypt(string plainText, string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt); // Generate a random salt

            var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(AesKeySize);

            using Aes? aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.GenerateIV(); // Generate a random IV

            using var memoryStream = new MemoryStream();
            memoryStream.Write(salt, 0, salt.Length); // Store the salt
            memoryStream.Write(aes.IV, 0, IvSize); // Store the IV

            using (CryptoStream? cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter? streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            byte[] bytes = Convert.FromBase64String(cipherText);

            using var memoryStream = new MemoryStream(bytes);
            byte[] salt = new byte[SaltSize];
            _ = memoryStream.Read(salt, 0, SaltSize); // Read the salt

            byte[] iv = new byte[IvSize];
            _ = memoryStream.Read(iv, 0, IvSize); // Read the IV

            var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] aesKey = deriveBytes.GetBytes(AesKeySize);

            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;

            using CryptoStream? cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader? streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}