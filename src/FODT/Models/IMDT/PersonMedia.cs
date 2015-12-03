using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class PersonMedia
    {
        public virtual int PersonMediaId { get; set; }
        public virtual Person Person { get; set; }
        public virtual MediaItem MediaItem { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
    }

    public class PersonMediaClassMap : ClassMap<PersonMedia>
    {
        public PersonMediaClassMap()
        {
            Id(x => x.PersonMediaId).GeneratedBy.Identity();
            References(x => x.Person, "PersonId").Not.Nullable();
            References(x => x.MediaItem, "MediaItemId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}