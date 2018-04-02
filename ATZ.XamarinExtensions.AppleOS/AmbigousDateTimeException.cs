using System;
namespace ATZ.XamarinExtensions.AppleOS
{
    public class AmbigousDateTimeException : Exception
    {
        public DateTime DateTime { get; }
        public TimeZoneInfo TimeZoneInfo { get; }

        public AmbigousDateTimeException(DateTime dateTime, TimeZoneInfo timeZoneInfo)
            : base($"{dateTime:yyyy-MM-dd, HH:mm:ss} is ambigous in the time zone '{timeZoneInfo.DisplayName}'!")
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }
    }
}
