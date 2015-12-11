using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FODT.Security
{
    public class FacebookAuthenticationOptions
    {
        private static FacebookAuthenticationOptions _fromWebConfig;

        public static FacebookAuthenticationOptions FromWebConfig()
        {
            if (_fromWebConfig == null)
            {
                _fromWebConfig = new FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings["facebook.appid"],
                    AppSecret = ConfigurationManager.AppSettings["facebook.appsecret"],
                };
            }
            return _fromWebConfig;
        }

        /// <summary>
        /// Gets or sets the Facebook-assigned appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the Facebook-assigned app secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// Default value is "/oauth/facebook".
        /// </summary>
        public string CallbackPath { get; set; } = "/oauth/facebook";

        /// <summary>
        /// A comma-seperated list of permissions to request.
        /// </summary>
        public string Scope { get; set; } = "public_profile,email";
    }
}
