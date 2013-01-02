using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FODT.Models;
using FODT.Models.IMDT;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace FODT.DataMigration
{
    public class Program
    {
        public const string DocumentStoreUrl = @"http://localhost:8080";
        public const string DataDumpDirectory = @"C:\src\FriendsOfDT\data\dump\";

        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            DocumentStoreConfiguration.BeginInit(DocumentStoreUrl);

            using (var store = DocumentStoreConfiguration.DocumentStore)
            {

                AwardsList awardsList;
                ClubPositionsList clubPositionsList;
                CrewPositionsList crewPositionsList;
                using (var session = store.OpenSession())
                {
                    var loadedAwardsList = LoadEntities("imdt_awards_list.csv");
                    awardsList = session.Load<AwardsList>("AwardsList") ?? new AwardsList();
                    awardsList.Awards = loadedAwardsList.Select(x => new FODT.Models.IMDT.KeyValuePair<int, string>() { Key = int.Parse(x.ID), Value = ((string)x.name) }).ToList();
                    session.Store(awardsList, "AwardsList");

                    var loadedECList = LoadEntities("imdt_ec_list.csv");
                    clubPositionsList = session.Load<ClubPositionsList>("ClubPositionsList") ?? new ClubPositionsList();
                    clubPositionsList.ClubPositions = loadedECList.Select(x => new FODT.Models.IMDT.KeyValuePair<int, string>() { Key = int.Parse(x.ID), Value = ((string)x.title) }).ToList();
                    session.Store(clubPositionsList, "ClubPositionsList");

                    var loadedJobsList = LoadEntities("imdt_jobs.csv");
                    crewPositionsList = session.Load<CrewPositionsList>("CrewPositionsList") ?? new CrewPositionsList();
                    crewPositionsList.CrewPositions = loadedJobsList.Select(x =>
                    {
                        var c = new CrewPositionListItem() { Key = int.Parse(x.ID), Name = (string)x.job, Priority = int.Parse(((string)x.priority).Replace("NULL", short.MaxValue.ToString())), DefinitionURL = ((string)x.URL).Replace("NULL", "") };
                        if (string.IsNullOrWhiteSpace(c.Name))
                        {
                            c.Name = "NULL";
                        }
                        return c;
                    }).ToList();
                    session.Store(crewPositionsList, "CrewPositionsList");
                    session.SaveChanges();
                }


                var loadedPeople = LoadEntities("imdt_people_fixed.csv");
                var loadedEC_byPeepID = LoadEntities("imdt_ec.csv")
                    .Where(x => x.peepID != "NULL")
                    .Select(x => new { peepID = int.Parse(x.peepID), ECID = int.Parse(x.ECID), year = short.Parse(x.year) })
                    .GroupBy(x => x.peepID).ToDictionary(x => x.Key);
                var loadedAwards_byPeepID = LoadEntities("imdt_awards.csv")
                    .Where(x => x.peepID != "NULL" && (x.showID == "NULL" || string.IsNullOrWhiteSpace(x.showID)))
                    .Select(x => new { peepID = int.Parse(x.peepID), awardID = int.Parse(x.awardID), year = short.Parse(x.year) })
                    .GroupBy(x => x.peepID).ToDictionary(x => x.Key);
                int maxPersonId = 0;
                var loadedPersonIds = new HashSet<int>();
                foreach (var item in loadedPeople)
                {
                    using (var session = store.OpenSession())
                    {
                        var person = session.Load<Person>(int.Parse((string)item.ID)) ?? new Person();
                        person.Id = int.Parse((string)item.ID);
                        person.Honorific = ((string)item.hon).Replace("NULL", "").Trim();
                        person.FirstName = ((string)item.fname).Replace("NULL", "").Trim();
                        person.LastName = ((string)item.lname).Replace("NULL", "").Trim();
                        person.MiddleName = ((string)item.mname).Replace("NULL", "").Trim();
                        person.Suffix = ((string)item.suffix).Replace("NULL", "").Trim();
                        person.SetFullName();
                        if (person.FullName.Contains("Deleted"))
                        {
                            // skip deleted people
                            continue;
                        }
                        loadedPersonIds.Add(person.Id);
                        person.AlternateName = ((string)item.nickname).Replace("NULL", "").Trim();
                        person.EmailAddress = ((string)item.email).Replace("NULL", "").Trim();
                        person.Biography = ((string)item.bio).Replace("NULL", "").Replace("\\n", Environment.NewLine).Trim();
                        var mediaId = ((string)item.media_id).Replace("NULL", "").Trim();
                        int intMediaId = 0;
                        if (int.TryParse(mediaId, out intMediaId))
                        {
                            person.PictureMediaId = "medias\\" + intMediaId;
                            if (intMediaId == 1)
                            {
                                person.PictureMediaId = string.Empty;
                            }
                        }
                        if (loadedEC_byPeepID.ContainsKey(person.Id))
                        {
                            person.ClubPositions = loadedEC_byPeepID[person.Id].Select(x => new ClubPosition()
                            {
                                ClubPositionId = x.ECID,
                                Year = x.year,
                            }).ToList();
                            if (person.ClubPositions.Any(x => !clubPositionsList.ClubPositions.Any(y => y.Key == x.ClubPositionId)))
                            {
                                System.Diagnostics.Debug.Fail("uh-oh, missing club position?");
                            }
                        }
                        if (loadedAwards_byPeepID.ContainsKey(person.Id))
                        {
                            person.Awards = loadedAwards_byPeepID[person.Id].Select(x => new PersonAward()
                            {
                                AwardId = x.awardID,
                                Year = x.year,
                            }).ToList();
                            if (person.Awards.Any(x => !awardsList.Awards.Any(y => y.Key == x.AwardId)))
                            {
                                System.Diagnostics.Debug.Fail("uh-oh, missing award?");
                            }
                        }
                        if (person.Id > maxPersonId)
                        {
                            maxPersonId = person.Id;
                        }
                        session.Store(person);
                        session.SaveChanges();
                    }
                }
                SetHiLoMax(store, maxPersonId, "Raven/Hilo/people");

                var parseIntOrNullInt = new Func<string, int?>(x =>
                {
                    int parsed = 0;
                    if (int.TryParse(x, out parsed))
                    {
                        return parsed;
                    }
                    return null;
                });

                var loadedShows = LoadEntities("imdt_shows_fixed.csv");
                var loadedCast_byShowId = LoadEntities("imdt_cast.csv")
                    .Where(x => x.peepID != "NULL" && x.showID != "NULL")
                    .Where(x => loadedPersonIds.Contains(int.Parse((string)x.peepID)))
                    .Select(x => new { peepID = int.Parse(x.peepID), showID = int.Parse(x.showID), role = x.role })
                    .GroupBy(x => x.showID).ToDictionary(x => x.Key);
                var loadedCrew_byShowId = LoadEntities("imdt_crew.csv")
                    .Where(x => x.peepID != "NULL" && x.showID != "NULL")
                    .Where(x => loadedPersonIds.Contains(int.Parse((string)x.peepID)))
                    .Select(x => new { peepID = int.Parse(x.peepID), showID = int.Parse(x.showID), jobID = int.Parse(x.jobID) })
                    .GroupBy(x => x.showID).ToDictionary(x => x.Key);
                var loadedAwards_byShowId = LoadEntities("imdt_awards.csv")
                    .Where(x => x.showID != "NULL")
                    .Where(x => !parseIntOrNullInt((string)x.peepID).HasValue || loadedPersonIds.Contains(int.Parse((string)x.peepID)))
                    .Select(x => new { showID = int.Parse(x.showID), peepID = (int?)parseIntOrNullInt(x.peepID), awardID = int.Parse(x.awardID), year = short.Parse(x.year) })
                    .GroupBy(x => x.showID).ToDictionary(x => x.Key);
                int maxShowId = 0;
                foreach (var item in loadedShows.Where(x => x.title != "NULL"))
                {
                    using (var session = store.OpenSession())
                    {
                        var show = session.Load<Show>(int.Parse((string)item.ID)) ?? new Show();
                        show.Id = int.Parse((string)item.ID);
                        show.Name = ((string)(item.title)).Replace("NULL", "").Trim().Replace("  ", " ");
                        show.Author = ((string)(item.author)).Replace("NULL", "").Trim().Replace("  ", " ");
                        show.Quarter = (Quarter)(byte)short.Parse(item.quarter);
                        show.Year = short.Parse(item.year);
                        if (loadedCast_byShowId.ContainsKey(show.Id))
                        {
                            show.Cast = loadedCast_byShowId[show.Id].Select(x => new CastMember()
                            {
                                PersonId = "people/" + x.peepID,
                                Role = x.role.Trim().Replace("  ", " "),
                            }).ToList();
                        }
                        if (loadedCrew_byShowId.ContainsKey(show.Id))
                        {
                            show.Crew = loadedCrew_byShowId[show.Id].Select(x => new CrewMember()
                            {
                                PersonId = "people/" + x.peepID,
                                CrewPositionId = x.jobID,
                            }).ToList();
                        }
                        if (loadedAwards_byShowId.ContainsKey(show.Id))
                        {
                            show.Awards = loadedAwards_byShowId[show.Id].Select(x => new ShowAward()
                            {
                                AwardId = x.awardID,
                                Year = x.year,
                                PersonId = x.peepID.HasValue ? "people/"+x.peepID.Value : null,
                            }).ToList();
                            if (show.Awards.Any(x => !awardsList.Awards.Any(y => y.Key == x.AwardId)))
                            {
                                System.Diagnostics.Debug.Fail("uh-oh, missing award?");
                            }
                        }
                        if (show.Id > maxShowId)
                        {
                            maxShowId = show.Id;
                        }
                        session.Store(show);
                        session.SaveChanges();
                    }
                }
                SetHiLoMax(store, maxShowId, "Raven/Hilo/shows");
            }
        }

        private static void SetHiLoMax(IDocumentStore store, int maxId, string doc)
        {
            store.DatabaseCommands.Patch(
                doc,
                new[]
                    {
                    new PatchRequest
                        {
                            Type = PatchCommandType.Set,
                            Name = "Max",                                     
                            Value = RavenJToken.FromObject(maxId + 1),
                        }
                });
        }

        public static List<dynamic> LoadEntities(string file)
        {
            var reader = new DynamicDelimitedReader(new TSVReader(File.OpenRead(Path.Combine(DataDumpDirectory, file)), false));
            reader.Initialize();
            return reader.Cast<dynamic>().ToList();
        }
    }
}
