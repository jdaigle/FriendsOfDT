using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;
using FODT.Security;

namespace FODT.Models.FODT
{
    public class UserFacebookAccessToken
    {
        protected UserFacebookAccessToken() { }

        public UserFacebookAccessToken(UserAccount user, FacebookAccessToken token)
        {
            this.User = user;
            this.AccessToken = token.AccessToken;
            this.InsertedDateTime = DateTime.UtcNow;
            this.ExpiresDateTime = DateTime.UtcNow.AddSeconds(token.Expires);
        }

        public virtual string AccessToken { get; protected set; }
        public virtual UserAccount User { get; protected set; }
        public virtual DateTime InsertedDateTime { get; protected set; }
        public virtual DateTime ExpiresDateTime { get; protected set; }
    }

    public class UserFacebookAccessTokenClassMap : ClassMap<UserFacebookAccessToken>
    {
        public UserFacebookAccessTokenClassMap()
        {
            Id(x => x.AccessToken).GeneratedBy.Assigned().CustomType("AnsiString").Length(255);
            References(x => x.User, "UserId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.ExpiresDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}