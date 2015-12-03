using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FriendsOfDT.Cryptography
{
    public class CWCryptoStream : CryptoStream
    {
        public CWCryptoStream(Stream target, CryptoStreamMode streamMode)
            : base(target, GenerateCryptoTransform(streamMode), streamMode)
        {
        }

        private static ICryptoTransform GenerateCryptoTransform(CryptoStreamMode streamMode)
        {
            if (streamMode == CryptoStreamMode.Read)
                return Rijndael.Create().CreateDecryptor(GetKey(), GetIv());
            else
                return Rijndael.Create().CreateEncryptor(GetKey(), GetIv());
        }

        private const string Key = "{D32E9160-82AF-4D45-BFE3-5644C57C2547}";
        private const string Iv = "{22427A14-FCD2-4E3A-89DE-CE92381BF248}";

        private static byte[] GetKey()
        {
            return ASCIIEncoding.Default.GetBytes(Key).Take(32).ToArray();
        }

        private static byte[] GetIv()
        {
            return ASCIIEncoding.Default.GetBytes(Iv).Take(16).ToArray();
        }


    }
}
