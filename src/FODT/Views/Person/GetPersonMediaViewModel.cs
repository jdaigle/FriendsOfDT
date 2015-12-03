using FODT.Views.Shared;

namespace FODT.Views.Person
{
    public class GetPersonMediaViewModel
    {
        public string PersonFullname { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }

        public string MediaUploadLinkURL { get; set; }
        public string PersonLinkURL { get; set; }
        public bool HasPreviousMediaLinkURL { get; set; }
        public string PreviousMediaLinkURL { get; set; }
        public bool HasNextMediaLinkURL { get; set; }
        public string NextMediaLinkURL { get; set; }
    }
}