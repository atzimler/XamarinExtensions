using System;
using NUnit.Framework;
using ATZ.PlatformAccess.AppleOS;
using Foundation;

namespace ATZ.XamarinExtensions.AppleOS.Tests
{
    public static class NSDateExtensions
    {
        public static void Is(this NSDate nsDate, string utcDateString, DateTime localDateTime)
        {
            Assert.AreEqual(utcDateString, nsDate.ToString());
            Assert.AreEqual(localDateTime, nsDate.ToDateTime());
        }
    }

    [TestFixture]
    public class DateTimeExtensionsFromNSDateToDateTimeShould
    {
        private readonly DateTime ReferenceDateTimeUtc = new DateTime(2001, 1, 1, 0, 0, 0);
        private readonly DateTime DaylightStartsAtUtc = new DateTime(2018, 10, 6, 16, 0, 0);
        private readonly DateTime DaylightEndsAtUtc = new DateTime(2019, 4, 6, 16, 0, 0);

        private NSDate GetNSDateUtc(DateTime utcDateTime)
        {
            return NSDate.FromTimeIntervalSinceReferenceDate((utcDateTime - ReferenceDateTimeUtc).TotalSeconds);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
        }

        [Test]
        public void Winter()
        {
            var nsDate = GetNSDateUtc(new DateTime(2018, 7, 1, 0, 0, 0));
            nsDate.Is("2018-07-01 00:00:00 +0000", new DateTime(2018, 7, 1, 10, 0, 0));
        }

        [Test]
        public void BeforeDaylight()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc.AddHours(-1));
            nsDate.Is("2018-10-06 15:00:00 +0000", new DateTime(2018, 10, 7, 1, 0, 0));
        }

        [Test]
        public void StartDaylightCorrectly()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc);
            nsDate.Is("2018-10-06 16:00:00 +0000", new DateTime(2018, 10, 7, 3, 0, 0));
        }

        [Test]
        public void AfterDaylight()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc.AddHours(1));
            nsDate.Is("2018-10-06 17:00:00 +0000", new DateTime(2018, 10, 7, 4, 0, 0));
        }

        [Test]
        public void Summer()
        {
            var nsDate = GetNSDateUtc(new DateTime(2018, 12, 24, 0, 0, 0));
            nsDate.Is("2018-12-24 00:00:00 +0000", new DateTime(2018, 12, 24, 11, 0, 0));
        }

        [Test]
        public void BeforeStandard()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc.AddHours(-1));
            nsDate.Is("2019-04-06 15:00:00 +0000", new DateTime(2019, 4, 7, 2, 0, 0));
        }

        [Test]
        public void EndDaylightCorrectly()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc);
            nsDate.Is("2019-04-06 16:00:00 +0000", new DateTime(2019, 4, 7, 2, 0, 0));
        }

        public void AfterStandard()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc.AddHours(1));
            nsDate.Is("2019-04-06 17:00:00 +0000", new DateTime(2019, 4, 7, 3, 0, 0));
        }
    }
}
