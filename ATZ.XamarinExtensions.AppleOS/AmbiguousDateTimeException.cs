using System;
using ATZ.PlatformAccess.AppleOS;

namespace ATZ.XamarinExtensions.AppleOS
{
    public class AmbiguousDateTimeException : Exception
    {
        public DateTime DateTime { get; }
        public TimeZoneInfo TimeZoneInfo { get; }

        public AmbiguousDateTimeException(DateTime dateTime, TimeZoneInfo timeZoneInfo)
            : base(dateTime.ExceptionMessage(timeZoneInfo, "is ambiguous"))
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }
    }
}
