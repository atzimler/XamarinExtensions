using System;
using ATZ.PlatformAccess.AppleOS;
using Foundation;
using NUnit.Framework;

namespace ATZ.XamarinExtensions.AppleOS.Tests.macOS
{
    public class TimeZoneVerificationRecord
    {
        public string DisplayName { get; set; }
        public AmbiguousTimeResolution StartsInTimeResolution { get; set; }
        public int VerifyFrom { get; set; }
        public int VerifyTo { get; set; }
        public DateTime DateTimeStartsFrom { get; set; } = new DateTime(2018, 1, 1, 0, 0, 0);

        public DateTime DaylightTransitionTime { get; set; } = DateTime.MinValue;
        public DateTime DaylightTransitionedTime { get; set; } = DateTime.MinValue;
        public DateTime StandardTransitionTime { get; set; } = DateTime.MinValue;
        public DateTime StandardTransitionedTime { get; set; } = DateTime.MinValue;
    }

    public class DateTimeExtensionsIntegrationTests
    {
        public TimeZoneVerificationRecord Abidjan = new TimeZoneVerificationRecord
        {
            DisplayName = "Africa/Abidjan",
            StartsInTimeResolution = AmbiguousTimeResolution.Standard,
            VerifyFrom = 536457600, VerifyTo = 567993600
        };

        public TimeZoneVerificationRecord Accra1941 = new TimeZoneVerificationRecord
        {
            DisplayName = "Africa/Accra",
            StartsInTimeResolution = AmbiguousTimeResolution.Standard,
            VerifyFrom = -1893456000, VerifyTo = -1861920000,
            DateTimeStartsFrom = new DateTime(1941, 1, 1, 0, 0, 0),

            DaylightTransitionTime = new DateTime(1941, 9, 1, 0, 0, 0),
            DaylightTransitionedTime = new DateTime(1941, 9, 1, 0, 20, 0),
            StandardTransitionTime = new DateTime(1941, 12, 31, 0, 0, 0),
            StandardTransitionedTime = new DateTime(1941, 12, 30, 23, 40, 0)
        };

        public TimeZoneVerificationRecord Accra2018 = new TimeZoneVerificationRecord
        {
            DisplayName = "Africa/Accra",
            StartsInTimeResolution = AmbiguousTimeResolution.Standard,
            VerifyFrom = 536457600, VerifyTo = 567993600
        };

        public TimeZoneVerificationRecord AddisAbaba = new TimeZoneVerificationRecord
        {
            DisplayName = "Africa/Addis_Ababa",
            StartsInTimeResolution = AmbiguousTimeResolution.Standard,
            VerifyFrom = 536446800, VerifyTo = 567982800
        };

        public TimeZoneVerificationRecord NewYork = new TimeZoneVerificationRecord
        {
            DisplayName = "America/New_York",
            StartsInTimeResolution = AmbiguousTimeResolution.Standard,
            VerifyFrom = 536475600, VerifyTo = 568011600,

            DaylightTransitionTime = new DateTime(2018, 3, 11, 2, 0, 0),
            DaylightTransitionedTime = new DateTime(2018, 3, 11, 3, 0, 0),
            StandardTransitionTime = new DateTime(2018, 11, 4, 2, 0, 0),
            StandardTransitionedTime = new DateTime(2018, 11, 4, 1, 0, 0)
        };

        public TimeZoneVerificationRecord Sydney = new TimeZoneVerificationRecord
        {
            DisplayName = "Australia/Sydney",
            StartsInTimeResolution = AmbiguousTimeResolution.DaylightSaving,
            VerifyFrom = 536418000, VerifyTo = 567954000,

            DaylightTransitionTime = new DateTime(2018, 10, 7, 2, 0, 0),
            DaylightTransitionedTime = new DateTime(2018, 10, 7, 3, 0, 0),
            StandardTransitionTime = new DateTime(2018, 4, 1, 3, 0, 0),
            StandardTransitionedTime = new DateTime(2018, 4, 1, 2, 0, 0)
        };

        public void VerifyTimeZone(TimeZoneVerificationRecord verificationRecord)
        {
            DateTimeExtensions.LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(verificationRecord.DisplayName);
            var resolution = verificationRecord.StartsInTimeResolution;

            var pointInTime = verificationRecord.VerifyFrom;
            var expectedDateTime = verificationRecord.DateTimeStartsFrom;
            while (pointInTime < verificationRecord.VerifyTo)
            {
                var inspectedNSDate = NSDate.FromTimeIntervalSinceReferenceDate(pointInTime);
                var convertedDateTime = inspectedNSDate.ToDateTime();

                Assert.AreEqual(
                    expectedDateTime.ToString(), convertedDateTime.ToString(),
                    $"ToDateTime() point in time: {pointInTime}, in time zone: {verificationRecord.DisplayName}");

                var convertedNSDate = convertedDateTime.ToNSDate(resolution);
                Assert.AreEqual(
                    pointInTime, (int)convertedNSDate.SecondsSinceReferenceDate,
                    $"ToNSDate() point in time: {convertedDateTime:yyyy/MM/dd, HH:mm:ss}, UTC: {inspectedNSDate}, in time zone: {verificationRecord.DisplayName}");

                pointInTime += 300;
                expectedDateTime += new TimeSpan(0, 5, 0);
                if (resolution == AmbiguousTimeResolution.Standard 
                    && expectedDateTime == verificationRecord.DaylightTransitionTime)
                {
                    expectedDateTime = verificationRecord.DaylightTransitionedTime;
                    resolution = AmbiguousTimeResolution.DaylightSaving;
                }

                if (resolution == AmbiguousTimeResolution.DaylightSaving
                    && expectedDateTime == verificationRecord.StandardTransitionTime)
                {
                    expectedDateTime = verificationRecord.StandardTransitionedTime;
                    resolution = AmbiguousTimeResolution.Standard;
                }
            }
        }

        [Test]
        public void FullVerificationOfTimeZones()
        {
            VerifyTimeZone(Sydney);
            VerifyTimeZone(NewYork);
            VerifyTimeZone(Abidjan);
            VerifyTimeZone(AddisAbaba);
            VerifyTimeZone(Accra1941);
            VerifyTimeZone(Accra2018);
        }

        [Test]
        public void MakeSureThereIsNoInternalNSDateBugWithSecondsSinceReference()
        {
            var expected = 536418000;
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(expected);
            Assert.AreEqual("2017-12-31 13:00:00 +0000", nsDate.ToString());

            var toSeconds = 567954000;
            var toNSDate = NSDate.FromTimeIntervalSinceReferenceDate(toSeconds);
            Assert.AreEqual("2018-12-31 13:00:00 +0000", toNSDate.ToString());

            while (expected < toSeconds)
            {
                Assert.AreEqual(expected, (int)nsDate.SecondsSinceReferenceDate);

                expected += 300;
                nsDate = NSDate.FromTimeIntervalSinceReferenceDate(expected);
            }
        }
    }
}
