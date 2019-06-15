using System;
using AppKit;
using Foundation;

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

            // Integration.
            var integrationTestFixture = new DateTimeExtensionsIntegrationTests();
            integrationTestFixture.MakeSureThereIsNoInternalNSDateBugWithSecondsSinceReference();
            integrationTestFixture.FullVerificationOfTimeZones();

            // Do any additional setup after loading the view.
            // TODO: Correct after fixing the DateTime conversion bug.
            var testFixture = new DateTimeExtensionsShould();
            testFixture.OneTimeSetUp();
            testFixture.Winter();
            testFixture.BeforeDaylight();
            testFixture.StartDaylightCorrectly();
            testFixture.AfterDaylight();
            testFixture.Summer();
            testFixture.BeforeStandard();
            testFixture.EndDaylightCorrectly();
            testFixture.AfterStandard();
            testFixture.VerifyIf_CAL262_IsFixed();
            testFixture.VerifyInvalidDateTimeException();
            testFixture.LocalGivesDebuggableInformation();

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
