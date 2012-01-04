using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriendsOfDT.Models.Accounts {
    public class RegisterNewAccountParameters {
        public string EmailAddress { get; set; }
        public string RequestedPassword { get; set; }
    }
}
