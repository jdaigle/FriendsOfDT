using System;

namespace FriendsOfDT.Models.Directory {
    [EntityMetadata(Version = 1)]
    public class DirectoryProfile {
        public static DirectoryProfile RegisterNewProfile(RegisterNewProfileParameters parameters) {
            return new DirectoryProfile(parameters.EmailAddress, parameters.FirstName, parameters.LastName) {
                PhoneNumber = parameters.PhoneNumber,
                GraduationYear = parameters.GraduationYear,
                Major = parameters.Major,
            };
        }

        public DirectoryProfile(string emailAddress, string firstName, string lastName) {
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string Id { get; protected set; }
        public string EmailAddress { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        public string PhoneNumber { get; protected set; }
        public string GraduationYear { get; protected set; }
        public string Major { get; protected set; }
    }
}