using FODT.Views.Photos;

namespace FODT.Views.Show
{
    public class GetShowPhotoViewModel
    {
        public string UploadLinkURL { get; set; }
        public string ShowLinkURL { get; set; }
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public int PhotoId { get; set; }

        public string PreviousPhotoLinkURL { get; set; }
        public string NextPhotoLinkURL { get; set; }

        public PhotoViewModel PhotoViewModel { get; set; }
    }
}