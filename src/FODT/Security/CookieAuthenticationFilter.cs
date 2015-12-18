using System;
using System.Data;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.FODT;
using NHibernate;

namespace FODT.Security
{
    public class CookieAuthenticationFilter : IAuthorizationFilter, IActionFilter
    {
        private const string cookieName = "vS4yzAh7vxYg7a8P";
        private static readonly DateTime expiredCookieDateTime = new DateTime(1990, 1, 1);

        public void OnActionExecuting(ActionExecutingContext filterContext) { } // no op

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Set<IAuthenticationManager>(new AuthenticationManager());

            var authCookie = filterContext.HttpContext.Request.Cookies.Get(cookieName);
            if (authCookie == null || authCookie.Value.IsNullOrWhiteSpace())
            {
                SetGuestPrincipal(filterContext);
                filterContext.HttpContext.Get<IAuthenticationManager>().SignOut();
                return;
            }

            AuthenticationToken authenticationToken = null;
            try
            {
                authenticationToken = AuthenticationTokenProtector.Unprotect(authCookie.Value);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.Fail("TODO log and return instead of throwing exception");
                throw;
            }

            // note that we don't check for cookie expiration, we simply don't care.

            var tokenClaim = authenticationToken.Identity.FindFirst(ClaimTypes.NameIdentifier);
            var authenticationType = authenticationToken.Identity.AuthenticationType;

            UserAccount userAccount = null;
            // TODO move this database access code back behind some provider
            // and implement the switch logic as a subclass instead of procedure
            using (var databaseSession = DatabaseBootstrapper.SessionFactory.OpenSession())
            {
                databaseSession.FlushMode = FlushMode.Commit;
                databaseSession.Transaction.Begin(IsolationLevel.RepeatableRead);
                switch (authenticationType)
                {
                    case FacebookAuthentication.AuthenticationType:
                        {
                            var id = int.Parse(tokenClaim.Value);
                            userAccount = databaseSession.Get<UserFacebookAccessToken>(id)?.User;
                            break;
                        }
                    default:
                        {
                            System.Diagnostics.Debug.Fail("TODO: log that we have an invalid authentication type");
                            break;
                        }
                }
                if (userAccount != null)
                {
                    userAccount.UpdateSeen();
                }
                databaseSession.CommitTransaction();
            }

            if (userAccount == null)
            {
                SetGuestPrincipal(filterContext);
                filterContext.HttpContext.Get<IAuthenticationManager>().SignOut();
            }

            authenticationToken.Identity.RemoveClaim(tokenClaim);
            authenticationToken.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userAccount.UserAccountId.ToString()));
            authenticationToken.Identity.AddClaim(new Claim(authenticationToken.Identity.NameClaimType, userAccount.Name));
            // TODO roles

            SetPrincipal(filterContext.HttpContext, new ClaimsPrincipal(authenticationToken.Identity));
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var authenticationManager = filterContext.HttpContext.Get<IAuthenticationManager>();

            var shouldIssueNewCookie = authenticationManager.SignInToken != null;
            var shouldDeleteCookie = authenticationManager.IsSignOut;

            if (!(shouldIssueNewCookie || shouldDeleteCookie))
            {
                return;
            }

            var cookies = filterContext.HttpContext.Response.Cookies;

            var cookie = new HttpCookie(cookieName)
            {
                HttpOnly = true,
                Path = "/",
                Secure = filterContext.HttpContext.Request.IsSecureConnection,
            };

            if (shouldIssueNewCookie)
            {
                cookie.Expires = DateTime.UtcNow.AddYears(20); // effectively never expires
                cookie.Value = AuthenticationTokenProtector.Protect(authenticationManager.SignInToken);
            }
            else if (shouldDeleteCookie)
            {
                cookie.Expires = expiredCookieDateTime;
            }

            cookies.Add(cookie);

            // set cache headers to not cache the response
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
        }

        private void SetGuestPrincipal(AuthorizationContext filterContext)
        {
            SetPrincipal(filterContext.HttpContext, GuestPrincipal.Default);
        }

        private void SetPrincipal(HttpContextBase httpContext, IPrincipal principal)
        {
            httpContext.User = principal;
            Thread.CurrentPrincipal = principal;
        }
    }
}