using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriendsOfDT.Models.Directory {
    public class RegisterNewProfileParameters {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Major { get; set; }
        public string GraduationYear { get; set; }
        public string PhoneNumber { get; set; }

        public bool AreValid() {
            return true;
        }
    }
}
