using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.Entities
{
    public class ShowCrew
    {
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
            Map(x => x.Position).Not.Nullable().CustomType("AnsiString").Length(75);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}