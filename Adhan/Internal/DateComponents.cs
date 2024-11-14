namespace Adhan.Internal
{
    public class DateComponents(int year, int month, int day)
    {
        public readonly int Year = year;
        public readonly int Month = month;
        public readonly int Day = day;

        /// <summary>
        /// Convenience method that returns a DateComponents from a given Date
        /// </summary>
        /// <param name="date">the date</param>
        /// <returns>the DateComponents (according to the default device timezone)</returns>
        public static DateComponents From(DateTime date)
        {
            return new DateComponents(date.Year, date.Month, date.Day);
        }
    }
}
