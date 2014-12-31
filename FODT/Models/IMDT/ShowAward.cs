using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class ShowAward
    {
        public ShowAward() { }
        public ShowAward(Show show, Person person, Award award, short year)
        {
            this.Show = show;
            this.Person = person;
            this.Award = award;
            this.Year = year;
            if (year < 1940 || year > (DateTime.Now.Year + 1))
            {
                throw new ArgumentOutOfRangeException("year");
            }
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
        }

        public virtual int ShowAwardId { get; set; }
        public virtual Show Show { get; set; }
        public virtual Person Person { get; set; }
        public virtual Award Award { get; set; }
        public virtual short Year { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class ShowAwardClassMap : ClassMap<ShowAward>
    {
        public ShowAwardClassMap()
        {
            Id(x => x.ShowAwardId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Not.Nullable();
            References(x => x.Person, "PersonId").Nullable();
            References(x => x.Award, "AwardId").Not.Nullable();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}