using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FriendsOfDT.Models.Accounts;
using FriendsOfDT.Tasks;
using Raven.Client;

namespace FriendsOfDT.Controllers {
    public partial class AbstractController : Controller {
        public IDocumentSession DocumentSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            DocumentSession = MvcApplication.DocumentStore.OpenSession();
        }

        protected void SetRoles(IEnumerable<WebAccountRole> roles) {
            var authCookie = HttpContext.Response.Cookies["QEMuwAstuk"];
            var rolesCookie = new HttpCookie("ceketHefRu");
            rolesCookie.Value = authCookie.Values + "|" + string.Join("|", roles.Select(x => x.ToString()).ToArray());
            rolesCookie.Expires = authCookie.Expires;
            rolesCookie.Domain = authCookie.Domain;
            rolesCookie.Secure = authCookie.Secure;
            rolesCookie.HttpOnly = authCookie.HttpOnly;
            HttpContext.Response.Cookies.Add(rolesCookie);
        }

        protected IEnumerable<WebAccountRole> GetRoles() {
            return GetRoles(HttpContext);
        }

        public static IEnumerable<WebAccountRole> GetRoles(HttpContextBase httpContext) {
            if (httpContext.Items.Contains("webAccountRoles")) {
                return (IEnumerable<WebAccountRole>)httpContext.Items["webAccountRoles"];
            }
            if (!httpContext.Request.Cookies.AllKeys.Contains("QEMuwAstuk") ||
                !httpContext.Request.Cookies.AllKeys.Contains("ceketHefRu"))
                return new WebAccountRole[0];
            var authCookie = httpContext.Request.Cookies["QEMuwAstuk"];
            var rolesCookie = httpContext.Request.Cookies["ceketHefRu"];

            var rolesValue = rolesCookie.Value;
            if (!rolesValue.StartsWith(authCookie.Value))
                return new WebAccountRole[0];
            httpContext.Items["webAccountRoles"] = rolesValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => {
                WebAccountRole result;
                if (Enum.TryParse<WebAccountRole>(x, out result))
                    return result;
                return WebAccountRole.None;
            }).Where(x => x != WebAccountRole.None);
            return (IEnumerable<WebAccountRole>)httpContext.Items["webAccountRoles"];
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            ViewBag.Roles = GetRoles();
            ViewBag.CanSeeAdminLink = AdminController.CanSeeAdminLink(ViewBag.Roles);
            if (ViewBag.IsAuthenticated) {
                var webAccount = DocumentSession.Load<WebAccount>(HttpContext.User.Identity.Name);
                ViewBag.Account = webAccount;
            }
            try {
                using (DocumentSession) {
                    if (filterContext.Exception == null) {
                        DocumentSession.SaveChanges();
                        TaskExecuter.StartExecuting();
                    }
                }
            } finally {
                TaskExecuter.Discard();
            }
        }
    }
}