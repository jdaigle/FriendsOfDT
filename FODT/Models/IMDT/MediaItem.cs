using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class MediaItem
    {
        public virtual int MediaItemId { get; set; }
        public virtual string Path { get; set; }
        public virtual string ThumbnailPath { get; set; }
        public virtual string TinyPath { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
    }

    public class MediaItemClassMap : ClassMap<MediaItem>
    {
        public MediaItemClassMap()
        {
            Schema("imdt");
            Id(x => x.MediaItemId).GeneratedBy.Identity();
            Map(x => x.Path).Not.Nullable().CustomType("AnsiString").Length(100);
            Map(x => x.ThumbnailPath).Not.Nullable().CustomType("AnsiString").Length(100);
            Map(x => x.TinyPath).Not.Nullable().CustomType("AnsiString").Length(100);
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}