using System;
using System.Security.Cryptography;
using System.Text;

namespace FriendsOfDT.Cryptography
{
    public static class SecureValue
    {
        private const string DEFAULT_KET = "Cl3@rw@v353cur1ty";

        public static string SymmetricEncrypt(this string plaintext, SymmetricAlgorithm algorithm)
        {
            return SymmetricEncrypt(plaintext, DEFAULT_KET, algorithm);
        }

        public static string SymmetricEncrypt(this string plaintext, string key, SymmetricAlgorithm algorithm)
        {
            if (string.IsNullOrWhiteSpace(plaintext))
                throw new ArgumentNullException("plaintext", "Cannot encrypt an empty string.");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key", "Cannot encrypt with an empty key.");

            byte[] keyBuffer = Convert.FromBase64String(key.Hash(HashAlgorithm.MD5));
            byte[] plainTextBuffer = Encoding.UTF8.GetBytes(plaintext);

            System.Security.Cryptography.SymmetricAlgorithm symmetricAlgorithm;
            switch (algorithm)
            {
                //case SymmetricAlgorithmEnum.DES:
                //    symmetricAlgorithm = new DESCryptoServiceProvider();
                //    break;
                case SymmetricAlgorithm.RC2:
                    symmetricAlgorithm = new RC2CryptoServiceProvider();
                    break;
                case SymmetricAlgorithm.TripleDES:
                    symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                    break;
                case SymmetricAlgorithm.Aes:
                default:
                    symmetricAlgorithm = new AesCryptoServiceProvider();
                    break;
            }

            symmetricAlgorithm.Key = keyBuffer;
            symmetricAlgorithm.Mode = CipherMode.ECB;
            // The legacy code did not indicate padding - not sure if this will effect the ouput much but we probably shouldn't change it
            // symmetricAlgorithm.Padding = PaddingMode.PKCS7;

            var encryptor = symmetricAlgorithm.CreateEncryptor();
            byte[] cipherBuffer = encryptor.TransformFinalBlock(plainTextBuffer, 0, plainTextBuffer.Length);
            symmetricAlgorithm.Clear();

            return Convert.ToBase64String(cipherBuffer);
        }

        public static string SymmetricDecrypt(this string plaintext, SymmetricAlgorithm algorithm)
        {
            return SymmetricDecrypt(plaintext, DEFAULT_KET, algorithm);
        }

        public static string SymmetricDecrypt(this string cipherText, string key, SymmetricAlgorithm algorithm)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                throw new ArgumentNullException("cipherText", "Cannot decrypt an empty cipher text.");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key", "Cannot decrypt with an empty key.");

            byte[] keyBuffer = Convert.FromBase64String(key.Hash(HashAlgorithm.MD5));
            byte[] cipherTextBuffer = Convert.FromBase64String(cipherText);

            System.Security.Cryptography.SymmetricAlgorithm symmetricAlgorithm;
            switch (algorithm)
            {
                // case SymmetricAlgorithmEnum.DES:
                //     symmetricAlgorithm = new DESCryptoServiceProvider();
                //     break;
                case SymmetricAlgorithm.RC2:
                    symmetricAlgorithm = new RC2CryptoServiceProvider();
                    break;
                case SymmetricAlgorithm.TripleDES:
                    symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                    break;
                case SymmetricAlgorithm.Aes:
                default:
                    symmetricAlgorithm = new AesCryptoServiceProvider();
                    break;
            }

            symmetricAlgorithm.Key = keyBuffer;
            symmetricAlgorithm.Mode = CipherMode.ECB;
            // The legacy code did not indicate padding - not sure if this will effect the ouput much but we probably shouldn't change it
            // symmetricAlgorithm.Padding = PaddingMode.PKCS7;

            var decryptor = symmetricAlgorithm.CreateDecryptor();
            byte[] plainTextBuffer = decryptor.TransformFinalBlock(cipherTextBuffer, 0, cipherTextBuffer.Length);
            symmetricAlgorithm.Clear();

            return Encoding.Default.GetString(plainTextBuffer);
        }
    }
}
