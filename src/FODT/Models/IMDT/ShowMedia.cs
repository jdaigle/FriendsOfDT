using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class ShowMedia
    {
        public virtual int ShowMediaId { get; set; }
        public virtual Show Show { get; set; }
        public virtual MediaItem MediaItem { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
    }

    public class ShowMediaClassMap : ClassMap<ShowMedia>
    {
        public ShowMediaClassMap()
        {
            Id(x => x.ShowMediaId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Not.Nullable();
            References(x => x.MediaItem, "MediaItemId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}