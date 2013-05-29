using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.Entities
{
    public class PersonClubPosition
    {
        public virtual int PersonClubPositionId { get; set; }
        public virtual Person Person { get; set; }
        public virtual string Position { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual short Year { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class PersonClubPositionClassMap : ClassMap<PersonClubPosition>
    {
        public PersonClubPositionClassMap()
        {
            Id(x => x.PersonClubPositionId).GeneratedBy.Identity();
            References(x => x.Person, "PersonId").Not.Nullable();
            Map(x => x.Position).Not.Nullable().CustomType("AnsiString").Length(75);
            Map(x => x.DisplayOrder).Not.Nullable();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}