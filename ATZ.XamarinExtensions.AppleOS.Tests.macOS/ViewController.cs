﻿using System;

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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tz = TimeZoneInfo.Local;
            var str = tz.DisplayName;
            var adjustmentRules = tz.GetAdjustmentRules();

            // Making sure the bug is still there.
            // This produces assertion exception - probably the values are mismatching. The nsDate seconds are correct, the transformation was the other way around.
            //var dateTime = new DateTime(2018, 4, 1, 7, 0, 0);
            //var nsDate = dateTime.ToNSDate();
            //Assert.AreEqual(544219200, nsDate.SecondsSinceReferenceDate);

            // Local TZ is Australia, Sydney, DayLightName: AEDT, Standard Name: AEST

            // This is the incorrect conversion.
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200); // 2018/03/31, 20:00:00 +0000
            var dateTime = nsDate.ToDateTime(); // UTC+10 - clock change occured during the night
            Assert.AreEqual(new DateTime(2018, 4, 1, 7, 0, 0), dateTime);

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



            // Check if DateTime should be adjusted:




            // Do any additional setup after loading the view.
            // TODO: Correct after fixing the DateTime conversion bug.
            var testFixture = new DateTimeExtensionsShould();
            testFixture.ConvertsDateCorrectly();
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
