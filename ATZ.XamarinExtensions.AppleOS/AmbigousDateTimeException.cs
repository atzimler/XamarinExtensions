using System;
using ATZ.PlatformAccess.AppleOS;

namespace ATZ.XamarinExtensions.AppleOS
{
    public class AmbigousDateTimeException : Exception
    {
        public DateTime DateTime { get; }
        public TimeZoneInfo TimeZoneInfo { get; }

        public AmbigousDateTimeException(DateTime dateTime, TimeZoneInfo timeZoneInfo)
            : base(dateTime.ExceptionMessage(timeZoneInfo, "is ambigous"))
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }
    }
}
