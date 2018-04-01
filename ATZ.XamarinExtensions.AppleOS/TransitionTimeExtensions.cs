using System;
using static System.TimeZoneInfo;

namespace ATZ.XamarinExtensions.AppleOS
{
    public static class TransitionTimeExtensions
    {
        public static DateTime ToDateTime(this TransitionTime transitionTime, int year)
        {
            return new DateTime(
                year, transitionTime.Month, transitionTime.Day,
                transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second);                
        }
    }
}
