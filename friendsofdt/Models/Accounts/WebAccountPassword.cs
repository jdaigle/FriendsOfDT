using System;
using System.Collections.Generic;
using System.Text;
using FriendsOfDT.Cryptography;

namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccountPassword {

        public WebAccountPassword(string webAccountId) {
            this.Id = "webAccounts/passwords/" + Guid.NewGuid();
            this.WebAccountId = webAccountId;
            PasswordHash = new byte[0];
            PasswordHistory = new List<PasswordHistory>();
        }

        public string Id { get; protected set; }
        public string WebAccountId { get; protected set; }
        public byte[] PasswordHash { get; protected set; }
        public List<PasswordHistory> PasswordHistory { get; protected set; }

        public bool PasswordMatches(string password) {
            var hashedBuffer = HashGenerator.ComputeHash(Encoding.ASCII.GetBytes(password), HashAlgorithm.SHA256);
            if (PasswordHash.Length != hashedBuffer.Length) {
                return false;
            }
            for (int i = 0; i < PasswordHash.Length; i++) {
                if (PasswordHash[i] != hashedBuffer[i]) {
                    return false;
                }
            }
            return true;
        }

        public void ValidatePassword(string password) {
            if (!PasswordMatches(password)) {
                throw new InvalidPasswordException();
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