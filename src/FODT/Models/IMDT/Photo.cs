﻿using System;
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
            LargeFileIsSameAsOriginal = true;
        }

        public const int NoPic = 1;

        public virtual int PhotoId { get; set; }
        public virtual Guid GUID { get; set; }
        public virtual DateTime InsertedDateTime { get; set; }
        public virtual bool LargeFileIsSameAsOriginal { get; set; }

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

        public virtual string GetOriginalFileURL()
        {
            return GetStorageRootPath() + GetOriginalFileName();
        }

        public virtual string GetLargeFileURL()
        {
            return GetStorageRootPath() + GetLargeFileName();
        }

        public virtual string GetThumbnailFileURL()
        {
            return GetStorageRootPath() + GetThumbnailFileName();
        }

        public virtual string GetTinyFileURL()
        {
            return GetStorageRootPath() + GetTinyFileName();
        }

        public virtual string GetOriginalFileName()
        {
            return GUID.ToString() + "-original.jpg";
        }

        public virtual string GetLargeFileName()
        {
            if (LargeFileIsSameAsOriginal)
            {
                return GUID.ToString() + "-original.jpg";
            }
            else
            {
                return GUID.ToString() + "-large.jpg";
            }
        }

        public virtual string GetThumbnailFileName()
        {
            return GUID.ToString() + "-thumbnail.jpg";
        }

        public virtual string GetTinyFileName()
        {
            return GUID.ToString() + "-tiny.jpg";
        }

        public virtual bool IsDefaultNoPic()
        {
            return this.PhotoId == NoPic;
        }
    }

    public class PhotoClassMap : ClassMap<Photo>
    {
        public PhotoClassMap()
        {
            Id(x => x.PhotoId).GeneratedBy.Identity();
            Map(x => x.GUID).Not.Nullable();
            Map(x => x.InsertedDateTime).Not.Nullable().CustomType<UtcDateTimeUserType>();
            Map(x => x.LargeFileIsSameAsOriginal).Not.Nullable();
        }
    }
}