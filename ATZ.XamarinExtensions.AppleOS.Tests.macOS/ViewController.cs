using System;
using System.Linq;
using AppKit;
using ATZ.PlatformAccess.AppleOS;
using Foundation;
using NUnit.Framework;

namespace ATZ.XamarinExtensions.AppleOS.Tests.macOS
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        private double CalculatePointInTime(DateTime dateTime, TimeSpan timeZoneDifference)
        {
            var referenceUtcDate = new DateTime(2001, 1, 1, 0, 0, 0);
            var utcDateTime = dateTime - timeZoneDifference;
            var referenceTimeSpan = utcDateTime - referenceUtcDate;
            return referenceTimeSpan.TotalSeconds;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            // Calculation.
            var from = CalculatePointInTime(new DateTime(1941, 1, 1, 0, 0, 0), new TimeSpan(0, 0, 0));
            var to = CalculatePointInTime(new DateTime(1942, 1, 1, 0, 0, 0), new TimeSpan(0, 0, 0));

            // Integration.
            var integrationTestFixture = new DateTimeExtensionsIntegrationTests();
            //integrationTestFixture.MakeSureThereIsNoInternalNSDateBugWithSecondsSinceReference();
            //integrationTestFixture.FullVerificationOfTimeZonesIn2018();
            var tz = timeZones.FirstOrDefault(t => t.GetAdjustmentRules().Count() > 0 && t.GetAdjustmentRules().Count(r => DateTime.Now.Between(r.DateStart, r.DateEnd)) == 0);

            // Do any additional setup after loading the view.
            // TODO: Correct after fixing the DateTime conversion bug.
            var testFixture = new DateTimeExtensionsShould();
            testFixture.OneTimeSetUp();
            //testFixture.Winter();
            //testFixture.BeforeDaylight();
            //testFixture.StartDaylightCorrectly();
            //testFixture.AfterDaylight();
            //testFixture.Summer();
            //testFixture.BeforeStandard();
            //testFixture.EndDaylightCorrectly();
            //testFixture.AfterStandard();
            //testFixture.VerifyIf_CAL262_IsFixed();

            // TODO: testFixture.ThrowExceptionOnInvalidDateTime();
            // TODO: Test if gives useful time zone information when ambigous time and it is throwing exception, but was initialized with TimeZoneInfo.Local.

            // TODO: Transform this into a console, so that it can run on the build server
            // (the code uses Foundation calls, and a result cannot be built as portable .NET code, which prevents NUnit at the moment to run it up from the test runner).
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
