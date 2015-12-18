using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Owin;

namespace FODT.Security
{
    using AuthenticateCallback = Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>;
    using AuthenticateDelegate = Func<string[], Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>, object, Task>;

    public class CookieAuthenticationMiddleware : OwinMiddleware
    {
        private const string cookieName = "vS4yzAh7vxYg7a8P";

        private const string HeaderNameCacheControl = "Cache-Control";
        private const string HeaderNamePragma = "Pragma";
        private const string HeaderNameExpires = "Expires";
        private const string HeaderValueNoCache = "no-cache";
        private const string HeaderValueMinusOne = "-1";

        public CookieAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app)
            : base(next)
        {
        }

        private Task _applyResponse;
        private bool _applyResponseInitialized;
        private object _applyResponseSyncLock;
        private bool _faulted;



        public override async Task Invoke(IOwinContext context)
        {
            context.Request.Set<AuthenticateDelegate>("security.Authenticate", AuthenticateAsync);
            

            context.Response.OnSendingHeaders(m => OnSendingHeaderCallback(context), this);
            await ReadAuthenticationCookieAsync(context);
            await Next.Invoke(context);
            await ApplyResponseExactlyOnceAsync(context);
        }

        public async Task AuthenticateAsync(
                string[] authenticationTypes,
                AuthenticateCallback callback,
                object state)
        {
            var properties = new Dictionary<string, object>(); // _handler.BaseOptions.Description.Properties
            if (authenticationTypes == null)
            {
                callback(null, null, properties, state);
            }
            else if (authenticationTypes.Contains("Cookie", StringComparer.Ordinal))
            {
                //AuthenticationTicket ticket = await _handler.AuthenticateAsync();
                //if (ticket != null && ticket.Identity != null)
                //{
                //    callback(ticket.Identity, ticket.Properties.Dictionary, properties, state);
                //}
            }
            await Task.FromResult(0);
        }

        private async Task ReadAuthenticationCookieAsync(IOwinContext context)
        {
            var cookieString = context.Request.Cookies["cookieName"];
            if (cookieString.IsNullOrWhiteSpace())
            {
                return;
            }

            AuthenticationToken token = null;
            try
            {
                token = AuthenticationTokenProtector.Unprotect(cookieString);
            }
            catch (Exception)
            {
                // TODO log and return instead of throwing exception
                throw;
            }

            // note that we don't check for cookie expiration, we simply don't care.

            // todo validate against database
            await Task.FromResult(0);

            // if valid
            context.Request.User = new ClaimsPrincipal(token.Identity);

            //var id = new ClaimsIdentity();
            //id.AddClaim(new Claim(ClaimTypes.NameIdentifier, ConvertIdToString(user.Id), //ClaimValueTypes.String));
            //id.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName, /ClaimValueTypes.String));
            //context.Request.User = new ClaimsPrincipal(id);
            //await Task.Yield();

            return;
        }

        protected async Task WriteAuthenticationCookieResponseAsync(IOwinContext context)
        {
            var shouldIssueNewCookie = false;
            var shouldDeleteCookie = false;
            if (!(shouldIssueNewCookie || shouldDeleteCookie ))
            {
                return;
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Secure = context.Request.IsSecure,
            };

            if (shouldIssueNewCookie)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddYears(20); // effectively never expires

                // var cookieValue = AuthenticationTokenProtector.Protect(token);
                // context.Response.Cookies.Append(cookieName, cookieValue, cookieOptions);

                throw new NotImplementedException("Issue new cookie");
            }
            else if (shouldDeleteCookie)
            {
                context.Response.Cookies.Delete(cookieName, cookieOptions);
            }

            // set cache headers to not cache the response
            context.Response.Headers.Set(
              HeaderNameCacheControl,
              HeaderValueNoCache);

            context.Response.Headers.Set(
                HeaderNamePragma,
                HeaderValueNoCache);

            context.Response.Headers.Set(
                HeaderNameExpires,
                HeaderValueMinusOne);

            await Task.FromResult(0);
        }

        private void OnSendingHeaderCallback(IOwinContext context)
        {
            ApplyResponseExactlyOnceAsync(context).Wait();
        }

        private async Task ApplyResponseExactlyOnceAsync(IOwinContext context)
        {
            try
            {
                if (!_faulted)
                {
                    await LazyInitializer.EnsureInitialized(
                        ref _applyResponse,
                        ref _applyResponseInitialized,
                        ref _applyResponseSyncLock,
                        () => WriteAuthenticationCookieResponseAsync(context));
                }
            }
            catch (Exception)
            {
                _faulted = true;
                throw;
            }
        }
    }
}