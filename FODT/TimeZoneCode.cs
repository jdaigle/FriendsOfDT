using System;
using System.ComponentModel;

namespace FODT
{
    public enum TimeZoneCode : int
    {
        [Description("UTC")]
        Utc = 0,
        [Description("Eastern")]
        Eastern = 1,
        [Description("Central")]
        Central = 2,
        [Description("Mountain")]
        Mountain = 3,
        [Description("Pacific")]
        Pacific = 4,
        [Description("US Mountain")]
        Arizona = 5,
        [Description("Hawaii-Aleutian")]
        Alaska = 6,
        [Description("Hawaii-Aleutian")]
        Hawaii = 7,
        [Description("US Eastern")]
        Indiana = 8,
    }

    public static class TimeZoneExtensions
    {
        public static TimeZoneInfo ToTimeZoneInfo(this TimeZoneCode zone)
        {
            switch (zone)
            {
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
