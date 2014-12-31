using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class PersonClubPosition
    {
        public PersonClubPosition() { }
        public PersonClubPosition(Person person, string position, short year)
        {
            this.Person = person;
            this.Position = position;
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentNullException("position");
            }
            this.Year = year;
            if (year < 1940 || year > (DateTime.Now.Year + 1))
            {
                throw new ArgumentOutOfRangeException("year");
            }
            this.DisplayOrder = 0;
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
        }

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
            Map(x => x.Position).Not.Nullable().Length(75);
            Map(x => x.DisplayOrder).Not.Nullable();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}