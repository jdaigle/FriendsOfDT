using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.FODT
{
    public class UserAccount
    {
        public UserAccount() { }

        public UserAccount(FacebookProfile profile)
        {
            this.UserAccountId = profile.id;
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
            Update(profile);
        }

        public virtual void Update(FacebookProfile profile)
        {
            this.Email = profile.email ?? string.Empty;
            this.Name = profile.name ?? string.Empty;
            this.FirstName = profile.first_name ?? string.Empty;
            this.MiddleName = profile.middle_name ?? string.Empty;
            this.LastName = profile.last_name ?? string.Empty;
            this.Gender = profile.gender ?? string.Empty;
            this.Locale = profile.locale ?? string.Empty;
            this.FacebookURL = profile.link ?? string.Empty;
            this.FacebookUsername = profile.username ?? string.Empty;
            if (profile.picture != null && profile.picture.data != null && !profile.picture.data.is_silhouette)
            {
                this.FacebookPictureURL = profile.picture.data.url ?? string.Empty;
            }
            else
            {
                this.FacebookPictureURL = string.Empty;
            }
        }

        public virtual string UserAccountId { get; set; }
        public virtual string Email { get; set; }
        public virtual string Name { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Gender { get; set; }
        public virtual string Locale { get; set; }
        public virtual string FacebookURL { get; set; }
        public virtual string FacebookUsername { get; set; }
        public virtual string FacebookPictureURL { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class UserAccountClassMap : ClassMap<UserAccount>
    {
        public UserAccountClassMap()
        {
            Schema("fodt");
            Id(x => x.UserAccountId).GeneratedBy.Assigned().CustomType("AnsiString").Length(50);
            Map(x => x.Email).Not.Nullable().Length(300);
            Map(x => x.Name).Not.Nullable().Length(300);
            Map(x => x.FirstName).Not.Nullable().Length(100);
            Map(x => x.MiddleName).Not.Nullable().Length(100);
            Map(x => x.LastName).Not.Nullable().Length(100);
            Map(x => x.Gender).Not.Nullable().Length(10);
            Map(x => x.Locale).Not.Nullable().Length(10);
            Map(x => x.FacebookURL).Not.Nullable().Length(300);
            Map(x => x.FacebookUsername).Not.Nullable().Length(300);
            Map(x => x.FacebookPictureURL).Not.Nullable().Length(300);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}