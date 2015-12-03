using System;
using System.Security.Cryptography;
using System.Text;

namespace FriendsOfDT.Cryptography
{
    public static class HashGenerator
    {
        private const int MIN_SALT_SIZE = 4;
        private const int MAX_SALT_SIZE = 8;

        public static byte[] GenerateSalt()
        {
            int saltSize = new Random().Next(MIN_SALT_SIZE, MAX_SALT_SIZE);
            var saltBuffer = new byte[saltSize];
            new RNGCryptoServiceProvider().GetNonZeroBytes(saltBuffer);
            return saltBuffer;
        }

        public static byte[] ComputeHash(byte[] buffer, HashAlgorithm algorithm)
        {
            System.Security.Cryptography.HashAlgorithm hashAlgorithm;
            switch (algorithm)
            {
                case HashAlgorithm.MD5:
                    hashAlgorithm = new MD5CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA1:
                    hashAlgorithm = new SHA1CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA256:
                    hashAlgorithm = new SHA256CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA384:
                    hashAlgorithm = new SHA384CryptoServiceProvider();
                    break;
                default:
                case HashAlgorithm.SHA512:
                    hashAlgorithm = new SHA512CryptoServiceProvider();
                    break;
            }
            return hashAlgorithm.ComputeHash(buffer);
        }

        public static string Hash(this string plaintext, HashAlgorithm algorithm)
        {
            // THIS HASH DOES NOT USE A SALT
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentNullException("plaintext", "Cannot hash an empty string.");
            return Convert.ToBase64String(ComputeHash(Encoding.Default.GetBytes(plaintext), algorithm));
        }

        public static string HashWithSalt(this string plaintext, byte[] saltBuffer, HashAlgorithm algorithm)
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentNullException("plaintext", "Cannot hash an empty string.");

            if (saltBuffer == null)
                saltBuffer = GenerateSalt();

            var unhashedBuffer = Encoding.Default.GetBytes(plaintext);
            var saltedUnhashedBuffer = new byte[unhashedBuffer.Length + saltBuffer.Length];
            for (int i = 0; i < unhashedBuffer.Length; i++)
                saltedUnhashedBuffer[i] = unhashedBuffer[i];
            for (int i = 0; i < saltBuffer.Length; i++)
                saltedUnhashedBuffer[unhashedBuffer.Length + i] = saltBuffer[i];

            return Convert.ToBase64String(ComputeHash(saltedUnhashedBuffer, algorithm));
        }
    }
}
