using System;
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
        public void ConvertDaylightTransitionStartCorrectly()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560534400);
            Assert.AreEqual("2018-10-06 16:00:00 +0000", nsDate.ToString());

            var convertedDateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 3, 0, 0), convertedDateTime);
        }

        [Test]
        public void ConvertDaylightTransitionStartCorrectlyEvenIfSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560534400);
            Assert.AreEqual("2018-10-06 16:00:00 +0000", nsDate.ToString());

            var convertedDateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 3, 0, 0), convertedDateTime);
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
        public void ConvertUtcTimeCrossingOnDaylightSavingEndReferenceZoneCorrectlyEvenIfSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544244400);
            Assert.AreEqual("2018-04-01 03:00:00 +0000", nsDate.ToString());

            var convertedTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 13, 0, 0), convertedTime);
        }

        [Test]
        public void ConvertsDateCorrectly()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 6, 0, 0), dateTime);
        }

        [Test]
        public void ConvertsDateCorrectlyEvenIfSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 6, 0, 0), dateTime);
        }

        [Test]
        public void AESTNewYearIsCorrect()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(536418000);
            Assert.AreEqual("2017-12-31 13:00:00 +0000", nsDate.ToString());

            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 1, 1, 0, 0, 0), dateTime);
        }

        [Test]
        public void AESTNewYearIsCorrectEvenIfTheSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(536418000);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 1, 1, 0, 0, 0), dateTime);
        }

        [Test]
        public void TransitionedBackToAESTCorrectly()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560538000);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 4, 0, 0), dateTime);
        }

        [Test]
        public void TransitionedBackToAESTCorrectlyEvenIfTheSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560538000);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 4, 0, 0), dateTime);
        }

        [Test]
        public void UtcCrossedDaylightSavingTransitionBackReferenceZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560570400);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 13, 0, 0), dateTime);
        }

        [Test]
        public void UtcCrossedDaylightSavingTransitionBackReferenceZoneEvenIfTheSystemInitializedTheTimeZone()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(560570400);
            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 10, 7, 13, 0, 0), dateTime);
        }

        [Test]
        public void EndingDaylight()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544204800);
            Assert.AreEqual("2018-03-31 16:00:00 +0000", nsDate.ToString());

            var dateTime = nsDate.ToDateTimeV2();
            Assert.AreEqual(new DateTime(2018, 4, 1, 2, 0, 0), dateTime);
        }
    }
}
