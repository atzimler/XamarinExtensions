using Foundation;
using System;

namespace ATZ.PlatformAccess.AppleOS
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime ReferenceDateInUtc;

        static DateTimeExtensions()
        {
            ReferenceDateInUtc = new DateTime(2001, 1, 1, 0, 0, 0);
        }

        public static NSDate FromDateTime(this NSDate nsDate, DateTime dateTime)
        {
            return dateTime.ToNSDate();
        }

        // ReSharper disable once InconsistentNaming => NSDate is the name of the type.
        public static DateTime FromNSDate(this DateTime dateTime, [JetBrains.Annotations.NotNull] NSDate nsDate)
        {
            return nsDate.ToDateTime();
        }

        [JetBrains.Annotations.NotNull]
        public static DateTime DateTime(NSDate nsDate)
        {
            return nsDate.ToDateTime();
        }

        // ReSharper disable once InconsistentNaming => NSDate is the name of the type.
        public static NSDate NSDate([JetBrains.Annotations.NotNull] DateTime dateTime)
        {
            return dateTime.ToNSDate();
        }

        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static DateTime ToDateTime([JetBrains.Annotations.NotNull] this NSDate nsDate)
        {
            var dateTimeInUtc = ReferenceDateInUtc.AddSeconds(nsDate.SecondsSinceReferenceDate);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeInUtc, TimeZoneInfo.Local);
        }

        // ReSharper disable once InconsistentNaming => NSDate is the name of the type.
        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static NSDate ToNSDate(this DateTime dateTime)
        {
            var dateTimeInUtc = TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
            var sinceReferenceDate = dateTimeInUtc - ReferenceDateInUtc;
            return Foundation.NSDate.FromTimeIntervalSinceReferenceDate(sinceReferenceDate.TotalSeconds);
        }
    }
}
