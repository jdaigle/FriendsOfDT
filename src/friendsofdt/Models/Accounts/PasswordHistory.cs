using System;

namespace FriendsOfDT.Models.Accounts {
    public class PasswordHistory {
        public byte[] PasswordHash { get; set; }
        public DateTime PasswordChangeDateTime { get; set; }
    }
}
