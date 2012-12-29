using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace FODT.Models.IMDT.Indexes
{
    public class Shows_Crew : AbstractIndexCreationTask
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition()
            {
                Map = @"
from show in docs.Shows
from crew in show.Crew
select new
{
	ShowName = show.Name,
	ShowQuarter = show.Quarter,
	ShowYear = show.Year,
	PersonId = crew.PersonId,	
	CrewPositionId = crew.CrewPositionId,
	PersonName = LoadDocument(crew.PersonId).Name,
};",
                Fields = { { "PersonId" } },
                Indexes = {
                    { "PersonId", FieldIndexing.Analyzed },
                    { "ShowName", FieldIndexing.No },
                    { "ShowQuarter", FieldIndexing.No },
                    { "ShowYear", FieldIndexing.No },
                    { "CrewPositionId", FieldIndexing.No },
                    { "PersonName", FieldIndexing.No },
                },
                Stores = { 
                    { "ShowName", FieldStorage.Yes },
                    { "ShowQuarter", FieldStorage.Yes },
                    { "ShowYear", FieldStorage.Yes },
                    { "CrewPositionId", FieldStorage.Yes },
                    { "PersonName", FieldStorage.Yes },
                },
            };
        }
    }
}
