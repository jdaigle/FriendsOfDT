using FODT.Views.Photos;

namespace FODT.Views.Person
{
    public class GetPersonPhotoViewModel
    {
        public string PersonFullname { get; set; }

        public PhotoViewModel PhotoViewModel { get; set; }

        public string PhotoUploadLinkURL { get; set; }
        public string PersonLinkURL { get; set; }
        public bool HasPreviousPhotoLinkURL { get; set; }
        public string PreviousPhotoLinkURL { get; set; }
        public bool HasNextPhotoLinkURL { get; set; }
        public string NextPhotoLinkURL { get; set; }
    }
}