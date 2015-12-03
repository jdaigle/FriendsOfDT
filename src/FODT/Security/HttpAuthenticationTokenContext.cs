using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;

namespace FODT.Security
{
    public class HttpAuthenticationTokenContext : IAuthenticationTokenContext
    {
        public const string AuthenticationTokenCookieName = "hlasOeglEsoUwRo";
        private readonly HttpContextBase httpContextBase;
        private readonly ICookieCollection cookies;

        public HttpAuthenticationTokenContext(HttpContextBase httpContextBase, ICookieCollection cookies)
        {
            this.httpContextBase = httpContextBase;
            this.cookies = cookies;
        }

        public AuthenticationToken IssueAuthenticationToken(int userAccountId, string accessToken, string name, string authenticationType, int expiresInSeconds)
        {
            var authenticationToken = new AuthenticationToken(userAccountId, accessToken, name, authenticationType, expiresInSeconds);
            IssueAuthenticationTokenCookie(authenticationToken);
            return authenticationToken;
        }

        public void RevokeAuthenticationToken()
        {
            cookies.Clear(AuthenticationTokenCookieName);
        }

        public AuthenticationToken CachedAuthenticationToken
        {
            get
            {
                return (AuthenticationToken)this.httpContextBase.Items[AuthenticationTokenCookieName];
            }
            set
            {
                this.httpContextBase.Items[AuthenticationTokenCookieName] = value;
            }
        }

        public AuthenticationToken GetCurrentAuthenticationToken()
        {
            if (CachedAuthenticationToken != null)
                return CachedAuthenticationToken;
            var authenticationTokenCookie = cookies.Get(AuthenticationTokenCookieName);
            if (authenticationTokenCookie != null && !string.IsNullOrEmpty(authenticationTokenCookie.Value))
            {
                try
                {
                    CachedAuthenticationToken = AuthenticationToken.Decrpyt(authenticationTokenCookie.Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return CachedAuthenticationToken;
        }

        public bool IsAuthenticated()
        {
            return GetCurrentAuthenticationToken() != null && !GetCurrentAuthenticationToken().IsExpired();
        }

        private void IssueAuthenticationTokenCookie(AuthenticationToken authenticationToken)
        {
            var authenticationTokenCookie = new HttpCookie(AuthenticationTokenCookieName, AuthenticationToken.Encrpyt(authenticationToken));
            authenticationTokenCookie.HttpOnly = true;
            authenticationTokenCookie.Expires = authenticationToken.ExpirationDateTime;
            cookies.Add(authenticationTokenCookie);
        }
    }
}