using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.Entities
{
    public class Award
    {
        public virtual int AwardId { get; set; }
        public virtual string Name { get; set; }
    }

    public class AwardClassMap : ClassMap<Award>
    {
        public AwardClassMap()
        {
            Id(x => x.AwardId).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().CustomType("AnsiString").Length(50);
        }
    }
}