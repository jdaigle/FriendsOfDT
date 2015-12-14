using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using Dapper;
using System.Net;
using System.Drawing.Imaging;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.Drawing;

namespace FODT.DataMigration
{
    public class Program
    {
        private static NHibernate.ISessionFactory sessionFactory;
        private static DbConnection oldDatabaseConnection;

        private static string azureStorageBaseURL = "";
        private static string azureStorageAccountName = "";
        private static string azureStorageAccountKey = "";

        private static bool doDatabaseImport =  true;
        private static bool doBlobUpload = false;

        private static readonly Encoding blobEncoding = Encoding.GetEncoding(1252);

        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            azureStorageAccountName = ConfigurationManager.AppSettings["azure-storage-account-name"];
            azureStorageAccountKey = ConfigurationManager.AppSettings["azure-storage-account-key"];
            azureStorageBaseURL = "https://" + azureStorageAccountName + ".blob.core.windows.net/" + ConfigurationManager.AppSettings["azure-storage-blob-container"] + "/";

            var connectionString = ConfigurationManager.ConnectionStrings["fodt"].ConnectionString;
            var cfg = DatabaseBootstrapper.Bootstrap(connectionString);
            sessionFactory = cfg.BuildSessionFactory();

            oldDatabaseConnection = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["old_fodt"].ConnectionString);
            oldDatabaseConnection.Open();

