using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace FODT.Models.IMDT.Indexes
{
    public class Shows_Cast : AbstractIndexCreationTask
    {
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition()
            {
                Map = @"
from show in docs.Shows
from cast in show.Cast
select new
{
	ShowName = show.Name,
	ShowQuarter = show.Quarter,
	ShowYear = show.Year,
	PersonId = cast.PersonId,	
	Role = cast.Role,
	PersonName = LoadDocument(cast.PersonId).Name,
};",
                Fields = { { "PersonId" } },
                Indexes = {
                    { "PersonId", FieldIndexing.Analyzed },
                    { "ShowName", FieldIndexing.No },
                    { "ShowQuarter", FieldIndexing.No },
                    { "ShowYear", FieldIndexing.No },
                    { "Role", FieldIndexing.No },
                    { "PersonName", FieldIndexing.No },
                },
                Stores = { 
                    { "ShowName", FieldStorage.Yes },
                    { "ShowQuarter", FieldStorage.Yes },
                    { "ShowYear", FieldStorage.Yes },
                    { "Role", FieldStorage.Yes },
                    { "PersonName", FieldStorage.Yes },
                },
            };
        }
    }
}
