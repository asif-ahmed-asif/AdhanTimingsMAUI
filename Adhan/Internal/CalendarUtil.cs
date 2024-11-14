namespace Adhan.Internal
{
    public class CalendarUtil
    {
        public static bool IsLeapYear(int year)
        {
            return year % 4 == 0 && !(year % 100 == 0 && year % 400 != 0);
        }

        /// <summary>
        /// Gets RoundedMinute for the particular date
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
        public static DateTime RoundedMinute(DateTime when)
        {
            return new DateTime((when.AddSeconds(30).Ticks / TimeSpan.TicksPerMinute) * TimeSpan.TicksPerMinute);
        }

        /// <summary>
        /// Gets a date for the particular date
        /// </summary>
        /// <param name="components">the date components</param>
        /// <returns>the date with a time set to 00:00:00 at utc</returns>
        public static DateTime ResolveTime(DateComponents components)
        {
            return new DateTime(components.Year, components.Month, components.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
