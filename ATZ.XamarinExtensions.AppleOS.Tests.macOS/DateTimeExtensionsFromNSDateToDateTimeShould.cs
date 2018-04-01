﻿using System;
using NUnit.Framework;
using ATZ.XamarinExtensions.AppleOS;
using ATZ.PlatformAccess.AppleOS;
using Foundation;

namespace ATZ.XamarinExtensions.AppleOS.Tests
{
    [TestFixture]
    public class DateTimeExtensionsFromNSDateToDateTimeShould
    {
        [Test]
        public void ConvertDaylightTransitionStartCorrectlyFromNSDateToDateTime()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560534400);
            Assert.AreEqual("2018-10-06 16:00:00 +0000", nsDate.ToString());

            var convertedDateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 2, 0, 0), convertedDateTime);
        }

        [Test]
        public void ConvertUtcTimeCrossingOnDaylightSavingEndReferenceZoneCorrectly()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544244400);
            Assert.AreEqual("2018-04-01 03:00:00 +0000", nsDate.ToString());

            var convertedTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 13, 0, 0), convertedTime);
        }

        [Test]
        public void ConvertsDateCorrectly()
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 6, 0, 0), dateTime);
        }
    }
}