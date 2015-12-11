using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Security
{
    public static class SimpleBase64UrlSafeTextEncoder
    {
        public static string Encode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static byte[] Decode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return Convert.FromBase64String(Pad(text.Replace('-', '+').Replace('_', '/')));
        }

        private static string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if (padding == 0)
            {
                return text;
            }
            return text + new string('=', padding);
        }
    }
}