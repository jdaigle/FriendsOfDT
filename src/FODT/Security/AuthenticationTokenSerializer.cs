using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;

namespace FODT.Security
{
    public static class AuthenticationTokenSerializer
    {
        private const int FormatVersion = 1;

        public static byte[] Serialize(AuthenticationToken token)
        {
            using (var memory = new MemoryStream())
            {
                using (var compression = new GZipStream(memory, CompressionLevel.Optimal))
                {
                    using (var writer = new BinaryWriter(compression))
                    {
                        Write(writer, token);
                    }
                }
                return memory.ToArray();
            }
        }

        public static AuthenticationToken Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var compression = new GZipStream(memory, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(compression))
                    {
                        return Read(reader);
                    }
                }
            }
        }

        public static void Write(BinaryWriter writer, AuthenticationToken token)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            writer.Write(FormatVersion);
            var identity = token.Identity;
            writer.Write(identity.AuthenticationType);
            writer.Write(identity.Claims.Count());
            foreach (var claim in identity.Claims)
            {
                WriteWithDefault(writer, claim.Type, identity.NameClaimType);
                writer.Write(claim.Value);
                WriteWithDefault(writer, claim.ValueType, DefaultValues.StringValueType);
            }
            WriteProperties(writer, token.Properties);
        }

        public static void WriteProperties(BinaryWriter writer, AuthenticationProperties properties)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            writer.Write(FormatVersion);
            writer.Write(properties.Dictionary.Count);
            foreach (var kv in properties.Dictionary)
            {
                writer.Write(kv.Key);
                writer.Write(kv.Value);
            }
        }

        public static AuthenticationToken Read(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.ReadInt32() != FormatVersion)
            {
                return null;
            }

            string authenticationType = reader.ReadString();
            int count = reader.ReadInt32();
            var claims = new Claim[count];
            for (int index = 0; index != count; ++index)
            {
                string type = ReadWithDefault(reader, ClaimsIdentity.DefaultNameClaimType);
                string value = reader.ReadString();
                string valueType = ReadWithDefault(reader, DefaultValues.StringValueType);
                claims[index] = new Claim(type, value, valueType);
            }
            var identity = new ClaimsIdentity(claims, authenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var properties = ReadProperties(reader);
            return new AuthenticationToken(identity, properties);
        }

        public static AuthenticationProperties ReadProperties(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.ReadInt32() != FormatVersion)
            {
                return null;
            }
            int count = reader.ReadInt32();
            var extra = new Dictionary<string, string>(count);
            for (int index = 0; index != count; ++index)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                extra.Add(key, value);
            }
            return new AuthenticationProperties(extra);
        }

        private static void WriteWithDefault(BinaryWriter writer, string value, string defaultValue)
        {
            if (string.Equals(value, defaultValue, StringComparison.Ordinal))
            {
                writer.Write(DefaultValues.DefaultStringPlaceholder);
            }
            else
            {
                writer.Write(value);
            }
        }

        private static string ReadWithDefault(BinaryReader reader, string defaultValue)
        {
            string value = reader.ReadString();
            if (string.Equals(value, DefaultValues.DefaultStringPlaceholder, StringComparison.Ordinal))
            {
                return defaultValue;
            }
            return value;
        }

        private static class DefaultValues
        {
            public const string DefaultStringPlaceholder = "\0";
            public const string StringValueType = "http://www.w3.org/2001/XMLSchema#string";
        }
    }
}