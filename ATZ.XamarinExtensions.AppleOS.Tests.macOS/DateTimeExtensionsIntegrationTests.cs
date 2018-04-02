using System;
using ATZ.PlatformAccess.AppleOS;
using Foundation;
using NUnit.Framework;

namespace ATZ.XamarinExtensions.AppleOS.Tests.macOS
{
    public class DateTimeExtensionsIntegrationTests
    {
        [Test]
        public void FullVerificationOfTimeZoneIn2018()
        {
            var pointInTime = 536418000;
            var endInspectionPointInTime = 567954000;
            var timeZoneName = "AEDT";
            var expectedDateTime = new DateTime(2018, 1, 1, 0, 0, 0);
            var ambigousTimeResolution = AmbigousTimeResolution.DaylightSaving;
            while (pointInTime < endInspectionPointInTime)
            {
                var inspectedNSDate = NSDate.FromTimeIntervalSinceReferenceDate(pointInTime);
                var convertedDateTime = inspectedNSDate.ToDateTime();

                Assert.AreEqual(expectedDateTime.ToString(), convertedDateTime.ToString(), $"ToDateTime() point in time: {pointInTime}");

                var convertedNSDate = convertedDateTime.ToNSDate(ambigousTimeResolution);
                Assert.AreEqual(pointInTime, (int)convertedNSDate.SecondsSinceReferenceDate,
                                $"ToNSDate() point in time: {convertedDateTime:yyyy/MM/dd, HH:mm:ss}, UTC: {inspectedNSDate}");

                pointInTime += 300;
                expectedDateTime += new TimeSpan(0, 5, 0);
                if (timeZoneName == "AEDT" && expectedDateTime == new DateTime(2018, 4, 1, 3, 0, 0))
                {
                    expectedDateTime = new DateTime(2018, 4, 1, 2, 0, 0);
                    timeZoneName = "AEST";
                    ambigousTimeResolution = AmbigousTimeResolution.Standard;
                }
                if (timeZoneName == "AEST" && expectedDateTime == new DateTime(2018, 10, 7, 2, 0, 0))
                {
                    expectedDateTime = new DateTime(2018, 10, 7, 3, 0, 0);
                    timeZoneName = "AEDT";
                    ambigousTimeResolution = AmbigousTimeResolution.DaylightSaving;
                }
            }
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
