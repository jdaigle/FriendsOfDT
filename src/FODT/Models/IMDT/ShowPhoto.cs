using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class ShowPhoto
    {
        public virtual int ShowPhotoId { get; set; }
        public virtual Show Show { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
    }

    public class ShowPhotoClassMap : ClassMap<ShowPhoto>
    {
        public ShowPhotoClassMap()
        {
            Id(x => x.ShowPhotoId).GeneratedBy.Identity();
            References(x => x.Show, "ShowId").Not.Nullable();
            References(x => x.Photo, "PhotoId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}