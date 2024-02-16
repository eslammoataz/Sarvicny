using System.Security.Cryptography;
using System.Text;

public static class CryptographyService
{
    // Use a secret key for encryption and decryption
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourSecretKey");

    public static int EncryptToInteger(string originalId)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Convert the original ID string to bytes
            byte[] inputBuffer = Encoding.UTF8.GetBytes(originalId);

            // Encrypt the ID
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            // Convert the encrypted bytes to an integer
            return BitConverter.ToInt32(encryptedBytes, 0);
        }
    }

    public static string DecryptFromInteger(int encryptedInteger)
    {
        byte[] encryptedBytes = BitConverter.GetBytes(encryptedInteger);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.GenerateIV();

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Decrypt the encrypted bytes
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            // Convert the decrypted bytes back to the original string ID
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
