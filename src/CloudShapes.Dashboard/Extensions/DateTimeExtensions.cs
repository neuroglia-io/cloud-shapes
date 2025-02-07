namespace CloudShapes.Dashboard;

/// <summary>
/// Provides extension methods for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{

    /// <summary>
    /// Formats the provided <see cref="DateTimeOffset"/> in a relative fashion (e.g.: 3 minutes ago, Yesterday at 1:00pm...)
    /// </summary>
    /// <param name="dateTime">The extended <see cref="DateTimeOffset"/></param>
    /// <returns>The <see cref="DateTimeOffset"/>, formatted in a relative fashion</returns>
    public static string AsMoment(this DateTimeOffset dateTime)
    {
        var localDateTime = dateTime.ToLocalTime();
        var now = DateTimeOffset.Now;
        var delta = now.Subtract(localDateTime);
        if (Math.Abs(delta.Days) >= 1)
        {
            var cultureFormats = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            var defaults = new CalendarTimeFormats(CultureInfo.CurrentUICulture);
            var formats = new CalendarTimeFormats(defaults.SameDay, defaults.NextDay, defaults.NextWeek, defaults.LastDay, defaults.LastWeek, $"{cultureFormats.ShortDatePattern} {cultureFormats.ShortTimePattern}");
            return now.DateTime.CalendarTime(localDateTime.DateTime, formats);
        }
        else if (delta < TimeSpan.Zero) return localDateTime.DateTime.ToNow();
        else return localDateTime.DateTime.FromNow();
    }
}
