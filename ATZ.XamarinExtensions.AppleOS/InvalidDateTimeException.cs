using System;
namespace ATZ.XamarinExtensions.AppleOS
{
    public class InvalidDateTimeException : Exception
    {
        public DateTime DateTime { get; }
        public TimeZoneInfo TimeZoneInfo { get; }

        public InvalidDateTimeException(DateTime dateTime, TimeZoneInfo timeZoneInfo)
            : base($"{dateTime:yyyy-MM-dd, HH:mm:ss} is invalid in the time zone '{timeZoneInfo.DisplayName}'!")
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }
    }
}
