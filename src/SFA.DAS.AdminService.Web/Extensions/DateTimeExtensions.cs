using System;
using System.Globalization;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo UkTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public static DateTime UtcToUkLocalTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, UkTimeZone);
        }

        public static string ToUkDateTimeString(this DateTime date)
        {
            return date
                .UtcToUkLocalTime()
                .ToString("d MMMM yyyy HH:mm", CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
