using System.Linq;
using System.Web;
using System.Web.Mvc;
using FriendsOfDT.Controllers;
using FriendsOfDT.Models.Accounts;

namespace FriendsOfDT {
    public sealed class AuthorizeRoleAttribute : AuthorizeAttribute {

        public AuthorizeRoleAttribute(params WebAccountRole[] roles) {
            WebAccountRoles = roles ?? new WebAccountRole[0];
        }

        public WebAccountRole[] WebAccountRoles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (WebAccountRoles == null || WebAccountRoles.Length == 0)
                return true;
            var currentRoles = AbstractController.GetRoles(httpContext);
            foreach (var item in WebAccountRoles) {
                if (!currentRoles.Contains(item)) return false;
            }
            return true;
        }
    }
}
