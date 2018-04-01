﻿using ATZ.XamarinExtensions.AppleOS;
using Foundation;
using JetBrains.Annotations;
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
        public static bool Between(this DateTime dateTime, ValueTuple<DateTime, DateTime> interval)
        {
            return Between(dateTime, interval.Item1, interval.Item2);
        }

        public static bool Between(this DateTime dateTime, DateTime from, DateTime to)
        {
            return from <= dateTime && dateTime <= to;
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
        public static DateTime ToDateTime([NotNull] this NSDate nsDate)
        {
            var dateTimeInUtc = ReferenceDateInUtc.AddSeconds(nsDate.SecondsSinceReferenceDate);
            var convertedDateTime = dateTimeInUtc + LocalTimeZoneInfo.BaseUtcOffset;
            if (LocalTimeZoneInfo.SupportsDaylightSavingTime)
            {
                var adjustmentRule = LocalTimeZoneInfo.GetAdjustmentRules().FirstOrDefault(r => convertedDateTime.Between(r.DateStart, r.DateEnd));
                if (adjustmentRule != null)
                {
                    var daylightSaving = convertedDateTime.Between(adjustmentRule.DaylightSaving());
                    var utcDaylightSaving = convertedDateTime.Add(adjustmentRule.DaylightDelta).Between(adjustmentRule.DaylightSaving());
                    if (daylightSaving && utcDaylightSaving)
                    {
                        return convertedDateTime + adjustmentRule.DaylightDelta;
                    }
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
