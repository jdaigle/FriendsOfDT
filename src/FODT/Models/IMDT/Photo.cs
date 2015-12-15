using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using FODT.Database;

namespace FODT.Models.IMDT
{
    public class Photo
    {
        public Photo()
        {
            GUID = Guid.NewGuid();
            InsertedDateTime = DateTime.UtcNow;
        }

        public const int NoPic = 1;

        public virtual int PhotoId { get; set; }
        public virtual Guid GUID { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }

        private static string azureStorageBaseURL;

        public static string GetStorageRootPath()
        {
            if (azureStorageBaseURL.IsNullOrWhiteSpace())
            {
                var azureStorageAccountName = ConfigurationManager.AppSettings["azure-storage-account-name"];
                azureStorageBaseURL = "https://" + azureStorageAccountName + ".blob.core.windows.net/" + ConfigurationManager.AppSettings["azure-storage-blob-container"] + "/";
            }
            return azureStorageBaseURL;
        }

        public virtual string GetURL()
        {
            return GetStorageRootPath() + GetOriginalFileName();
        }

        public virtual string GetThumbnailURL()
        {
            return GetStorageRootPath() + GetThumbnailFileName();
        }

        public virtual string GetTinyURL()
        {
            return GetStorageRootPath() + GetTinyFileName();
        }

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

    public class PhotoClassMap : ClassMap<Photo>
    {
        public PhotoClassMap()
        {
            Id(x => x.PhotoId).GeneratedBy.Identity();
            Map(x => x.GUID).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
        }
    }
}