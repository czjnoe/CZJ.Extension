using CZJ.Extension.Helper;

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

        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        /// <param name="time">时间</param>
        public static long GetUnixTimestamp(this DateTime time)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long ticks = (time - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return ConvertHelper.ToLong(ticks / TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// 从Unix时间戳获取时间
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        public static DateTime GetTimeFromUnixTimestamp(this long timestamp)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            TimeSpan span = new TimeSpan(long.Parse(timestamp + "0000000"));
            return start.Add(span).Add(new TimeSpan(8, 0, 0));
        }

        /// <summary>
        /// 获取本地日期
        /// </summary>
        /// <param name="date">日期</param>
        public static DateTime GetLocalDateTime(this DateTime date)
        {
            if (date == DateTime.MinValue)
                return DateTime.MinValue;
            switch (date.Kind)
            {
                case DateTimeKind.Utc:
                    return date.ToLocalTime();
                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(date, DateTimeKind.Local);
                default:
                    return date;
            }
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime dateTime)
        {
            dateTime = GetLocalDateTime(dateTime);
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime dateTime)
        {
            dateTime = GetLocalDateTime(dateTime);
            return dateTime.ToString("HH:mm:ss");
        }


        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime dateTime)
        {
            dateTime = GetLocalDateTime(dateTime);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 获取中文格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            dateTime = GetLocalDateTime(dateTime);
            return $"{dateTime.Year}年{dateTime.Month}月{dateTime.Day}日";
        }
    }
}
