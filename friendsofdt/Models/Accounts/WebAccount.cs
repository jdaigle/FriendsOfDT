using System;
using System.Collections.Generic;
using System.Text;
using FriendsOfDT.Cryptography;

namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccount {

        public static string GetId(Guid id) {
            return "webAccounts/" + id;
        }

        public static WebAccount RegisterNewAccount(RegisterNewAccountParameters parameters) {
            return new WebAccount() {
                EmailAddress = parameters.EmailAddress,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                RegistrationFirstName = parameters.FirstName,
                RegistrationLastName = parameters.LastName,
                RegistrationAltFirstName = parameters.AltFirstName,
                RegistrationAltLastName = parameters.AltLastName,
                RegistrationStatus = RegistrationStatus.NotVerified,
            };
        }

        public WebAccount() {
            this.Id = "webAccounts/" + Guid.NewGuid();
            Roles = new List<WebAccountRole>();
            PasswordHash = new byte[0];
            PasswordHistory = new List<PasswordHistory>();
        }

        public string GetGuidPartOfId() {
            return Id.Replace("webAccounts/", "");
        }

        public string Id { get; protected set; }
        public string EmailAddress { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        public List<WebAccountRole> Roles { get; protected set; }
        public byte[] PasswordHash { get; protected set; }
        public List<PasswordHistory> PasswordHistory { get; protected set; }
        public int LoginCount { get; protected set; }
        public DateTime? LastLoginDateTime { get; protected set; }

        public RegistrationStatus RegistrationStatus { get; protected set; }
        public string RegistrationFirstName { get; protected set; }
        public string RegistrationAltFirstName { get; protected set; }
        public string RegistrationLastName { get; protected set; }
        public string RegistrationAltLastName { get; protected set; }

        public void Verify() {
            if (RegistrationStatus == RegistrationStatus.NotVerified) {
                RegistrationStatus = RegistrationStatus.Verified;
                // TODO: Publish event for e-mail notification
            }
        }

        public void Enable() {
            if (RegistrationStatus == RegistrationStatus.Disabled) {
                RegistrationStatus = RegistrationStatus.Verified;
            }
        }

        public void Disable() {
            if (RegistrationStatus == RegistrationStatus.Verified) {
                RegistrationStatus = RegistrationStatus.Disabled;
            }
        }

        public bool CanLogin() {
            return RegistrationStatus == Accounts.RegistrationStatus.Verified;
        }

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

        public void IncrementLogin() {
            LoginCount++;
            LastLoginDateTime = DateTime.UtcNow;
        }
    }
}