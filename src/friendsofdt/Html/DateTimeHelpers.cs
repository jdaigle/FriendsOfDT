using System;
using System.Web.Mvc;

namespace FriendsOfDT.Html {
    public static class DateTimeHelpers {
        public static MvcHtmlString DisplayDateTime(this HtmlHelper html, DateTime dateTime) {
            return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToString("hh:mm tt"));
        }

        public static MvcHtmlString DisplayDateTime(this HtmlHelper html, DateTime? dateTime, TimeZoneInfo timeZoneInfo, string defaultValueIfNull) {
            if (!dateTime.HasValue) return new MvcHtmlString(defaultValueIfNull);
            return DisplayDateTime(html, dateTime.Value, timeZoneInfo);
        }

        public static MvcHtmlString DisplayDateTime(this HtmlHelper html, DateTime dateTime, TimeZoneInfo timeZoneInfo) {
            return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToString("hh:mm:ss tt") + " (" + TimeZoneAbbreviation(timeZoneInfo, dateTime) + ")");
        }

        public static MvcHtmlString ConvertAndDisplayDateTime(this HtmlHelper html, DateTime? dateTime, TimeZoneInfo targetTimeZoneInfo, string defaultValueIfNull) {
            if (!dateTime.HasValue) return new MvcHtmlString(defaultValueIfNull);
            return ConvertAndDisplayDateTime(html, dateTime.Value, targetTimeZoneInfo);
        }

        public static MvcHtmlString ConvertAndDisplayDateTime(this HtmlHelper html, DateTime dateTime, TimeZoneInfo targetTimeZoneInfo) {
            return ConvertAndDisplayDateTime(html, dateTime, targetTimeZoneInfo, true);
        }

        public static MvcHtmlString ConvertAndDisplayDateTime(this HtmlHelper html, DateTime dateTime, TimeZoneInfo targetTimeZoneInfo, bool displayTimeZone) {
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), targetTimeZoneInfo);
            if (displayTimeZone)
                return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToString("hh:mm:ss tt") + " (" + TimeZoneAbbreviation(targetTimeZoneInfo, dateTime) + ")");
            else
                return new MvcHtmlString(dateTime.ToShortDateString() + " " + dateTime.ToString("hh:mm:ss tt"));
        }

        private static string TimeZoneAbbreviation(TimeZoneInfo targetTimeZoneInfo, DateTime dateTime) {
            var fullName = TimeZone.CurrentTimeZone.IsDaylightSavingTime(dateTime)
                ? TimeZone.CurrentTimeZone.DaylightName
                : TimeZone.CurrentTimeZone.StandardName;

            var abbr = "";
            foreach (var s in fullName.Split(new char[] { ' ' }))
                if (s.Length >= 1)
                    abbr += s.Substring(0, 1);
            return abbr;
        }
    }
}