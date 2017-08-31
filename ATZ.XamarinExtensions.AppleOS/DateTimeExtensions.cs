using Foundation;
using JetBrains.Annotations;
using System;

namespace ATZ.PlatformAccess.iOS
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
        public static DateTime FromNSDate(this DateTime dateTime, [NotNull] NSDate nsDate)
        {
            return nsDate.ToDateTime();
        }

        [NotNull]
        public static DateTime DateTime(NSDate nsDate)
        {
            return nsDate.ToDateTime();
        }

        // ReSharper disable once InconsistentNaming => NSDate is the name of the type.
        public static NSDate NSDate([NotNull] DateTime dateTime)
        {
            return dateTime.ToNSDate();
        }

        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static DateTime ToDateTime([NotNull] this NSDate nsDate)
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
