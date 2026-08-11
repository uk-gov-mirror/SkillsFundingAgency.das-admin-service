using System;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class DateExtensions
    {
        public static DateTime? ConstructDate(string dayString, string monthString, string yearString)
        {
            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return null;

            if (!IsValidDate(year, month, day))
                return null;

            return new DateTime(year, month, day);
        }

        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }

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
                .ToString("d MMMM yyyy HH:mm", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
