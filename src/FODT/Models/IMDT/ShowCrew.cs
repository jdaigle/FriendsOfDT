using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class ShowCrew
    {
        public ShowCrew() { }
        public ShowCrew(Person person, Show show, string position)
        {
            this.Person = person;
            this.Show = show;
            this.Position = position;
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentNullException(nameof(position));
            }
            this.DisplayOrder = 0;
            this.InsertedDateTime = DateTime.UtcNow;
            this.LastModifiedDateTime = DateTime.UtcNow;
        }

        public virtual int ShowCrewId { get; set; }
        public virtual Show Show { get; set; }
        public virtual Person Person { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual string Position { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class CrewClassMap : ClassMap<ShowCrew>
    {
        public CrewClassMap()
        {
            Id(x => x.ShowCrewId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Not.Nullable();
            References(x => x.Person, "PersonId").Not.Nullable();
            Map(x => x.DisplayOrder).Not.Nullable();
            Map(x => x.Position).Not.Nullable().Length(75);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}