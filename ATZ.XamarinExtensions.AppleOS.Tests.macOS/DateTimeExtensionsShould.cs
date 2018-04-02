using System;
using NUnit.Framework;
using ATZ.PlatformAccess.AppleOS;
using Foundation;
using System.Linq;

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

        [Test]
        public void AfterStandard()
        {
            var nsDate = GetNSDateUtc(DaylightEndsAtUtc.AddHours(1));
            var localDateTime = new DateTime(2019, 4, 7, 3, 0, 0);
            const string UtcDateString = "2019-04-06 17:00:00 +0000";

            nsDate.Is(UtcDateString, localDateTime);
            localDateTime.IsUnambigous(UtcDateString);
        }


        [Test]
        public void VerifyIf_CAL262_IsFixed()
        {
            // This is now a correct conversion.
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200); // 2018/03/31, 20:00:00 +0000
            var dateTime = nsDate.ToDateTime(); // UTC+10 - clock change occured during the night
            Assert.AreEqual("2018-03-31 20:00:00 +0000", nsDate.ToString());
            Assert.AreEqual(new DateTime(2018, 4, 1, 6, 0, 0), dateTime);

            // Just to make sure everything really works, the other side of the problematic event (the starting date) was this.
            var nsDate2 = NSDate.FromTimeIntervalSinceReferenceDate(544186800); // 2018/03/31, 11:00:00 +0000
            var dateTime2 = nsDate2.ToDateTime(); // UTC+11 - This day is already in UTC+10, but clock change has not occured yet.
            Assert.AreEqual(new DateTime(2018, 3, 31, 22, 0, 0), dateTime2);

            // These are working correctly even when the bug is present:
            var nsDate3 = NSDate.FromTimeIntervalSinceReferenceDate(543607200); // 2018/03/24, 18:00:00 +0000
            var dateTime3 = nsDate3.ToDateTime(); // UTC+11
            Assert.AreEqual(new DateTime(2018, 3, 25, 5, 0, 0), dateTime3);

            var nsDate4 = NSDate.FromTimeIntervalSinceReferenceDate(544561200); // 2018/04/04, 19:00:00 +0000
            var dateTime4 = nsDate4.ToDateTime(); // UTC+10
            Assert.AreEqual(new DateTime(2018, 4, 5, 5, 0, 0), dateTime4);
        }

        [Test]
        public void VerifyInvalidDateTimeException()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            var localDateTime = new DateTime(2018, 10, 7, 2, 30, 0);
            var ex = Assert.Throws<InvalidDateTimeException>(() => localDateTime.ToNSDate());
            Assert.AreEqual("2018-10-07, 02:30:00 is invalid in the time zone 'Australia/Sydney'!", ex.Message);
        }

        [Test]
        public void LocalGivesDebuggableInformation()
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.Local;
            Assert.IsTrue(
                TimeZoneInfo.Local.SupportsDaylightSavingTime,
                "Sorry, this has to be tested in a location where there is support for daylight saving in the local time zone.");
            var adjustmentRule = TimeZoneInfo.Local.GetAdjustmentRules().FirstOrDefault(r => r.DateStart.Between(new DateTime(2018, 1, 1), new DateTime(2019, 1, 1)));
            Assert.IsNotNull(
                adjustmentRule,
                "Sorry, this has to be tested in a location where we actually can find adjustment rules.");
            var daylightEnd = adjustmentRule.DaylightEnd();
            var halfDelta = new TimeSpan(0, (int)adjustmentRule.DaylightDelta.TotalMinutes / 2, 0);
            var ambigousTime = daylightEnd - halfDelta;

            var ex = Assert.Throws<AmbigousDateTimeException>(() => ambigousTime.ToNSDate());
            Assert.AreEqual($"2019-04-07, 02:29:59 is ambigous in the time zone 'Local'! The local time zone: StandardName: {TimeZoneInfo.Local.StandardName}, DaylightName: {TimeZoneInfo.Local.DaylightName}, BaseUtcOffset: {TimeZoneInfo.Local.BaseUtcOffset}.", ex.Message);
        }
    }
}
