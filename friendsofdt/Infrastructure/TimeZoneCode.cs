using System;

namespace FriendsOfDT {
    public enum TimeZoneCode : int {
        Utc = 0,
        Eastern = 1,
        Central = 2,
        Mountain = 3,
        Pacific = 4,
        Arizona = 5,
        Alaska = 6,
        Hawaii = 7,
        Indiana = 8,
    }

    public static class TimeZoneExtensions {
        public static TimeZoneInfo ToTimeZoneInfo(this TimeZoneCode zone) {
            switch (zone) {
                case TimeZoneCode.Utc:
                    return TimeZoneInfo.Utc;
                case TimeZoneCode.Central:
                    return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                case TimeZoneCode.Mountain:
                    return TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
                case TimeZoneCode.Pacific:
                    return TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                case TimeZoneCode.Arizona:
                    return TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
                case TimeZoneCode.Alaska:
                    return TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
                case TimeZoneCode.Hawaii:
                    return TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
                case TimeZoneCode.Indiana:
                    return TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
                case TimeZoneCode.Eastern:
                default:
                    return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
        }
    }
}