using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class PersonPhoto
    {
        public virtual int PersonPhotoId { get; set; }
        public virtual Person Person { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
    }

    public class PersonPhotoClassMap : ClassMap<PersonPhoto>
    {
        public PersonPhotoClassMap()
        {
            Id(x => x.PersonPhotoId).GeneratedBy.Identity();
            References(x => x.Person, "PersonId").Not.Nullable();
            References(x => x.Photo, "PhotoId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}