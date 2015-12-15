using System;
using System.Collections;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class Show : IComparable<Show>
    {
        public virtual int ShowId { get; set; }
        public virtual string Title { get; set; }

        public virtual string DisplayTitle { get { return ExtensionMethods.RearrangeShowTitle(this.Title); } }

        public virtual string Author { get; set; }
        public virtual Quarter Quarter { get; set; }
        public virtual short Year { get; set; }
        public virtual string Pictures { get; set; }
        public virtual string FunFacts { get; set; }
        public virtual string Toaster { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }

        public virtual int CompareTo(Show other)
        {
            return ShowComparer.ChronologicalShowComparison(this, other);
        }
    }

    public class ShowClassMap : ClassMap<Show>
    {
        public ShowClassMap()
        {
            Id(x => x.ShowId).GeneratedBy.Identity();
            Map(x => x.Title).Not.Nullable().Length(50);
            Map(x => x.Author).Not.Nullable().Length(50);
            Map(x => x.Quarter).Not.Nullable().CustomType<Quarter>();
            Map(x => x.Year).Not.Nullable();
            Map(x => x.Pictures).Not.Nullable().Length(100);
            Map(x => x.FunFacts).Not.Nullable().Length(10000);
            Map(x => x.Toaster).Not.Nullable().Length(10000);
            References(x => x.Photo, "PhotoId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}