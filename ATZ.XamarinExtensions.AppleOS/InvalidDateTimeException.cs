using System;
using ATZ.PlatformAccess.AppleOS;

namespace ATZ.XamarinExtensions.AppleOS
{
    public class InvalidDateTimeException : Exception
    {
        public DateTime DateTime { get; }
        public TimeZoneInfo TimeZoneInfo { get; }

        public InvalidDateTimeException(DateTime dateTime, TimeZoneInfo timeZoneInfo)
            : base(dateTime.ExceptionMessage(timeZoneInfo, "is invalid"))
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }
    }
}
