using System;
using NUnit.Framework;
using ATZ.PlatformAccess.AppleOS;
using Foundation;

namespace ATZ.XamarinExtensions.AppleOS.Tests
{
    public static class TestDateTimeExtensions
    {
        public static void IsUnambigous(this DateTime dateTime, string utcDateString)
        {
            Assert.AreEqual(utcDateString, dateTime.ToNSDate().ToString());
            Assert.AreEqual(utcDateString, dateTime.ToNSDate(AmbigousTimeResolution.Exception).ToString());
            Assert.AreEqual(utcDateString, dateTime.ToNSDate(AmbigousTimeResolution.Standard).ToString());
            Assert.AreEqual(utcDateString, dateTime.ToNSDate(AmbigousTimeResolution.DaylightSaving).ToString());
        }

        public static void IsAmbigous(this DateTime dateTime, string andItShouldBeConvertedTo, AmbigousTimeResolution butItIs)
        {
            var ex = Assert.Throws<AmbigousDateTimeException>(() => dateTime.ToNSDate());
            Assert.AreEqual(
                $"{dateTime:yyyy-MM-dd, HH:mm:ss} is ambigous in the time zone '{DateTimeExtensions.LocalTimeZoneInfo.DisplayName}'!",
                ex.Message);
            ex = Assert.Throws<AmbigousDateTimeException>(() => dateTime.ToNSDate(AmbigousTimeResolution.Exception));
            Assert.AreEqual(
                $"{dateTime:yyyy-MM-dd, HH:mm:ss} is ambigous in the time zone '{DateTimeExtensions.LocalTimeZoneInfo.DisplayName}'!",
                ex.Message);

            Assert.AreEqual(andItShouldBeConvertedTo, dateTime.ToNSDate(butItIs).ToString());
        }
    }

    public static class NSDateExtensions
    {
        public static void Is(this NSDate nsDate, string utcDateString, DateTime localDateTime)
        {
            Assert.AreEqual(utcDateString, nsDate.ToString());
            Assert.AreEqual(localDateTime, nsDate.ToDateTime());
        }
    }

    [TestFixture]
    public class DateTimeExtensionsShould
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
            var localDateTime = new DateTime(2018, 7, 1, 10, 0, 0);
            const string UtcDateString = "2018-07-01 00:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }

        [Test]
        public void BeforeDaylight()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc.AddHours(-1));
            var localDateTime = new DateTime(2018, 10, 7, 1, 0, 0);
            const string UtcDateString = "2018-10-06 15:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }

        [Test]
        public void StartDaylightCorrectly()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc);
            var localDateTime = new DateTime(2018, 10, 7, 3, 0, 0);
            const string UtcDateString = "2018-10-06 16:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }

        [Test]
        public void AfterDaylight()
        {
            var nsDate = GetNSDateUtc(DaylightStartsAtUtc.AddHours(1));
            var localDateTime = new DateTime(2018, 10, 7, 4, 0, 0);
            const string UtcDateString = "2018-10-06 17:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }

        [Test]
        public void Summer()
        {
            var nsDate = GetNSDateUtc(new DateTime(2018, 12, 24, 0, 0, 0));
            var localDateTime = new DateTime(2018, 12, 24, 11, 0, 0);
            const string UtcDateString = "2018-12-24 00:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }

        [Test]
        public void BeforeStandard()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc.AddHours(-1));
            var localDateTime = new DateTime(2019, 4, 7, 2, 0, 0);
            const string UtcDateString = "2019-04-06 15:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsAmbigous(butItIs: AmbigousTimeResolution.DaylightSaving, andItShouldBeConvertedTo: UtcDateString);
        }

        [Test]
        public void EndDaylightCorrectly()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc);
            var localDateTime = new DateTime(2019, 4, 7, 2, 0, 0);
            const string UtcDateString = "2019-04-06 16:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsAmbigous(butItIs: AmbigousTimeResolution.Standard, andItShouldBeConvertedTo: UtcDateString);
        }

        public void AfterStandard()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc.AddHours(1));
            var localDateTime = new DateTime(2019, 4, 7, 3, 0, 0);
            const string UtcDateString = "2019-04-06 17:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }
    }
}
