using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.Entities
{
    public class Person
    {
        public virtual int PersonId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Honorific { get; set; }
        public virtual string Suffix { get; set; }
        public virtual string Nickname { get; set; }
        public virtual string Biography { get; set; }
        public virtual int MediaId { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }
    }

    public class PersonClassMap : ClassMap<Person>
    {
        public PersonClassMap()
        {
            Id(x => x.PersonId).GeneratedBy.Identity();
            Map(x => x.FirstName).Not.Nullable().CustomType("AnsiString").Length(50);
            Map(x => x.MiddleName).Not.Nullable().CustomType("AnsiString").Length(50);
            Map(x => x.LastName).Not.Nullable().CustomType("AnsiString").Length(50);
            Map(x => x.Honorific).Not.Nullable().CustomType("AnsiString").Length(50);
            Map(x => x.Suffix).Not.Nullable().CustomType("AnsiString").Length(50);
            Map(x => x.Nickname).Not.Nullable().CustomType("AnsiString").Length(100);
            Map(x => x.Biography).Not.Nullable().CustomType("AnsiString").Length(10000);
            Map(x => x.MediaId).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}