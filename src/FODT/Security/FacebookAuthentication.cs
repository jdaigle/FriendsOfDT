using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace FODT.Security
{
    public static class FacebookAuthentication
    {
        public const string AuthenticationType = "oauth/facebook";

        /// <summary>
        /// The URI where the client will be redirected to authenticate.
        /// </summary>
        public const string AuthorizationEndpoint = @"https://www.facebook.com/dialog/oauth";
        /// <summary>
        /// The URI the middleware will access to exchange the OAuth token.
        /// </summary>
        public const string TokenEndpoint = @"https://graph.facebook.com/oauth/access_token";
        /// <summary>
        /// The URI the middleware will access to obtain the user information.
        /// </summary>
        public const string UserInformationEndpoint = @"https://graph.facebook.com/me";

        public static string GetAuthChallengeURL(HttpRequestBase request, FacebookAuthenticationOptions options)
        {
            string redirect_uri = CalculateRedirectURI(request, options);
            var client_id = Uri.EscapeDataString(options.AppId);
            var scope = Uri.EscapeDataString(options.Scope);

            // todo encrypt state and include a redirect URL?
            var state = Uri.EscapeDataString("");

            var url = $"{AuthorizationEndpoint}?response_type=code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&state={state}";

            return url;
        }

        public static FacebookAccessToken ExchangeCodeForAccessToken(HttpRequestBase request, FacebookAuthenticationOptions options, string code)
        {
            string redirect_uri = CalculateRedirectURI(request, options);
            var client_id = Uri.EscapeDataString(options.AppId);
            var client_secret = Uri.EscapeDataString(options.AppSecret);

            var getAccessTokenURL = $"{TokenEndpoint}?grant_type=authorization_code&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}&code={code}";
            var getAccessTokenResponse = ExecuteHttpGET(getAccessTokenURL);
            var parsedAccessTokenResponse = HttpUtility.ParseQueryString(getAccessTokenResponse);
            var accessToken = parsedAccessTokenResponse["access_token"];
            var expires = int.Parse(parsedAccessTokenResponse["expires"]);

            var appsecret_proof = GenerateAppSecretProof(accessToken, client_secret);
            var getUserInfoURL = $"{UserInformationEndpoint}?access_token={Uri.EscapeDataString(accessToken)}&appsecret_proof={Uri.EscapeDataString(appsecret_proof)}";
            var getUserInfoResponse = ExecuteHttpGET(getUserInfoURL);
            var userInfo = JObject.Parse(getUserInfoResponse);
            return new FacebookAccessToken(accessToken, expires, userInfo);
        }

        private static string GenerateAppSecretProof(string accessToken, string appSecret)
        {
            var key = Encoding.UTF8.GetBytes(appSecret);
            using (HashAlgorithm hashAlgorithm = new HMACSHA256(key))
            {
                byte[] hashedBuffer = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(accessToken));
                var sbHash = new StringBuilder();
                for (int i = 0; i < hashedBuffer.Length; i++)
                {
                    sbHash.Append(hashedBuffer[i].ToString("x2"));
                }
                return sbHash.ToString();
            }
        }

        private static string ExecuteHttpGET(string url)
        {
            try
            {
                var httpRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                using (var httpResponse = httpRequest.GetResponse())
                {
                    return ReadEntireResponse(httpResponse);
                }
            }
            catch (WebException e)
            {
                string error = "";
                string statusCode = "";
                if (e.Response != null)
                {
                    using (e.Response)
                    {
                        error = ReadEntireResponse(e.Response);
                        statusCode = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    }
                    throw new Exception($"WebException. HTTP: {statusCode} = {error}");
                }
                throw;
            }
        }

        private static string ReadEntireResponse(WebResponse response)
        {
            var httpResponse = (HttpWebResponse)response;
            using (var resStream = new StreamReader(httpResponse.GetResponseStream(), Encoding.ASCII))
            {
                return resStream.ReadToEnd();
            }
        }

        private static string CalculateRedirectURI(HttpRequestBase request, FacebookAuthenticationOptions options)
        {
            var requestPrefix = request.Url.GetLeftPart(UriPartial.Authority);
            var redirect_uri = requestPrefix + request.ApplicationPath.Substring(1) + options.CallbackPath;
            return Uri.EscapeDataString(redirect_uri);
        }
    }
}