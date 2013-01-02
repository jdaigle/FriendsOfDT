using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.Client;
using Raven.Client.Exceptions;

namespace FODT.Models.IMDT.Indexes
{
    public class Awards : AbstractIndexCreationTask
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition()
            {
                Maps =  { @"
from show in docs.Shows
from award in show.Awards
select new
{
	ShowName = show.Name,
	ShowQuarter = show.Quarter,
	ShowYear = show.Year,
	AwardId = award.AwardId,
	AwardYear = award.Year,
	PersonId = award.PersonId,
	PersonFullName = LoadDocument(award.PersonId).FullName,
    PersonLastName = LoadDocument(award.PersonId).LastName,
};"
, @"
from person in docs.People
from award in person.Awards
select new
{
	ShowName = (string)null,
	ShowQuarter = (string)null,
	ShowYear = (string)null,
	AwardId = award.AwardId,
	AwardYear = award.Year,
	PersonId = person.__document_id,
	PersonFullName = person.FullName,
    PersonLastName = person.LastName,
};" 
                },
                Fields = { { "PersonId" } },
                Indexes = {
                    { "PersonId", FieldIndexing.Analyzed },
                    { "ShowName", FieldIndexing.No },
                    { "ShowQuarter", FieldIndexing.No },
                    { "ShowYear", FieldIndexing.No },
                    { "AwardId", FieldIndexing.No },
                    { "AwardYear", FieldIndexing.No },
                    { "PersonFullName", FieldIndexing.No },
                    { "PersonLastName", FieldIndexing.No },
                },
                Stores = { 
                    { "ShowName", FieldStorage.Yes },
                    { "ShowQuarter", FieldStorage.Yes },
                    { "ShowYear", FieldStorage.Yes },
                    { "AwardId", FieldStorage.Yes },
                    { "AwardYear", FieldStorage.Yes },
                    { "PersonId", FieldStorage.Yes },
                    { "PersonFullName", FieldStorage.Yes },
                    { "PersonLastName", FieldStorage.Yes },
                },
            };
        }
    }
}
