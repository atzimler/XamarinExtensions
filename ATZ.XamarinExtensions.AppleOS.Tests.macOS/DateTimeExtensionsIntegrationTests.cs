using System;
using ATZ.PlatformAccess.AppleOS;
using Foundation;
using NUnit.Framework;

namespace ATZ.XamarinExtensions.AppleOS.Tests.macOS
{
    public class TimeZoneVerificationRecord
    {
        public string DisplayName { get; set; }
        public AmbigousTimeResolution StartsInTimeResolution { get; set; }

        public DateTime DaylightTransitionTime { get; set; }
        public DateTime DaylightTransitionedTime { get; set; }
        public DateTime StandardTransitionTime { get; set; }
        public DateTime StandardTransitionedTime { get; set; }
    }

    public class DateTimeExtensionsIntegrationTests
    {
        public TimeZoneVerificationRecord Sydney = new TimeZoneVerificationRecord
        {
            DisplayName = "Australia/Sydney",
            StartsInTimeResolution = AmbigousTimeResolution.DaylightSaving,

            DaylightTransitionTime = new DateTime(2018, 10, 7, 2, 0, 0),
            DaylightTransitionedTime = new DateTime(2018, 10, 7, 3, 0, 0),
            StandardTransitionTime = new DateTime(2018, 4, 1, 3, 0, 0),
            StandardTransitionedTime = new DateTime(2018, 4, 1, 2, 0, 0)
        };

        private void VerifyTimeZone(TimeZoneVerificationRecord verificationRecord)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(verificationRecord.DisplayName);
            var resolution = verificationRecord.StartsInTimeResolution;

            var pointInTime = 536418000;
            var endInspectionPointInTime = 567954000;
            var expectedDateTime = new DateTime(2018, 1, 1, 0, 0, 0);
            while (pointInTime < endInspectionPointInTime)
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
                if (resolution == AmbigousTimeResolution.Standard 
                    && expectedDateTime == verificationRecord.DaylightTransitionTime)
                {
                    expectedDateTime = verificationRecord.DaylightTransitionedTime;
                    resolution = AmbigousTimeResolution.DaylightSaving;
                }

                if (resolution == AmbigousTimeResolution.DaylightSaving
                    && expectedDateTime == verificationRecord.StandardTransitionTime)
                {
                    expectedDateTime = verificationRecord.StandardTransitionedTime;
                    resolution = AmbigousTimeResolution.Standard;
                }
            }
        }

        [Test]
        public void FullVerificationOfTimeZonesIn2018()
        {
            VerifyTimeZone(Sydney);
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
