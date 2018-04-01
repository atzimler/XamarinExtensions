using System;
using ATZ.PlatformAccess.AppleOS;
using static System.TimeZoneInfo;

namespace ATZ.XamarinExtensions.AppleOS
{
    public static class AdjustmentRuleExtensions
    {
        public static DateTime DaylightEnd(this AdjustmentRule adjustmentRule)
        {
            return adjustmentRule.DaylightTransitionEnd.ToDateTime(adjustmentRule.DateEnd.Year).AddSeconds(-1);
        }

        public static (DateTime from, DateTime to) DaylightSaving(this AdjustmentRule adjustmentRule)
        {
            return (adjustmentRule.DaylightStart(), adjustmentRule.DaylightEnd());
        }

        public static DateTime DaylightStart(this AdjustmentRule adjustmentRule)
        {
            return adjustmentRule.DaylightTransitionStart.ToDateTime(adjustmentRule.DateStart.Year); //.AddSeconds(-1);
        }
    }
}
