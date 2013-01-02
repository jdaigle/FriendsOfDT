
namespace FODT.Models.IMDT.Indexes
{
    public class AwardProjection
    {
        public string __document_id { get; set; } // show or person id
        public string PersonId { get; set; }
        public string PersonFullName { get; set; }
        public string PersonLastName { get; set; }
        public string ShowName { get; set; }
        public Quarter? ShowQuarter { get; set; }
        public short? ShowYear { get; set; }
        public int AwardId { get; set; }
        public short AwardYear { get; set; }

        public string GetShowId()
        {
            if (__document_id.ToLower().StartsWith("shows"))
            {
                return __document_id;
            }
            return string.Empty;
        }
    }
}