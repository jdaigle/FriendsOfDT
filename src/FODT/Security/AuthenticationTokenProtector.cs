using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;

namespace FODT.Security
{
    public static class AuthenticationTokenProtector
    {
        public static string Protect(AuthenticationToken token)
        {
            byte[] buffer = AuthenticationTokenSerializer.Serialize(token);
            byte[] protectedBuffer = Encrpyt(buffer);
            return SimpleBase64UrlSafeTextEncoder.Encode(protectedBuffer);
        }

        public static AuthenticationToken Unprotect(string protectedToken)
        {
            try
            {
                if (protectedToken.IsNullOrWhiteSpace())
                {
                    return null;
                }

                byte[] protectedBuffer = SimpleBase64UrlSafeTextEncoder.Decode(protectedToken);
                if (protectedBuffer == null)
                {
                    return null;
                }

                byte[] buffer = Decrpyt(protectedBuffer);
                if (buffer == null)
                {
                    return null;
                }

                return AuthenticationTokenSerializer.Deserialize(buffer);
            }
            catch
            {
                throw new Exception("Failed to unprotect authentication token. This will require some debugging...");
            }
        }

        public static byte[] Encrpyt(byte[] buffer)
        {
            using (var encryptor = symmetricAlgorithm.CreateEncryptor())
            {
                return encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        public static byte[] Decrpyt(byte[] buffer)
        {
            using (var decryptor = symmetricAlgorithm.CreateDecryptor())
            {
                return decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        private static readonly byte[] decryptionKey;
        private static readonly SymmetricAlgorithm symmetricAlgorithm;

        static AuthenticationTokenProtector()
        {
            var decryptionKeyHex = ConfigurationManager.AppSettings["cookie.decryptionKey"];
            decryptionKey = Enumerable.Range(0, decryptionKeyHex.Length)
                .Where(x => 0 == x % 2)
                .Select(x => Convert.ToByte(decryptionKeyHex.Substring(x, 2), 16))
                .ToArray();

            symmetricAlgorithm = new AesCryptoServiceProvider();
            symmetricAlgorithm.Key = decryptionKey;
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
        }
    }
}