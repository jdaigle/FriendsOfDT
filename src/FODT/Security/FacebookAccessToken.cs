using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace FODT.Security
{
    public class FacebookAccessToken
    {
        public FacebookAccessToken(string accessToken, int expires, JObject userInfo)
        {
            AccessToken = accessToken;
            Expires = expires;
            FacebookID = int.Parse(TryGetValue(userInfo, "id"));
            Name = TryGetValue(userInfo, "name");
            FirstName = TryGetValue(userInfo, "first_name");
            LastName = TryGetValue(userInfo, "last_name");
            Email = TryGetValue(userInfo, "email");
        }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }

        public string AccessToken { get; }
        public int Expires { get; }
        public int FacebookID { get; }
        public string Name { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
    }
}