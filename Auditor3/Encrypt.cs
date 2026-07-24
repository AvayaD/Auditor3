/*
 * Auditor3 :: Encrypt
 * 
 * This class defines the encryption mechanisms of the application.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Security.Cryptography;
using System.Text;

namespace Auditor3 {
    internal static class Encrypt {
        internal const string Key =                 // The AES encryption key
            "ZUExejVUU0pKOXJGanAvZ1ZUMG1hUT09LHJlTWc0U3Z2bVhBdjdiUVRFenc5STEzVXJWWUtGYkx4NTZmRWQ1N0Z5bDA9";
        internal const string Salt = "Cent1uriP2";  // The salt value

        // Method for generating an AES encryption key
        internal static string GenerateKey() {
            using var encryption = Aes.Create();
            encryption.KeySize = 256;
            encryption.BlockSize = 128;
            encryption.Mode = CipherMode.CBC;
            encryption.Padding = PaddingMode.PKCS7;

            encryption.GenerateIV();
            var iv = Convert.ToBase64String(encryption.IV);

            encryption.GenerateKey();
            var key = Convert.ToBase64String(encryption.Key);

            var complete = iv + "," + key;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(complete));
        }

        // Method for encrypting a string
        internal static string EncryptString(string input) {
            input = Salt + input;

            var keyParts = Encoding.UTF8.GetString(Convert.FromBase64String(Key)).Split(',');
            var ivBytes = Convert.FromBase64String(keyParts[0]);
            var keyBytes = Convert.FromBase64String(keyParts[1]);

            using var encryption = Aes.Create();
            encryption.KeySize = 256;
            encryption.BlockSize = 128;
            encryption.Mode = CipherMode.CBC;
            encryption.Padding = PaddingMode.PKCS7;
            encryption.IV = ivBytes;
            encryption.Key = keyBytes;

            var array = Encoding.UTF8.GetBytes(input);
            using var crypto = encryption.CreateEncryptor();
            var cipher = crypto.TransformFinalBlock(array, 0, array.Length);
            return Convert.ToBase64String(cipher);
        }

        // Method for decrypting a string
        public static string DecryptString(string input) {
            var keyParts = Encoding.UTF8.GetString(Convert.FromBase64String(Key)).Split(',');
            var ivBytes = Convert.FromBase64String(keyParts[0]);
            var keyBytes = Convert.FromBase64String(keyParts[1]);

            using var encryption = Aes.Create();
            encryption.KeySize = 256;
            encryption.BlockSize = 128;
            encryption.Mode = CipherMode.CBC;
            encryption.Padding = PaddingMode.PKCS7;
            encryption.IV = ivBytes;
            encryption.Key = keyBytes;

            var array = Convert.FromBase64CharArray(input.ToCharArray(), 0, input.Length);
            using var crypto = encryption.CreateDecryptor();
            var decrypted = Encoding.UTF8.GetString(crypto.TransformFinalBlock(array, 0, array.Length));

            return decrypted.Substring(Salt.Length, decrypted.Length - Salt.Length);
        }
    }
}
