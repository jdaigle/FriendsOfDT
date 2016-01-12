using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FODT
{
    public static class AzureBlogStorageUtil
    {
        public static byte[] DownloadPublicBlob(string url)
        {
            try
            {
                var request = HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        return ReadFully(stream);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (ex.Response)
                    {
                        if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                    }
                }
                throw;
            }
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public static bool BlobExists(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "HEAD";
            request.Timeout = 3000;
            request.ServicePoint.Expect100Continue = false;
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (ex.Response)
                    {
                        if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                        {
                            return false;
                        }
                    }
                }
                throw;
            }
        }

        public static void PutBlob(string url, string accountName, string accountKey, byte[] blob, string contentType = "")
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "PUT";
            request.Timeout = 5000;

            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.Headers.Add("x-ms-version", "2015-02-21");
            request.ContentLength = blob.Length;

            if (!contentType.IsNullOrWhiteSpace())
            {
                request.ContentType = contentType;
            }

            SignRequest(request, accountName, accountKey);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(blob, 0, blob.Length);
                stream.Flush();
            }

            using (request.GetResponse()) { }
        }

        public static void DeleteBlob(string url, string accountName, string accountKey)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "DELETE";
            request.Timeout = 5000;

            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.Headers.Add("x-ms-version", "2015-02-21");

            SignRequest(request, accountName, accountKey);

            using (request.GetResponse()) { }
        }

        public static void SignRequest(HttpWebRequest request, string accountName, string accountKey)
        {
            if (!request.Headers.AllKeys.Contains("x-ms-date", StringComparer.Ordinal))
            {
                string dateString = ConvertDateTimeToHttpString(DateTime.UtcNow);
                request.Headers.Add("x-ms-date", dateString);
            }

            string message = CanonicalizeHttpRequest(request, accountName);
            string signature = ComputeHmac256(Convert.FromBase64String(accountKey), message);

            request.Headers.Add(
                "Authorization",
                $"SharedKey {accountName}:{signature}");
        }

        private static string ComputeHmac256(byte[] key, string message)
        {
            using (HashAlgorithm hashAlgorithm = new HMACSHA256(key))
            {
                byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                return Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
            }
        }

        private static string CanonicalizeHttpRequest(HttpWebRequest request, string accountName)
        {
            // Add the method (GET, POST, PUT, or HEAD).
            CanonicalizedString canonicalizedString = new CanonicalizedString(request.Method);

            // Add the Content-* HTTP headers. Empty values are allowed.
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentEncoding]);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentLanguage]);
            AppendCanonicalizedContentLengthHeader(canonicalizedString, request);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentMd5]);
            canonicalizedString.AppendCanonicalizedElement(request.ContentType);

            // Add the Date HTTP header (only if the x-ms-date header is not being used)
            AppendCanonicalizedDateHeader(canonicalizedString, request);

            // Add If-* headers and Range header
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfModifiedSince]);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfMatch]);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfNoneMatch]);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfUnmodifiedSince]);
            canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.Range]);

            // Add any custom headers
            AppendCanonicalizedCustomHeaders(canonicalizedString, request);

            // Add the canonicalized URI element
            string resourceString = GetCanonicalizedResourceString(request.RequestUri, accountName);
            canonicalizedString.AppendCanonicalizedElement(resourceString);

            return canonicalizedString.ToString();
        }

        private static void AppendCanonicalizedContentLengthHeader(CanonicalizedString canonicalizedString, HttpWebRequest request)
        {
            if (request.ContentLength != -1L && request.ContentLength != 0)
            {
                canonicalizedString.AppendCanonicalizedElement(request.ContentLength.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                canonicalizedString.AppendCanonicalizedElement(null);
            }
        }

        private static void AppendCanonicalizedDateHeader(CanonicalizedString canonicalizedString, HttpWebRequest request)
        {
            string microsoftDateHeaderValue = request.Headers["x-ms-date"];
            // Add the Date HTTP header (only if the x-ms-date header is not being used)
            if (string.IsNullOrEmpty(microsoftDateHeaderValue))
            {
                canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.Date]);
            }
            else
            {
                canonicalizedString.AppendCanonicalizedElement(null);
            }
        }

        private static void AppendCanonicalizedCustomHeaders(CanonicalizedString canonicalizedString, HttpWebRequest request)
        {
            List<string> headerNames = new List<string>(request.Headers.AllKeys.Length);
            foreach (string headerName in request.Headers.AllKeys)
            {
                if (headerName.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
                {
                    headerNames.Add(headerName.ToLowerInvariant());
                }
            }

            CultureInfo sortingCulture = new CultureInfo("en-US");
            StringComparer sortingComparer = StringComparer.Create(sortingCulture, false);
            headerNames.Sort(sortingComparer);

            StringBuilder canonicalizedElement = new StringBuilder(50);
            foreach (string headerName in headerNames)
            {
                string value = request.Headers[headerName];
                if (!string.IsNullOrEmpty(value))
                {
                    canonicalizedElement.Length = 0;
                    canonicalizedElement.Append(headerName);
                    canonicalizedElement.Append(":");
                    canonicalizedElement.Append(value.TrimStart().Replace("\r\n", string.Empty));

                    canonicalizedString.AppendCanonicalizedElement(canonicalizedElement.ToString());
                }
            }
        }

        private static string GetCanonicalizedResourceString(Uri uri, string accountName, bool isSharedKeyLiteOrTableService = false)
        {
            StringBuilder canonicalizedResource = new StringBuilder(100);
            canonicalizedResource.Append('/');
            canonicalizedResource.Append(accountName);
            canonicalizedResource.Append(GetAbsolutePathWithoutSecondarySuffix(uri, accountName));

            IDictionary<string, string> queryParameters = ParseQueryString(uri.Query);
            if (!isSharedKeyLiteOrTableService)
            {
                List<string> queryParameterNames = new List<string>(queryParameters.Keys);
                queryParameterNames.Sort(StringComparer.OrdinalIgnoreCase);

                foreach (string queryParameterName in queryParameterNames)
                {
                    canonicalizedResource.Append('\n');
                    canonicalizedResource.Append(queryParameterName.ToLowerInvariant());
                    canonicalizedResource.Append(':');
                    canonicalizedResource.Append(queryParameters[queryParameterName]);
                }
            }
            else
            {
                // Add only the comp parameter
                string compQueryParameterValue;
                if (queryParameters.TryGetValue("comp", out compQueryParameterValue))
                {
                    canonicalizedResource.Append("?comp=");
                    canonicalizedResource.Append(compQueryParameterValue);
                }
            }

            return canonicalizedResource.ToString();
        }

        private static string GetAbsolutePathWithoutSecondarySuffix(Uri uri, string accountName)
        {
            string absolutePath = uri.AbsolutePath;
            string secondaryAccountName = string.Concat(accountName, "-secondary");

            int startIndex = absolutePath.IndexOf(secondaryAccountName, StringComparison.OrdinalIgnoreCase);
            if (startIndex == 1)
            {
                startIndex += accountName.Length;
                absolutePath = absolutePath.Remove(startIndex, "-secondary".Length);
            }

            return absolutePath;
        }

        private class CanonicalizedString
        {
            private const int DefaultCapacity = 300;
            private const char ElementDelimiter = '\n';

            /// <summary>
            /// Stores the internal <see cref="StringBuilder"/> that holds the canonicalized string.
            /// </summary>
            private readonly StringBuilder canonicalizedString;

            /// <summary>
            /// Initializes a new instance of the <see cref="CanonicalizedString"/> class.
            /// </summary>
            /// <param name="initialElement">The first canonicalized element to start the string with.</param>
            public CanonicalizedString(string initialElement)
                : this(initialElement, CanonicalizedString.DefaultCapacity)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CanonicalizedString"/> class.
            /// </summary>
            /// <param name="initialElement">The first canonicalized element to start the string with.</param>
            /// <param name="capacity">The starting size of the string.</param>
            public CanonicalizedString(string initialElement, int capacity)
            {
                this.canonicalizedString = new StringBuilder(initialElement, capacity);
            }

            /// <summary>
            /// Append additional canonicalized element to the string.
            /// </summary>
            /// <param name="element">An additional canonicalized element to append to the string.</param>
            public void AppendCanonicalizedElement(string element)
            {
                this.canonicalizedString.Append(CanonicalizedString.ElementDelimiter);
                this.canonicalizedString.Append(element);
            }

            /// <summary>
            /// Converts the value of this instance to a string.
            /// </summary>
            /// <returns>A string whose value is the same as this instance.</returns>
            public override string ToString()
            {
                return this.canonicalizedString.ToString();
            }
        }

        /// <summary>
        /// Parse the http query string.
        /// </summary>
        /// <param name="query">Http query string.</param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query))
            {
                return retVal;
            }

            if (query.StartsWith("?", StringComparison.Ordinal))
            {
                if (query.Length == 1)
                {
                    return retVal;
                }

                query = query.Substring(1);
            }

            string[] valuePairs = query.Split('&');
            foreach (string pair in valuePairs)
            {
                string key;
                string value;

                int equalDex = pair.IndexOf("=", StringComparison.Ordinal);
                if (equalDex < 0)
                {
                    key = string.Empty;
                    value = Uri.UnescapeDataString(pair);
                }
                else
                {
                    key = Uri.UnescapeDataString(pair.Substring(0, equalDex));
                    value = Uri.UnescapeDataString(pair.Substring(equalDex + 1));
                }

                string existingValue;
                if (retVal.TryGetValue(key, out existingValue))
                {
                    retVal[key] = string.Concat(existingValue, ",", value);
                }
                else
                {
                    retVal[key] = value;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Converts the DateTimeOffset object to an Http string of form: Mon, 28 Jan 2008 12:11:37 GMT.
        /// </summary>
        /// <param name="dateTime">The DateTimeOffset object to convert to an Http string.</param>
        /// <returns>String of form: Mon, 28 Jan 2008 12:11:37 GMT.</returns>
        public static string ConvertDateTimeToHttpString(DateTimeOffset dateTime)
        {
            // 'R' means rfc1123 date which is what the storage services use for all dates...
            // It will be in the following format:
            // Mon, 28 Jan 2008 12:11:37 GMT
            return dateTime.UtcDateTime.ToString("R", CultureInfo.InvariantCulture);
        }
    }
}
