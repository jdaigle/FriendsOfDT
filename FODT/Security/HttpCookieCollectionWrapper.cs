using System;
using System.Web;

namespace FODT.Security
{
    public class HttpCookieCollectionWrapper : ICookieCollection
    {
        private readonly HttpContextBase httpContextBase;

        public HttpCookieCollectionWrapper(HttpContextBase httpContextBase)
        {
            this.httpContextBase = httpContextBase;
        }

        public void Add(HttpCookie cookie)
        {
            httpContextBase.Response.Cookies.Add(cookie);
        }

        public HttpCookie Get(string name)
        {
            return httpContextBase.Request.Cookies.Get(name);
        }

        public void Clear(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Value = String.Empty;
            httpContextBase.Response.SetCookie(cookie);
        }

        public void ClearAll()
        {
            foreach (string cookieName in httpContextBase.Request.Cookies.AllKeys)
            {
                Clear(cookieName);
            }
        }

        public void RemoveAllFromResponse()
        {
            httpContextBase.Response.Cookies.Clear();
        }
    }
}
