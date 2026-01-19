namespace CZJ.Extension
{
    /// <summary>
    /// 计时器工具类，提供各种计时和时间间隔计算的方法
    /// </summary>
    public static class StopWatchUtil
    {
        /// <summary>
        /// 记录程序启动时间
        /// </summary>
        private static readonly DateTime _startTime = DateTime.Now;

        /// <summary>
        /// 获取当前时间戳，即 Unix 时间戳，精确到毫秒
        /// </summary>
        /// <returns>当前时间戳</returns>
        public static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 获取程序启动时间
        /// </summary>
        /// <returns>程序启动时间</returns>
        public static DateTime GetStartTime()
        {
            return _startTime;
        }

        /// <summary>
        /// 获取当前时间距离程序启动时间的时间间隔
        /// </summary>
        /// <returns>当前时间距离程序启动时间的时间间隔</returns>
        public static TimeSpan GetElapsedTime()
        {
            return DateTime.Now - _startTime;
        }

        /// <summary>
        /// 创建一个新的 Stopwatch 并启动计时
        /// </summary>
        /// <returns>一个新的 Stopwatch</returns>
        public static Stopwatch StartNew()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        /// <summary>
        /// 计算指定操作的执行时间
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>操作执行的时间</returns>
        public static TimeSpan Measure(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// 计算指定操作的执行时间，并输出执行结果到指定文件
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="fileName">输出结果的文件名</param>
        public static void MeasureAndSave(Action action, string fileName)
        {
            TimeSpan elapsedTime = Measure(action);
            System.IO.File.WriteAllText(fileName, elapsedTime.TotalMilliseconds.ToString());
        }

        /// <summary>
        /// 计算指定操作的执行时间，并返回操作的结果
        /// </summary>
        /// <typeparam name="T">操作返回值的类型</typeparam>
        /// <param name="func">要执行的操作</param>
        /// <param name="elapsed">输出参数，操作执行的时间</param>
        /// <returns>操作的返回值</returns>
        public static T Measure<T>(Func<T> func, out TimeSpan elapsed)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            T result = func.Invoke();
            stopwatch.Stop();
            elapsed = stopwatch.Elapsed;
            return result;
        }

        /// <summary>
        /// 计算指定操作的执行时间，并返回包含结果和耗时的元组
        /// </summary>
        /// <typeparam name="T">操作返回值的类型</typeparam>
        /// <param name="func">要执行的操作</param>
        /// <returns>包含结果和耗时的元组</returns>
        public static (T Result, TimeSpan Elapsed) MeasureWithResult<T>(Func<T> func)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            T result = func.Invoke();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }

        /// <summary>
        /// 异步计算指定操作的执行时间
        /// </summary>
        /// <param name="asyncAction">要执行的异步操作</param>
        /// <returns>操作执行的时间</returns>
        public static async Task<TimeSpan> MeasureAsync(Func<Task> asyncAction)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await asyncAction.Invoke();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// 异步计算指定操作的执行时间，并返回操作的结果
        /// </summary>
        /// <typeparam name="T">操作返回值的类型</typeparam>
        /// <param name="asyncFunc">要执行的异步操作</param>
        /// <returns>包含结果和耗时的元组</returns>
        public static async Task<(T Result, TimeSpan Elapsed)> MeasureAsync<T>(Func<Task<T>> asyncFunc)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            T result = await asyncFunc.Invoke();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }

        /// <summary>
        /// 等待指定的时间
        /// </summary>
        /// <param name="milliseconds">要等待的毫秒数</param>
        public static void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// 计算两个时间的时间间隔
        /// </summary>
        /// <param name="time1">第一个时间</param>
        /// <param name="time2">第二个时间</param>
        /// <returns>两个时间的时间间隔</returns>
        public static TimeSpan GetTimeSpan(DateTime time1, DateTime time2)
        {
            return time1 - time2;
        }

        /// <summary>
        /// 计算两个时间戳的时间间隔
        /// </summary>
        /// <param name="timestamp1">第一个时间戳</param>
        /// <param name="timestamp2">第二个时间戳</param>
        /// <returns>两个时间戳的时间间隔</returns>
        public static TimeSpan GetTimeSpan(long timestamp1, long timestamp2)
        {
            DateTime time1 = DateTimeOffset.FromUnixTimeMilliseconds(timestamp1).LocalDateTime;
            DateTime time2 = DateTimeOffset.FromUnixTimeMilliseconds(timestamp2).LocalDateTime;
            return GetTimeSpan(time1, time2);
        }

        /// <summary>
        /// 将时间间隔格式化为友好的字符串，例如 1h 20m 30s
        /// </summary>
        /// <param name="timeSpan">要格式化的时间间隔</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            int hours = timeSpan.Days * 24 + timeSpan.Hours;
            string formattedTimeSpan = $"{hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            return formattedTimeSpan;
        }

        #region 多计时管理

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Stopwatch> _timers
            = new System.Collections.Concurrent.ConcurrentDictionary<string, Stopwatch>();

        /// <summary>
        /// 开始一个命名的计时器，可以在其他方法中停止
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <param name="restart">如果计时器已存在，是否重新开始</param>
        /// <returns>如果成功启动返回 true，否则返回 false</returns>
        public static bool Start(string timerName, bool restart = false)
        {
            if (string.IsNullOrWhiteSpace(timerName))
                throw new ArgumentException("计时器名称不能为空", nameof(timerName));

            if (_timers.TryGetValue(timerName, out var existingTimer))
            {
                if (restart)
                {
                    existingTimer.Restart();
                    return true;
                }
                return false; // 计时器已存在且不重启
            }

            var stopwatch = Stopwatch.StartNew();
            return _timers.TryAdd(timerName, stopwatch);
        }

        /// <summary>
        /// 停止指定的计时器并返回耗时
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <param name="remove">是否从管理器中移除该计时器</param>
        /// <returns>计时器的耗时，如果计时器不存在则返回 null</returns>
        public static TimeSpan? Stop(string timerName, bool remove = true)
        {
            if (string.IsNullOrWhiteSpace(timerName))
                throw new ArgumentException("计时器名称不能为空", nameof(timerName));

            if (_timers.TryGetValue(timerName, out var stopwatch))
            {
                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;

                if (remove)
                {
                    _timers.TryRemove(timerName, out _);
                }

                return elapsed;
            }

            return null;
        }

        /// <summary>
        /// 暂停指定的计时器（不移除）
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>如果成功暂停返回 true，否则返回 false</returns>
        public static bool Pause(string timerName)
        {
            if (_timers.TryGetValue(timerName, out var stopwatch))
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 恢复已暂停的计时器
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>如果成功恢复返回 true，否则返回 false</returns>
        public static bool Resume(string timerName)
        {
            if (_timers.TryGetValue(timerName, out var stopwatch))
            {
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定计时器的当前耗时（不停止计时器）
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>计时器的当前耗时，如果计时器不存在则返回 null</returns>
        public static TimeSpan? GetElapsed(string timerName)
        {
            if (_timers.TryGetValue(timerName, out var stopwatch))
            {
                return stopwatch.Elapsed;
            }
            return null;
        }

        /// <summary>
        /// 检查指定的计时器是否存在
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>如果存在返回 true，否则返回 false</returns>
        public static bool Exists(string timerName)
        {
            return _timers.ContainsKey(timerName);
        }

        /// <summary>
        /// 检查指定的计时器是否正在运行
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>如果正在运行返回 true，否则返回 false</returns>
        public static bool IsRunning(string timerName)
        {
            if (_timers.TryGetValue(timerName, out var stopwatch))
            {
                return stopwatch.IsRunning;
            }
            return false;
        }

        /// <summary>
        /// 移除指定的计时器
        /// </summary>
        /// <param name="timerName">计时器名称</param>
        /// <returns>如果成功移除返回 true，否则返回 false</returns>
        public static bool Remove(string timerName)
        {
            return _timers.TryRemove(timerName, out _);
        }

        /// <summary>
        /// 获取所有活动的计时器名称
        /// </summary>
        /// <returns>所有计时器名称的集合</returns>
        public static System.Collections.Generic.IEnumerable<string> GetActiveTimers()
        {
            return _timers.Keys;
        }

        /// <summary>
        /// 清除所有计时器
        /// </summary>
        public static void ClearAll()
        {
            foreach (var stopwatch in _timers.Values)
            {
                stopwatch.Stop();
            }
            _timers.Clear();
        }

        /// <summary>
        /// 停止所有计时器并返回它们的耗时信息
        /// </summary>
        /// <returns>包含所有计时器名称和耗时的字典</returns>
        public static System.Collections.Generic.Dictionary<string, TimeSpan> StopAll()
        {
            var results = new System.Collections.Generic.Dictionary<string, TimeSpan>();
            foreach (var kvp in _timers)
            {
                kvp.Value.Stop();
                results[kvp.Key] = kvp.Value.Elapsed;
            }
            _timers.Clear();
            return results;
        }

        #endregion
    }
}
