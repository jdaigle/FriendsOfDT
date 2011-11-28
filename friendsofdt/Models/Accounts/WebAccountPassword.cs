using System;
using System.Collections.Generic;
using System.Text;
using FriendsOfDT.Cryptography;

namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccountPassword {

        public WebAccountPassword() {
            PasswordHash = new byte[0];
            PasswordHistory = new List<PasswordHistory>();
        }

        public string WebSiteAccountId { get; protected set; }

        public byte[] PasswordHash { get; protected set; }
        public List<PasswordHistory> PasswordHistory { get; protected set; }

        public void ValidatePassword(string password) {
            var hashedBuffer = HashGenerator.ComputeHash(Encoding.ASCII.GetBytes(password), HashAlgorithm.SHA256);
            if (PasswordHash.Length != hashedBuffer.Length) {
                throw new InvalidPasswordException();
            }
            for (int i = 0; i < PasswordHash.Length; i++) {
                if (PasswordHash[i] != hashedBuffer[i]) {
                    throw new InvalidPasswordException();
                }
            }
        }

        public void ChangePassword(string password) {
            PasswordHash = HashGenerator.ComputeHash(Encoding.ASCII.GetBytes(password), HashAlgorithm.SHA256);
            PushPasswordHistory();
        }

        private void PushPasswordHistory() {
            PasswordHistory.Add(new PasswordHistory() { PasswordHash = PasswordHash, PasswordChangeDateTime = DateTime.UtcNow });
            while (PasswordHistory.Count > 5) {
                PasswordHistory.RemoveAt(0);
            }
        }
    }
}