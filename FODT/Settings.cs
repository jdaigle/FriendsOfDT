using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FODT
{
    public class Settings
    {
        private static string facebook_appid = ConfigurationManager.AppSettings["facebook_appid"];
        public static string Facebook_AppId
        {
            get { return facebook_appid; }
            set { facebook_appid = value; }
        }

        private static string facebook_appsecret = ConfigurationManager.AppSettings["facebook_appsecret"];
        public static string Facebook_AppSecret
        {
            get { return facebook_appsecret; }
            set { facebook_appsecret = value; }
        }

        private static string facebook_login_siteurl = ConfigurationManager.AppSettings["facebook_login_siteurl"];
        public static string Facebook_Login_SiteURL
        {
            get { return facebook_login_siteurl; }
            set { facebook_login_siteurl = value; }
        }
    }
}