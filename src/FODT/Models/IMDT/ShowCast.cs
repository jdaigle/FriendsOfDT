using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class ShowCast
    {
        public ShowCast() { }
        public ShowCast(Person person, Show show, string role)
        {
            this.Person = person;
            this.Show = show;
            this.Role = role;
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentNullException(nameof(role));
            }
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
        }

        public virtual int ShowCastId { get; set; }
        public virtual Show Show { get; set; }
        public virtual Person Person { get; set; }
        public virtual string Role { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class CastClassMap : ClassMap<ShowCast>
    {
        public CastClassMap()
        {
            Id(x => x.ShowCastId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Not.Nullable();
            References(x => x.Person, "PersonId").Not.Nullable();
            Map(x => x.Role).Not.Nullable().Length(75);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}