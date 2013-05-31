using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.FODT;
using FODT.Security;
using Newtonsoft.Json;

namespace FODT.Controllers
{
    [RoutePrefix("User")]
    public partial class UserController : BaseController
    {
        private IAuthenticationTokenContext authenticationTokenContext;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            authenticationTokenContext = new HttpAuthenticationTokenContext(this.HttpContext, new HttpCookieCollectionWrapper(this.HttpContext));
            base.OnActionExecuting(filterContext);
        }

        [GET("SignIn")]
        public virtual ActionResult SignIn(string redirectUrl)
        {
            var url =
                string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope=email&state={2}",
                Settings.Facebook_AppId,
                GenerateFacbookOAuthResponseURL(),
                HttpUtility.UrlEncode(redirectUrl ?? string.Empty));
            return Redirect(url);
        }

        [GET("SignOut")]
        public virtual ActionResult SignOut()
        {
            authenticationTokenContext.RevokeAuthenticationToken();
            return RedirectToAction(MVC.Home.Welcome());
        }

        private string GenerateFacbookOAuthResponseURL()
        {
            return Settings.Facebook_Login_SiteURL + Url.Action(Actions.HandleFacebookOAuthCallback());
        }

        [GET("oauth/facebook")]
        public virtual ActionResult HandleFacebookOAuthCallback(string code, string state)
        {
            var redirectURL = string.Empty;
            if (!string.IsNullOrWhiteSpace(state))
            {
                redirectURL = HttpUtility.UrlDecode(state);
            }

            var requestUri =
                string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                Settings.Facebook_AppId,
                GenerateFacbookOAuthResponseURL(),
                Settings.Facebook_AppSecret,
                code);
            var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            var response = ReadResponse(request.GetResponse());
            var responseAccessToken = HttpUtility.ParseQueryString(response);

            var accessToken = responseAccessToken["access_token"];
            var expires = int.Parse(responseAccessToken["expires"]);

            var profile = GetFacebookProfile(accessToken);
            ActionResult result = null;
            var user = DatabaseSession.Get<UserAccount>(profile.id);
            if (user == null)
            {
                user = new UserAccount(profile);
                DatabaseSession.Save(user);
                result = RedirectToAction(Actions.Welcome());
            }
            else
            {
                user.Update(profile);
                if (DatabaseSession.IsDirtyEntity(user))
                {
                    // TODO: audit stuff is built-in?
                    user.LastModifiedDateTime = DateTime.UtcNow;
                }
                if (!string.IsNullOrWhiteSpace(redirectURL))
                {
                    result = Redirect(redirectURL);
                }
                else
                {
                    result = RedirectToAction(MVC.Home.Welcome());
                }
            }
            var _accessToken = new UserAccessToken();
            _accessToken.UserAccountId = user.UserAccountId;
            _accessToken.AccessToken = accessToken;
            _accessToken.InsertedDateTime = DateTime.UtcNow;
            _accessToken.ExpiresDateTime = DateTime.UtcNow.AddSeconds(expires);
            DatabaseSession.Save(_accessToken);

            DatabaseSession.CommitTransaction();

            authenticationTokenContext.IssueAuthenticationToken(accessToken, profile.name, "oauth/facebook", expires);

            return result;
        }

        [GET("Welcome")]
        public virtual ActionResult Welcome()
        {
            return View();
        }

        private FacebookProfile GetFacebookProfile(string accessToken)
        {
            var requestUri = string.Format("https://graph.facebook.com/me?fields=id,name,first_name,middle_name,last_name,link,username,gender,locale,age_range,email,picture.type(large)&access_token={0}", accessToken);
            var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            var response = ReadResponse(request.GetResponse());


            return JsonConvert.DeserializeObject<FacebookProfile>(response);
        }

        private static string ReadResponse(WebResponse response)
        {
            var httpResponse = (HttpWebResponse)response;
            using (var resStream = new StreamReader(httpResponse.GetResponseStream(), Encoding.ASCII))
            {
                return resStream.ReadToEnd();
            }
        }
    }
}