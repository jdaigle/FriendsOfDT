using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class PersonAward
    {
        public PersonAward() { }
        public PersonAward(Person person, Award award, short year)
        {
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

        public virtual int PersonAwardId { get; set; }
        public virtual Person Person { get; set; }
        public virtual Award Award { get; set; }
        public virtual short Year { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class PersonAwardClassMap : ClassMap<PersonAward>
    {
        public PersonAwardClassMap()
        {
            Id(x => x.PersonAwardId).GeneratedBy.Identity();
            References(x => x.Person, "PersonId").Not.Nullable();
            References(x => x.Award, "AwardId").Not.Nullable();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}