using System;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
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
        public virtual MediaItem MediaItem { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual DateTime LastModifiedDateTime { get; set; }

        public virtual string Fullname
        {
            get
            {
                var name = FirstName.Trim();
                if (!string.IsNullOrWhiteSpace(Honorific))
                {
                    name = Honorific.Trim() + " " + name;
                }
                if (!string.IsNullOrWhiteSpace(MiddleName))
                {
                    name = name + " " + MiddleName.Trim();
                }
                if (!string.IsNullOrWhiteSpace(Nickname))
                {
                    name = name + " '" + Nickname.Trim() + "'";
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    name = name + " " + LastName.Trim();
                }
                if (!string.IsNullOrWhiteSpace(Suffix))
                {
                    name = name + ", " + Suffix.Trim();
                }
                return name.Trim();
            }
        }

        /// <summary>
        /// LastName, FirstName MiddleName
        /// </summary>
        public virtual string SortableName
        {
            get
            {
                var name = FirstName.Trim();
                if (!string.IsNullOrWhiteSpace(MiddleName))
                {
                    name = name + " " + MiddleName.Trim();
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    name = LastName.Trim() + ", " + name;
                }
                return name.Trim();
            }
        }
    }

    public class PersonClassMap : ClassMap<Person>
    {
        public PersonClassMap()
        {
            Id(x => x.PersonId).GeneratedBy.Identity();
            Map(x => x.FirstName).Not.Nullable().Length(50);
            Map(x => x.MiddleName).Not.Nullable().Length(50);
            Map(x => x.LastName).Not.Nullable().Length(50);
            Map(x => x.Honorific).Not.Nullable().Length(50);
            Map(x => x.Suffix).Not.Nullable().Length(50);
            Map(x => x.Nickname).Not.Nullable().Length(100);
            Map(x => x.Biography).Not.Nullable().Length(10000);
            References(x => x.MediaItem, "MediaItemId").Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LastModifiedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}