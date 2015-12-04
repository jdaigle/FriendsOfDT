using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class EditPersonViewModel
    {
        public static EditPersonViewModel Empty()
        {
            return new EditPersonViewModel() { Photos = new List<Photo>() };
        }

        public string POSTUrl { get; set; }

        public int? PersonId { get; set; }

        public string Honorific { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Nickname { get; set; }
        public string Biography { get; set; }

        public int? DefaultPhotoId { get; set; }

        public List<Photo> Photos { get; set; }

        public class Photo
        {
            public int PhotoItemId { get; set; }
            public string PhotoTinyURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}