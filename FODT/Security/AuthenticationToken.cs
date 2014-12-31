using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;

namespace FODT.Security
{
    public sealed class AuthenticationToken
    {
        private const string decryptionKeyHex = "7059303602CBDCDB13888FFEE7809A0299082917EE06394B0B3459A20F9CB631";
        private static readonly byte[] decryptionKey;
        private static readonly SymmetricAlgorithm symmetricAlgorithm;

        public AuthenticationToken() { }

        public AuthenticationToken(int userAccountId, string accessToken, string name, string authenticationType, int expiresInSeconds)
            : this(userAccountId, accessToken, name, authenticationType, DateTime.Now.AddSeconds(expiresInSeconds)) { }

        private AuthenticationToken(int userAccountId, string accessToken, string name, string authenticationType, DateTime expirationDateTime)
        {
            this.UserAccountId = userAccountId;
            this.AccessToken = accessToken;
            this.Name = name;
            this.AuthenticationType = authenticationType;
            this.ExpirationDateTime = expirationDateTime;
        }

        public int UserAccountId { get; set; }
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        private AuthenticationTokenIdentity identity;
        [JsonIgnore]
        public AuthenticationTokenIdentity Identity { get { return identity ?? (identity = new AuthenticationTokenIdentity(this)); } }

        public bool IsExpired()
        {
            return DateTime.Now >= ExpirationDateTime;
        }

        static AuthenticationToken()
        {
            decryptionKey = Enumerable.Range(0, decryptionKeyHex.Length)
                .Where(x => 0 == x % 2)
                .Select(x => Convert.ToByte(decryptionKeyHex.Substring(x, 2), 16))
                .ToArray();

            symmetricAlgorithm = new AesCryptoServiceProvider();
            symmetricAlgorithm.Key = decryptionKey;
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
        }

        public static string Encrpyt(AuthenticationToken token)
        {
            var plainText = JsonConvert.SerializeObject(token);
            var plainTextBuffer = Encoding.ASCII.GetBytes(plainText);
            using (var encryptor = symmetricAlgorithm.CreateEncryptor())
            {
                var cipherBuffer = encryptor.TransformFinalBlock(plainTextBuffer, 0, plainTextBuffer.Length);
                return Convert.ToBase64String(cipherBuffer);
            }
        }

        public static AuthenticationToken Decrpyt(string cipherText)
        {
            var cipherTextBuffer = Convert.FromBase64String(cipherText);
            using (var decryptor = symmetricAlgorithm.CreateDecryptor())
            {
                byte[] plainTextBuffer = decryptor.TransformFinalBlock(cipherTextBuffer, 0, cipherTextBuffer.Length);
                var plainText = Encoding.ASCII.GetString(plainTextBuffer);
                return JsonConvert.DeserializeObject<AuthenticationToken>(plainText);
            }
        }
    }
}