            if (doDatabaseImport)
            {
                TruncateDatabase();
                ImportPhotos();
                ImportAwardsList();
                ImportPersons();
                ImportPersonPhotos();
                ImportShows();
                ImportShowPhotos();
                ImportAwards();
                ImportCast();
                ImportCrew();
                ImportEC();
            }
            if (doBlobUpload)
            {
                ImportPhotoBlobs();
            }
        }

        private static void TruncateDatabase()
        {
            using (var session = sessionFactory.OpenSession())
            {
                session.Connection.Execute(@"
DELETE FROM ShowPhoto;
DELETE FROM ShowCrew;
DELETE FROM ShowCast;
DELETE FROM Award;
DELETE FROM Show;
DELETE FROM PersonPhoto;
DELETE FROM PersonClubPosition;
DELETE FROM Person;
DELETE FROM Photo;
DELETE FROM AwardType;
");
            }
        }

        private static void Log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + ": " + msg);
        }

        private static void ImportShows()
        {
            var shows = oldDatabaseConnection.Query("SELECT * FROM shows").ToList();
            var mediaLookup = oldDatabaseConnection.Query("SELECT * FROM media")
                .Select(x => new { media_id = (int)x.ID, mediaItem_id = (int)x.item_id })
                .ToDictionary(x => x.media_id);

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Show ON").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in shows)
                {
                    if (string.IsNullOrWhiteSpace(_row.title))
                    {
                        // no title? no show
                        continue;
                    }
                    var entity = new Show();
                    entity.ShowId = _row.ID;
                    entity.Title = (_row.title ?? string.Empty).Trim();
                    if (_row.quarter != null)
                    {
                        entity.Quarter = (Quarter)(byte)_row.quarter;
                    }
                    else
                    {
                        // TODO?
                    }
                    entity.Author = (_row.author ?? string.Empty).Trim();
                    if (_row.year != null)
                    {
                        entity.Year = (short)_row.year;
                    }
                    else
                    {
                        // TODO?
                    }
                    entity.Pictures = (_row.pictures ?? string.Empty).Trim();

                    entity.FunFacts = "";
                    if (_row.funfacts != null && _row.funfacts.Length > 0)
                    {
                        entity.FunFacts = blobEncoding.GetString(_row.funfacts);
                    }
                    entity.Toaster = "";
                    if (_row.toaster != null && _row.toaster.Length > 0)
                    {
                        entity.Toaster = blobEncoding.GetString(_row.toaster);
                    }

                    if (_row.media_id != null)
                    {
                        entity.Photo = session.Load<Photo>(mediaLookup[_row.media_id].mediaItem_id);
                    }
                    else
                    {
                        entity.Photo = session.Load<Photo>(1); // default to nopic
                    }
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.ShowId);
                    if (entity.ShowId > maxId) maxId = entity.ShowId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Show OFF").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.Show', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportPersons()
        {
            var people = oldDatabaseConnection.Query("SELECT * FROM people").ToList();
            var mediaLookup = oldDatabaseConnection.Query("SELECT * FROM media")
                .Select(x => new { media_id = (int)x.ID, mediaItem_id = (int)x.item_id })
                .ToDictionary(x => x.media_id);
            Log("Importing " + people.Count + " People");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Person ON").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in people)
                {
                    var entity = new Person();
                    entity.PersonId = (int)_row.ID;
                    entity.Honorific = (_row.hon ?? string.Empty).Trim();
                    entity.FirstName = (_row.fname ?? string.Empty).Trim();
                    entity.MiddleName = (_row.mname ?? string.Empty).Trim();
                    entity.LastName = (_row.lname ?? string.Empty).Trim();
                    entity.Suffix = (_row.suffix ?? string.Empty).Trim();
                    entity.Nickname = (_row.nickname ?? string.Empty).Trim();
                    if (entity.FirstName.Length == 0 && entity.LastName.Length == 0)
                    {
                        continue; // skip
                    }
                    entity.Biography = "";
                    if (_row.bio != null && _row.bio.Length > 0)
                    {
                        entity.Biography = blobEncoding.GetString(_row.bio);
                    }
                    if (mediaLookup.ContainsKey((int)_row.media_id))
                    {
                        entity.Photo = session.Load<Photo>(mediaLookup[(int)_row.media_id].mediaItem_id);
                    }
                    else
                    {
                        entity.Photo = session.Load<Photo>(1); // default to nopic
                    }
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.PersonId);
                    if (entity.PersonId > maxId) maxId = entity.PersonId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Person OFF").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.Person', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportPhotos()
        {
            var media_items = oldDatabaseConnection.Query("SELECT * FROM media_items").ToList();
            Log("Importing " + media_items.Count + " Media Items");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].[Photo] ON").ExecuteUpdate();
                var maxId = 0;
                var count = 0;
                foreach (var _row in media_items)
                {
                    var entity = new Photo();
                    if (_row.guid == null)
                    {
                        throw new InvalidOperationException("Media Item is Missing GUID. Row needs to be updated before Importing");
                    }
                    Guid guid = _row.guid;
                    entity.PhotoId = (int)_row.ID;
                    entity.GUID = (Guid)_row.guid;
                    entity.InsertedDateTime = DateTime.MinValue;
                    if (_row.lastmod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)_row.lastmod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.PhotoId);
                    if (entity.PhotoId > maxId) maxId = entity.PhotoId;
                    count++;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].[Photo] OFF").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.[Photo]', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
                Log("Imported " + count + " Photos");
            }
        }

        private static void ImportPhotoBlobs()
        {
            var rootPhotoPath = @"http://imdt.friendsofdt.org/";

            var media_items = oldDatabaseConnection.Query("SELECT * FROM media_items").ToList();
            Log("Importing " + media_items.Count + " Media Blobs");

            var count = 0;
            foreach (var _row in media_items)
            {
                var entity = new Photo();
                if (_row.guid == null)
                {
                    throw new InvalidOperationException("Media Item is Missing GUID. Row needs to be updated before Importing");
                }
                Guid guid = _row.guid;
                entity.PhotoId = (int)_row.ID;
                entity.GUID = (Guid)_row.guid;

                byte[] original = null;
                original = AzureBlogStorageUtil.DownloadPublicBlob(rootPhotoPath + _row.item.ToString().Replace("./", ""));

                if (original == null)
                {
                    Log("Missing File!" + _row.item.ToString());
                    continue;
                }

                var fullSize = ImageUtilities.LoadBitmap(original);
                var thumbnail = ImageUtilities.GetBytes(ImageUtilities.Resize(fullSize, 240, 240), ImageFormat.Jpeg);
                var tiny = ImageUtilities.GetBytes(ImageUtilities.Resize(fullSize, 50, 50), ImageFormat.Jpeg);

                PutBlob(entity.GetOriginalFileName(), original);
                PutBlob(entity.GetThumbnailFileName(), thumbnail);
                PutBlob(entity.GetTinyFileName(), tiny);

                count++;
                if (count % 100 == 0)
                {
                    Log("Imported " + count + " Photo Blobs");

                }
            }
            Log("Imported " + count + " Photo Blobs");
        }

        private static void PutBlob(string name, byte[] buffer)
        {
            if (!AzureBlogStorageUtil.BlobExists(azureStorageBaseURL + name))
            {
                AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + name, azureStorageAccountName, azureStorageAccountKey, buffer, "image/jpeg");
            }
        }

        private static void ImportAwardsList()
        {
            var awards = oldDatabaseConnection.Query("SELECT * FROM awards_list").ToList();
            Log("Importing " + awards.Count + " awards");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].AwardType ON").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in awards)
                {
                    var entity = new AwardType();
                    entity.AwardTypeId = (int)_row.ID;
                    entity.Name = ((string)_row.name ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(entity.Name))
                    {
                        continue; // skip blank names
                    }
                    session.Save(entity, entity.AwardTypeId);
                    if (entity.AwardTypeId > maxId) maxId = entity.AwardTypeId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].AwardType OFF").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.AwardType', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportPersonPhotos()
        {
            var media = oldDatabaseConnection.Query("SELECT * FROM media").ToList();
            Log("Importing " + media.Count + " person media");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].PersonPhoto ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in media)
                {
                    if (_row.IDtype.ToLower() != "people")
                    {
                        continue;
                    }
                    var entity = new PersonPhoto();
                    entity.PersonPhotoId = _row.ID;
                    entity.Person = session.Load<Person>(_row.assocID);
                    entity.Photo = session.Load<Photo>(_row.item_id);
                    entity.InsertedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.PersonPhotoId);
                    if (entity.PersonPhotoId > maxId) maxId = entity.PersonPhotoId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].PersonPhoto OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.PersonPhoto', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportShowPhotos()
        {
            var media = oldDatabaseConnection.Query("SELECT * FROM media").ToList();
            Log("Importing " + media.Count + " show media");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowPhoto ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in media)
                {
                    if (_row.IDtype.ToLower() != "shows")
                    {
                        continue;
                    }
                    var entity = new ShowPhoto();
                    entity.ShowPhotoId = _row.ID;
                    entity.Show = session.Load<Show>(_row.assocID);
                    entity.Photo = session.Load<Photo>(_row.item_id);
                    entity.InsertedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.ShowPhotoId);
                    if (entity.ShowPhotoId > maxId) maxId = entity.ShowPhotoId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowPhoto OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.ShowPhoto', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportAwards()
        {
            var awards = oldDatabaseConnection.Query("SELECT * FROM awards").ToList();
            Log("Importing " + awards.Count + " show awards");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Award ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in awards)
                {
                    if (_row.showID == null && _row.peepID == null)
                    {
                        continue;
                    }
                    var entity = new Award();
                    entity.AwardId = _row.ID;
                    if (_row.showID != null)
                    {
                        entity.Show = session.Load<Show>(_row.showID);
                    }
                    if (_row.peepID != null)
                    {
                        entity.Person = session.Load<Person>(_row.peepID);
                    }
                    entity.AwardType = session.Load<AwardType>((int)_row.awardID);
                    entity.Year = (short)_row.year;
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.AwardId);
                    if (entity.AwardId > maxId) maxId = entity.AwardId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].Award OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.Award', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportCast()
        {
            var cast = oldDatabaseConnection.Query("SELECT * FROM cast").ToList();
            Log("Importing " + cast.Count + " cast");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowCast ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in cast)
                {
                    if (_row.peepID == null || _row.showID == null)
                    {
                        // broken?
                        continue;
                    }
                    var entity = new ShowCast();
                    entity.ShowCastId = _row.ID;
                    entity.Person = session.Load<Person>(_row.peepID);
                    entity.Show = session.Load<Show>(_row.showID);
                    entity.Role = (_row.role ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(entity.Role))
                    {
                        continue; // skip
                    }
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.ShowCastId);
                    if (entity.ShowCastId > maxId) maxId = entity.ShowCastId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowCast OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.ShowCast', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportCrew()
        {
            var crew = oldDatabaseConnection.Query("SELECT * FROM crew").ToList();
            var jobs = oldDatabaseConnection.Query("SELECT * FROM jobs").Select(x =>
            {
                if (x.ID == null || string.IsNullOrWhiteSpace(x.job))
                {
                    return null;
                }
                int order = 999;
                if (x.priority != null)
                {
                    order = x.priority;
                }
                return new
                {
                    id = (int)x.ID,
                    name = (string)x.job,
                    order = order,
                };
            }).Where(x => x != null).ToDictionary(x => x.id);
            Log("Importing " + crew.Count + " crew");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowCrew ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in crew)
                {
                    if (_row.peepID == null || _row.showID == null)
                    {
                        // broken?
                        continue;
                    }
                    var entity = new ShowCrew();
                    entity.ShowCrewId = _row.ID;
                    entity.Person = session.Load<Person>(_row.peepID);
                    entity.Show = session.Load<Show>(_row.showID);
                    int jobId = _row.jobID;
                    entity.DisplayOrder = jobs[jobId].order;
                    entity.Position = jobs[jobId].name;
                    if (string.IsNullOrWhiteSpace(entity.Position))
                    {
                        continue; // skip
                    }
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.ShowCrewId);
                    if (entity.ShowCrewId > maxId) maxId = entity.ShowCrewId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].ShowCrew OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.ShowCrew', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }

        private static void ImportEC()
        {
            var ec = oldDatabaseConnection.Query("SELECT * FROM ec").ToList();
            var ec_list = oldDatabaseConnection.Query("SELECT * FROM ec_list").Select(x =>
            {
                if (x.ID == null)
                {
                    return null;
                }
                return new
                {
                    id = (int)x.ID,
                    name = (string)x.title ?? "",
                };
            }).Where(x => x != null).ToDictionary(x => x.id);
            Log("Importing " + ec.Count + " ec");

            using (var session = sessionFactory.OpenSession())
            {
                session.Transaction.Begin();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].PersonClubPosition ON;").ExecuteUpdate();
                var maxId = 0;
                foreach (var _row in ec)
                {
                    if (_row.peepID == null)
                    {
                        // broken?
                        continue;
                    }
                    var entity = new PersonClubPosition();
                    entity.PersonClubPositionId = _row.ID;
                    entity.Person = session.Load<Person>(_row.peepID);
                    entity.Position = ec_list[_row.ECID].name;
                    entity.Year = (short)_row.year;
                    if (string.IsNullOrWhiteSpace(entity.Position))
                    {
                        continue; // skip
                    }
                    entity.DisplayOrder = _row.ECID;
                    entity.InsertedDateTime = DateTime.MinValue;
                    entity.LastModifiedDateTime = DateTime.MinValue;
                    if (_row.last_mod != null)
                    {
                        entity.InsertedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                        entity.LastModifiedDateTime = TimeZoneInfo.ConvertTimeToUtc(_row.last_mod, TimeZoneCode.Eastern.ToTimeZoneInfo());
                    }
                    session.Save(entity, entity.PersonClubPositionId);
                    if (entity.PersonClubPositionId > maxId) maxId = entity.PersonClubPositionId;
                }
                session.Flush();
                session.CreateSQLQuery("SET IDENTITY_INSERT [dbo].PersonClubPosition OFF;").ExecuteUpdate();
                session.CreateSQLQuery("DBCC CHECKIDENT ('dbo.PersonClubPosition', RESEED, " + (maxId + 1) + ")").ExecuteUpdate();
                session.Transaction.Commit();
                session.Close();
            }
        }
    }
}
