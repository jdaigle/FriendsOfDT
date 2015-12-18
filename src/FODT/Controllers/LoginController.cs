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
using FODT.Views.Login;

namespace FODT.Controllers
{
    [NoCache]
    public class LoginController : BaseController
    {
        [HttpGet, Route("~/Login")]
        public ActionResult Index()
        {
            return View(new LoginViewModel()
            {
                FacebookOAuthChallengeURL = this.GetURL(c => c.FacebookOAuthChallenge("")),
            });
        }

        [HttpGet, Route("~/SignOut")]
        public ActionResult SignOut()
        {
            //authenticationTokenContext.RevokeAuthenticationToken();
            return Redirect("~");
        }

        [HttpGet, Route("~/oauth/facebook/challenge")]
        public ActionResult FacebookOAuthChallenge(string redirectUrl = "")
        {
            var url = FacebookAuthentication.GetAuthChallengeURL(Request, FacebookAuthenticationOptions.FromWebConfig());
            return Redirect(url);
        }

        [HttpGet, Route("~/oauth/facebook")]
        public ActionResult HandleFacebookOAuthCallback(string code, string state)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return this.RedirectToAction(c => c.Index());
            }

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
                //result = this.RedirectToAction(c => c.Welcome());
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
            var token = user.AddFacebookAccessToken(accessToken);
            DatabaseSession.Save(user);
            DatabaseSession.Flush();

            var tokenID = token.UserFacebookAccessTokenId;

            //authenticationTokenContext.IssueAuthenticationToken(user.UserAccountId, accessToken, profile.name, "oauth/facebook", expires);

            return result;
        }
    }
}