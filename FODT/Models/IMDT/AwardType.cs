using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class AwardType
    {
        public virtual int AwardTypeId { get; set; }
        public virtual string Name { get; set; }
    }

    public class AwardClassMap : ClassMap<AwardType>
    {
        public AwardClassMap()
        {
            Id(x => x.AwardTypeId).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Length(50);
        }
    }
}