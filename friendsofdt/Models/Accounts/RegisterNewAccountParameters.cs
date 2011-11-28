using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriendsOfDT.Models.Accounts {
    public class RegisterNewAccountParameters {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string AltFirstName { get; set; }
        public string AltLastName { get; set; }
    }
}
