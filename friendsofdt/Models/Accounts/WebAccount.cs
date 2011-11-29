using System;
using System.Collections.Generic;

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
        }

        public string Id { get; protected set; }
        public string EmailAddress { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        public List<WebAccountRole> Roles { get; protected set; }

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

        public bool CanLogin() {
            return RegistrationStatus == Accounts.RegistrationStatus.Verified;
        }
    }
}