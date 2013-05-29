using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.Entities
{
    public class PersonAward
    {
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