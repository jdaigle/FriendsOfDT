using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FODT.Security
{
    public sealed class LoadPrincipalAttribute : FilterAttribute, IAuthorizationFilter
    {
        public static Func<IAuthenticationTokenContext> GetAuthenticationTokenContext { get; set; }
        private readonly Func<ControllerContext, AuthenticationTokenIdentity, IPrincipal, IPrincipal> lookupPrincipal;
        private static readonly IPrincipal GuestPrincipal = new GuestPrincipal(new GuestPrincipal.GuestIdentity());

        static LoadPrincipalAttribute()
        {
            GetAuthenticationTokenContext = new Func<IAuthenticationTokenContext>(() =>
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                return new HttpAuthenticationTokenContext(httpContext, new HttpCookieCollectionWrapper(httpContext));
            });
        }

        public LoadPrincipalAttribute() : this(null) { }

        /// <param name="lookupPrincipal">
        /// A Function which takes in (ControllerContext, IClearwaveIdentity from AuthToken, instance of UnknownPrincipal) and
        /// returns an IPrincipal Matching the Request
        /// </param>
        public LoadPrincipalAttribute(Func<ControllerContext, AuthenticationTokenIdentity, IPrincipal, IPrincipal> lookupPrincipal)
        {
            this.lookupPrincipal = lookupPrincipal ?? ((c, x, y) => { return y; });
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var authenticationTokenContext = GetAuthenticationTokenContext();
            var authenticationToken = authenticationTokenContext.GetCurrentAuthenticationToken();
            if (authenticationToken != null && !authenticationToken.IsExpired())
            {
                Thread.CurrentPrincipal = lookupPrincipal(filterContext, authenticationToken.Identity, GuestPrincipal);
            }
            else
            {
                Thread.CurrentPrincipal = GuestPrincipal;
            }
            filterContext.HttpContext.User = Thread.CurrentPrincipal;
        }
    }
}