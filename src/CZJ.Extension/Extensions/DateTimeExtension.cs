namespace CZJ.Extension
{
    public static class DateTimeExtension
    {
        public static string GetUtcString(this DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Utc
                ? dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK")
                : dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        public static DateTime ConvertFromUtcString(this string utcString)
        {
            var dateTime = DateTime.Parse(utcString);
            return dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
        }

        /// <summary>
        ///     获取当前日期当前周的周一日期
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DateTime GetCurrentWeekMonday(this DateTime current)
        {
            if (current.DayOfWeek == DayOfWeek.Sunday)
                return current.AddDays(-6);
            return current.AddDays(-((int)current.DayOfWeek - 1));
        }

        /// <summary>
        ///     获取当前日期当前周的周日日期
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DateTime GetCurrentWeekSunday(this DateTime current)
        {
            if (current.DayOfWeek == DayOfWeek.Sunday)
                return current;
            return current.AddDays(7 - (int)current.DayOfWeek);
        }

        /// <summary>
        ///     获取当前日期当前月的第一天
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DateTime GetCurrentMonthFirstDay(this DateTime current)
        {
            return DateTime.Parse(current.ToString("yyyy-MM-01"));
        }

        /// <summary>
        ///     获取当前日期当前月的最后一天
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DateTime GetCurrentMonthLastDay(this DateTime current)
        {
            return DateTime.Parse(current.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        ///     移除日期的时区,时区变为Unspecified
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime RemoveKind(this DateTime time)
        {
            return DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
        }

        public static (DateTime? beginTime, DateTime? endTime) ConvertToDateTimeZoneByString(string startTime, string endTime)
        {
            if (startTime.IsValidationDateTime() && !endTime.IsValidationDateTime())
            {
                return (DateTime.Parse(startTime), null);
            }

            if (!startTime.IsValidationDateTime() && endTime.IsValidationDateTime())
            {
                return (null, DateTime.Parse(endTime));
            }

            if (startTime.IsValidationDateTime() && endTime.IsValidationDateTime())
            {
                return (DateTime.Parse(startTime), DateTime.Parse(endTime).AddDays(1));
            }

            return (null, null);
        }

        public static (DateTime? beginTime, DateTime? endTime) ConvertToDateTimeZoneByDate(DateTime? startTime, DateTime? endTime)
        {
            if (startTime.IsValidationDateTime() && !endTime.IsValidationDateTime())
            {
                return (startTime, null);
            }

            if (!startTime.IsValidationDateTime() && endTime.IsValidationDateTime())
            {
                return (null, endTime);
            }

            if (startTime.IsValidationDateTime() && endTime.IsValidationDateTime())
            {
                return (startTime, endTime.Value.AddDays(1));
            }

            return (null, null);
        }

        public static bool IsValidationDateTime(this DateTime? dateTime)
        {
            return dateTime.HasValue && (dateTime.Value != DateTime.MinValue || dateTime.Value != DateTime.MaxValue);
        }

        public static bool IsValidationDateTime(this DateTime dateTime)
        {
            return dateTime != DateTime.MinValue || dateTime != DateTime.MaxValue;
        }

        public static bool IsValidationDateTime(this string dateTime)
        {
            return dateTime.IsNotNullOrWhiteSpace() && !dateTime.Contains(DateTime.MaxValue.ToString("yyyy")) && !dateTime.Contains(DateTime.MinValue.ToString("yyyy"));
        }

        public static string ToStandardDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToStandardDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
