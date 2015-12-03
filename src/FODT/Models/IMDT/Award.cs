using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class Award
    {
        public Award() { }
        public Award(Show show, Person person, AwardType awardType, short year)
        {
            this.Show = show;
            this.Person = person;
            if (Show == null && Person == null)
            {
                throw new ArgumentException("Show or Person is required");
            }
            this.AwardType = awardType;
            this.Year = year;
            if (year < 1940 || year > (DateTime.Now.Year + 1))
            {
                throw new ArgumentOutOfRangeException("year");
            }
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
        }

        public virtual int AwardId { get; set; }
        public virtual Show Show { get; set; }
        public virtual Person Person { get; set; }
        public virtual AwardType AwardType { get; set; }
        public virtual short Year { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class ShowAwardClassMap : ClassMap<Award>
    {
        public ShowAwardClassMap()
        {
            Id(x => x.AwardId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Nullable();
            References(x => x.Person, "PersonId").Nullable();
            References(x => x.AwardType, "AwardTypeId").Not.Nullable();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}