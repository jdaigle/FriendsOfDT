using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccount {

        public static WebAccount RegisterNewAccount(RegisterNewAccountParameters parameters) {
            return new WebAccount(parameters.EmailAddress) {
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                RegistrationFirstName = parameters.FirstName,
                RegistrationLastName = parameters.LastName,
                RegistrationAltFirstName = parameters.AltFirstName,
                RegistrationAltLastName = parameters.AltLastName,
                RegistrationStatus = RegistrationStatus.NotVerified,
            };
        }

        protected WebAccount() { }

        public WebAccount(string emailAddress) {
            this.Id = emailAddress;
        }

        public string Id { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

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
    }
}