﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class MediaItem
    {
        public MediaItem()
        {
            GUID = Guid.NewGuid();
            InsertedDateTime = DateTime.UtcNow;
        }

        public const int NoPic = 1;

        public virtual int MediaItemId { get; set; }
        public virtual Guid GUID { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }

        public virtual string GetOriginalFileName()
        {
            return GUID.ToString() + "-original.jpg";
        }

        public virtual string GetThumbnailFileName()
        {
            return GUID.ToString() + "-thumbnail.jpg";
        }

        public virtual string GetTinyFileName()
        {
            return GUID.ToString() + "-tiny.jpg";
        }
    }

    public class MediaItemClassMap : ClassMap<MediaItem>
    {
        public MediaItemClassMap()
        {
            Id(x => x.MediaItemId).GeneratedBy.Identity();
            Map(x => x.GUID).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}