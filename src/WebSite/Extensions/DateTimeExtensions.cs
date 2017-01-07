using System;
using System.Web;

namespace WebSite.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? ToUserLocalFromUtc(this DateTime? dateTime)
        {
            return dateTime?.AddMinutes(-GetTimeZoneOffset());
        }

        public static DateTime ToUserLocalFromUtc(this DateTime dateTime)
        {
            return dateTime.AddMinutes(-GetTimeZoneOffset());
        }

        public static DateTime? ToUtcFromUserLocal(this DateTime? dateTime)
        {
            return dateTime?.AddMinutes(GetTimeZoneOffset());
        }

        public static DateTime ToUtcFromUserLocal(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return dateTime;
            return dateTime.AddMinutes(GetTimeZoneOffset());
        }

        public static int GetTimeZoneOffset()
        {
            var request = HttpContext.Current.Request;
            int timezoneMinutesOffset;
            if (request.Cookies["user_timezone_minutes_offset"] == null)
            {
                timezoneMinutesOffset = -(int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
            }
            else
            {
                timezoneMinutesOffset = int.Parse(request.Cookies["user_timezone_minutes_offset"].Value);
            }

            return timezoneMinutesOffset;
        }
    }
}
