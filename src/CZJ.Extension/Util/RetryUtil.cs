namespace CZJ.Extension
{
    public class RetryUtil
    {
        private int _delayMilliseconds;
        private int _maxAttempts;

        public static RetryUtil New => new();

        /// <summary>
        /// 重试次数
        /// </summary>
        /// <param name="maxAttempts"></param>
        /// <returns></returns>
        public RetryUtil MaxAttempts(int maxAttempts)
        {
            _maxAttempts = maxAttempts;
            return this;
        }

        /// <summary>
        /// 重试延时时间
        /// </summary>
        /// <param name="delayMilliseconds"></param>
        /// <returns></returns>
        public RetryUtil DelayMilliseconds(int delayMilliseconds)
        {
            _delayMilliseconds = delayMilliseconds;
            return this;
        }

        public void Execute(Action action)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    action();
                    break;
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        Thread.Sleep(_delayMilliseconds);
                    }
                }
            }
        }

        /// <summary>
        /// 带执行次数
        /// </summary>
        /// <param name="action"></param>
        public void Execute(Action<int> action)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    action(currentAttempt);
                    break;
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        Thread.Sleep(_delayMilliseconds);
                    }
                }
            }
        }

        public T Execute<T>(Func<T> func)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    return func();
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        Thread.Sleep(_delayMilliseconds);
                    }
                }
            }
        }

        /// <summary>
        /// 带执行次数回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T Execute<T>(Func<int, T> func)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    return func(currentAttempt);
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        Thread.Sleep(_delayMilliseconds);
                    }
                }
            }
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    await action();
                    break;
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        await Task.Delay(_delayMilliseconds);
                    }
                }
            }
        }

        /// <summary>
        /// 带执行次数回调
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Func<int, Task> action)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    await action(currentAttempt);
                    break;
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        await Task.Delay(_delayMilliseconds);
                    }
                }
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    return await func();
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        await Task.Delay(_delayMilliseconds);
                    }
                }
            }
        }

        /// <summary>
        /// 带执行次数回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<int, Task<T>> func)
        {
            int currentAttempt = 0;
            int remainingAttempts = _maxAttempts;

            while (true)
            {
                currentAttempt++;
                try
                {
                    return await func(currentAttempt);
                }
                catch
                {
                    if (remainingAttempts-- <= 0)
                    {
                        throw;
                    }

                    if (_delayMilliseconds > 0)
                    {
                        await Task.Delay(_delayMilliseconds);
                    }
                }
            }
        }
    }
}
