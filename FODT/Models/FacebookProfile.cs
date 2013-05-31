using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models
{
    public class FacebookProfile
    {
        public string id;
        public string name;
        public string first_name;
        public string middle_name;
        public string last_name;
        public string link;
        public string username;
        public string gender;
        public string locale;
        public string email;
        public FacebookProfile_age_range age_range;
        public FacebookProfile_picture picture;

        public class FacebookProfile_age_range
        {
            public int min;
            public int max;
        }

        public class FacebookProfile_picture
        {
            public FacebookProfile_picture_data data;
        }

        public class FacebookProfile_picture_data
        {
            public string url;
            public bool is_silhouette; 
        }
    }
}