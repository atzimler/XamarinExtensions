using ATZ.XamarinExtensions.AppleOS;
using Foundation;
using System;
using System.Linq;
using static System.TimeZoneInfo;

namespace ATZ.PlatformAccess.AppleOS
{
    public static class DateTimeExtensions
    {
        // TODO: Promote this to the API of the functions.
        public static TimeZoneInfo LocalTimeZoneInfo { get; set; } = Local;
        private static readonly DateTime ReferenceDateInUtc;

        static DateTimeExtensions()
        {
            ReferenceDateInUtc = new DateTime(2001, 1, 1, 0, 0, 0);
        }

        #region Interval Operators
        public static bool Between(this DateTime dateTime, DateTime from, DateTime to)
        {
            return from <= dateTime && dateTime <= to;
        }

        public static bool Outside(this DateTime dateTime, ValueTuple<DateTime, DateTime> interval)
        {
            return Outside(dateTime, interval.Item1, interval.Item2);
        }

        public static bool Outside(this DateTime dateTime, DateTime from, DateTime to)
        {
            return !Between(dateTime, from, to);
        }
        #endregion

        #region Conversions between NSDate and DateTime
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

        // Fun fact: It turns out that this specific function is also implemented in Xamarin.Forms.Platform.iOS - however, it is currently missing from the MacOS platform.
        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static DateTime ToDateTime([JetBrains.Annotations.NotNull] this NSDate nsDate)
        {
            var dateTimeInUtc = ReferenceDateInUtc.AddSeconds(nsDate.SecondsSinceReferenceDate);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeInUtc, TimeZoneInfo.Local);
        }

        // Fun fact: It turns out that this specific function is also implemented in Xamarin.Forms.Platform.iOS - however, it is currently missing from the MacOS platform.
        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static DateTime ToDateTimeV2([JetBrains.Annotations.NotNull] this NSDate nsDate)
        {
            var dateTimeInUtc = ReferenceDateInUtc.AddSeconds(nsDate.SecondsSinceReferenceDate);
            var convertedDateTime = ConvertTimeFromUtc(dateTimeInUtc, LocalTimeZoneInfo);
            if (LocalTimeZoneInfo.SupportsDaylightSavingTime)
            {
                var adjustmentRule = LocalTimeZoneInfo.GetAdjustmentRules().FirstOrDefault(r => convertedDateTime.Between(r.DateStart, r.DateEnd));
                if (adjustmentRule != null)
                {
                    var invalid = LocalTimeZoneInfo.IsInvalidTime(convertedDateTime);

                    // correct solution for testcase 1 & 2
                    if (invalid || !dateTimeInUtc.IsDaylightSavingTime() && convertedDateTime.IsDaylightSavingTime())
                    {
                        convertedDateTime += adjustmentRule.DaylightDelta;
                    }
                    if (!invalid && dateTimeInUtc.IsDaylightSavingTime() && !convertedDateTime.IsDaylightSavingTime())
                    {
                        convertedDateTime -= adjustmentRule.DaylightDelta;
                    }
//                    if (misaligned && outsideDaylightSaving)
////                    if (misaligned)
                    //{
                    //    convertedDateTime -= adjustmentRule.DaylightDelta;
                    //}
                }
            }

            return convertedDateTime;
        }

        // Fun fact: It turns out that this specific function is also implemented in Xamarin.Forms.Platform.iOS - however, it is currently missing from the MacOS platform.
        // ReSharper disable once InconsistentNaming => NSDate is the name of the type.
        // ReSharper disable once MemberCanBePrivate.Global => Part of API
        public static NSDate ToNSDate(this DateTime dateTime)
        {
            var dateTimeInUtc = TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
            var sinceReferenceDate = dateTimeInUtc - ReferenceDateInUtc;
            return Foundation.NSDate.FromTimeIntervalSinceReferenceDate(sinceReferenceDate.TotalSeconds);
        }
        #endregion
    }
}
