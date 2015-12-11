using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.FODT;
using FODT.Security;
using Newtonsoft.Json;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("User")]
    public class UserController : BaseController
    {
        [HttpGet, Route("SignIn")]
        public ActionResult SignIn(string redirectUrl)
        {
            var url = FacebookAuthentication.GetAuthChallengeURL(Request, FacebookAuthenticationOptions.FromWebConfig());
            return Redirect(url);
        }

        [HttpGet, Route("SignOut")]
        public ActionResult SignOut()
        {
            //authenticationTokenContext.RevokeAuthenticationToken();
            return Redirect("~");
        }

        private string GenerateFacbookOAuthResponseURL()
        {
            return Settings.Facebook_Login_SiteURL + Url.Action();
        }

        [HttpGet, Route("~/oauth/facebook")]
        public ActionResult HandleFacebookOAuthCallback(string code, string state)
        {
            // TODO, decrypt redirectURL
            var redirectURL = string.Empty;
            if (!string.IsNullOrWhiteSpace(state))
            {
                redirectURL = HttpUtility.UrlDecode(state);
            }

            var accessToken = FacebookAuthentication.ExchangeCodeForAccessToken(Request, FacebookAuthenticationOptions.FromWebConfig(), code);

            ActionResult result = null;
            var user = DatabaseSession.Query<UserAccount>().Where(x => x.FacebookId == accessToken.FacebookID).SingleOrDefault();
            if (user == null)
            {
                user = new UserAccount(accessToken);
                result = this.RedirectToAction(c => c.Welcome());
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(redirectURL))
                {
                    result = Redirect(redirectURL);
                }
                else
                {
                    result = Redirect("~");
                }
            }
            user.AddFacebookAccessToken(accessToken);
            DatabaseSession.Save(user);
            DatabaseSession.CommitTransaction();

            //authenticationTokenContext.IssueAuthenticationToken(user.UserAccountId, accessToken, profile.name, "oauth/facebook", expires);

            return result;
        }

        [HttpGet, Route("Welcome")]
        public ActionResult Welcome()
        {
            return View();
        }
    }
}