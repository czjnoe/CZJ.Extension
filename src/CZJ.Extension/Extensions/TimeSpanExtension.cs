namespace CZJ.Extension
{
    /// <summary>
    /// TimeSpan 扩展
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 将 TimeSpan 格式化为友好的字符串
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="includeMilliseconds">是否包含毫秒</param>
        /// <returns>格式化后的字符串</returns>
        public static string ToFriendlyString(this TimeSpan timeSpan, bool includeMilliseconds = true)
        {
            var parts = new System.Collections.Generic.List<string>();

            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days}天");

            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours}小时");

            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes}分钟");

            if (timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds}秒");

            if (includeMilliseconds && timeSpan.Milliseconds > 0)
                parts.Add($"{timeSpan.Milliseconds}毫秒");

            return parts.Count > 0 ? string.Join(" ", parts) : "0毫秒";
        }

        /// <summary>
        /// 将 TimeSpan 格式化为简短的字符串（英文）
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="includeMilliseconds">是否包含毫秒</param>
        /// <returns>格式化后的字符串</returns>
        public static string ToShortString(this TimeSpan timeSpan, bool includeMilliseconds = true)
        {
            var parts = new System.Collections.Generic.List<string>();

            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days}d");

            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours}h");

            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes}m");

            if (timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds}s");

            if (includeMilliseconds && timeSpan.Milliseconds > 0)
                parts.Add($"{timeSpan.Milliseconds}ms");

            return parts.Count > 0 ? string.Join(" ", parts) : "0ms";
        }

        /// <summary>
        /// 将 TimeSpan 格式化为紧凑格式（HH:MM:SS.mmm）
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="includeMilliseconds">是否包含毫秒</param>
        /// <returns>格式化后的字符串</returns>
        public static string ToCompactString(this TimeSpan timeSpan, bool includeMilliseconds = false)
        {
            if (includeMilliseconds)
                return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
            else
                return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        /// <summary>
        /// 将 TimeSpan 转换为总毫秒数（长整型）
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>总毫秒数</returns>
        public static long ToMilliseconds(this TimeSpan timeSpan)
        {
            return (long)timeSpan.TotalMilliseconds;
        }

        /// <summary>
        /// 将 TimeSpan 转换为总秒数（长整型）
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>总秒数</returns>
        public static long ToSeconds(this TimeSpan timeSpan)
        {
            return (long)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 判断时间间隔是否超过指定的阈值
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="threshold">阈值</param>
        /// <returns>如果超过阈值返回 true，否则返回 false</returns>
        public static bool IsGreaterThan(this TimeSpan timeSpan, TimeSpan threshold)
        {
            return timeSpan > threshold;
        }

        /// <summary>
        /// 判断时间间隔是否小于指定的阈值
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="threshold">阈值</param>
        /// <returns>如果小于阈值返回 true，否则返回 false</returns>
        public static bool IsLessThan(this TimeSpan timeSpan, TimeSpan threshold)
        {
            return timeSpan < threshold;
        }

        /// <summary>
        /// 判断时间间隔是否在指定范围内
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果在范围内返回 true，否则返回 false</returns>
        public static bool IsBetween(this TimeSpan timeSpan, TimeSpan min, TimeSpan max)
        {
            return timeSpan >= min && timeSpan <= max;
        }

        /// <summary>
        /// 将时间间隔四舍五入到最近的秒
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>四舍五入后的时间间隔</returns>
        public static TimeSpan RoundToSeconds(this TimeSpan timeSpan)
        {
            return TimeSpan.FromSeconds(Math.Round(timeSpan.TotalSeconds));
        }

        /// <summary>
        /// 将时间间隔四舍五入到最近的分钟
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>四舍五入后的时间间隔</returns>
        public static TimeSpan RoundToMinutes(this TimeSpan timeSpan)
        {
            return TimeSpan.FromMinutes(Math.Round(timeSpan.TotalMinutes));
        }

        /// <summary>
        /// 判断时间间隔是否为零或接近零
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="tolerance">容差（默认1毫秒）</param>
        /// <returns>如果接近零返回 true，否则返回 false</returns>
        public static bool IsNearZero(this TimeSpan timeSpan, TimeSpan? tolerance = null)
        {
            var threshold = tolerance ?? TimeSpan.FromMilliseconds(1);
            return Math.Abs(timeSpan.Ticks) <= threshold.Ticks;
        }
    }
}
