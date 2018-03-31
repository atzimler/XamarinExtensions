using System;
using NUnit.Framework;
using ATZ.XamarinExtensions.AppleOS;
using ATZ.PlatformAccess.AppleOS;
using Foundation;

namespace ATZ.XamarinExtensions.AppleOS.Tests
{
    [TestFixture]
    public class DateTimeExtensionsShould
    {
        [Test]
        public void ConvertsDateCorrectly()
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate(544219200);
            var dateTime = nsDate.ToDateTime();
            Assert.AreEqual(new DateTime(2018, 4, 1, 6, 0, 0), dateTime);
        }
    }
}
