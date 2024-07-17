using System.Security.Cryptography;
using System.Text;

namespace Web.Domain.Helpers
{
    public static class AesEncryptionHelper
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("GetHackedSalting");
        private static readonly int Iterations = 1000;
        private const int AesKeySize = 32; // AES key size is 32 bytes for 256-bit encryption

        public static string Encrypt(string plainText, string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(AesKeySize);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();

            memoryStream.WriteByte((byte)aes.IV.Length); // Write the IV length as a single byte
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256);
            byte[] aesKey = deriveBytes.GetBytes(AesKeySize);
            byte[] bytes = Convert.FromBase64String(cipherText);

            using var memoryStream = new MemoryStream(bytes);
            var aes = Aes.Create();
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC;

            int ivLength = memoryStream.ReadByte(); // Read the IV length as a single byte
            var iv = new byte[ivLength];
            memoryStream.Read(iv, 0, ivLength);
            aes.IV = iv;

            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}