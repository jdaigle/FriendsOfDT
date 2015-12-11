using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;
using FODT.Security;

namespace FODT.Models.FODT
{
    public class UserAccount
    {
        protected UserAccount()
        {
            FacebookAccessTokens = new HashSet<UserFacebookAccessToken>();
        }

        public UserAccount(FacebookAccessToken token)
            :this()
        {
            FacebookId = token.FacebookID;
            InsertedDateTime = DateTime.UtcNow;
            LastModifiedDateTime = DateTime.UtcNow;
            Update(token);
        }

        public virtual void AddFacebookAccessToken(FacebookAccessToken token)
        {
            FacebookAccessTokens.Add(new UserFacebookAccessToken(this, token));
            Update(token);
        }

        public virtual void Update(FacebookAccessToken token)
        {
            if (!token.Name.IsNullOrWhiteSpace())
            {
                Name = token.Name.Trim();
                LastModifiedDateTime = DateTime.UtcNow;
            }
            if (!token.Email.IsNullOrWhiteSpace())
            {
                Email = token.Email.Trim();
                LastModifiedDateTime = DateTime.UtcNow;
            }
        }

        public virtual int UserAccountId { get; protected set; }
        public virtual string Email { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual int? FacebookId { get; protected set; }
        public virtual DateTime InsertedDateTime { get; protected set; }
        public virtual DateTime LastModifiedDateTime { get; protected set; }

        public virtual ISet<UserFacebookAccessToken> FacebookAccessTokens { get; protected set; }
    }

    public class UserAccountClassMap : ClassMap<UserAccount>
    {
        public UserAccountClassMap()
        {
            Id(x => x.UserAccountId).GeneratedBy.Identity();
            Map(x => x.Email).Not.Nullable().Length(300);
            Map(x => x.Name).Not.Nullable().Length(300);
            Map(x => x.FacebookId).Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            HasMany(x => x.FacebookAccessTokens)
                .AsBag()
                .Inverse();
        }
    }
}