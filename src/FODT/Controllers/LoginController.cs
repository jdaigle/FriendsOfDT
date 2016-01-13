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
        public ActionResult Login()
        {
            return View(new LoginViewModel()
            {
                FacebookOAuthChallengeURL = this.GetURL(c => c.FacebookOAuthChallenge("")),
            });
        }

        [HttpGet, Route("~/SignOut")]
        public ActionResult SignOut()
        {
            HttpContext.Get<IAuthenticationManager>().SignOut();
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
                return this.RedirectToAction(c => c.Login());
            }

            // TODO, decrypt redirectURL
            var redirectURL = string.Empty;
            if (!string.IsNullOrWhiteSpace(state))
            {
                redirectURL = HttpUtility.UrlDecode(state);
            }

            var facebookAccessToken = FacebookAuthentication.ExchangeCodeForAccessToken(Request, FacebookAuthenticationOptions.FromWebConfig(), code);

            var user = DatabaseSession.Query<UserAccount>().Where(x => x.FacebookId == facebookAccessToken.FacebookID).SingleOrDefault();
            if (user == null)
            {
                user = new UserAccount(facebookAccessToken);
                // TODO: redirect to a welcome page to confirm info redirectURL = this.GetURL<>
            }
            user.UpdateSeen();
            var tokenEntity = user.AddFacebookAccessToken(facebookAccessToken);
            DatabaseSession.Save(user);
            DatabaseSession.Flush();
            var tokenID = tokenEntity.UserFacebookAccessTokenId;

            HttpContext.Get<IAuthenticationManager>().SignIn(tokenID.ToString(), FacebookAuthentication.AuthenticationType);

            if (redirectURL.IsNullOrWhiteSpace())
            {
                redirectURL = "~";
            }
            return Redirect(redirectURL);
        }
    }
}