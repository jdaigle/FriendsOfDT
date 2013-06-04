using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.FODT
{
    public class UserAccessToken
    {
        public virtual string UserAccountId { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime ExpiresDateTime { get; set; }
    }

    public class UserAccessTokenClassMap : ClassMap<UserAccessToken>
    {
        public UserAccessTokenClassMap()
        {
            Id(x => x.AccessToken).GeneratedBy.Assigned().CustomType("AnsiString").Length(255);
            Map(x => x.UserAccountId).CustomType("AnsiString").Length(50);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.ExpiresDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}