namespace CZJ.Extension
{
    public class RetryHelper
    {
        private int _delayMilliseconds;
        private int _maxAttempts;

        public static RetryHelper New => new();

        /// <summary>
        /// 重试参数
        /// </summary>
        /// <param name="maxAttempts"></param>
        /// <returns></returns>
        public RetryHelper MaxAttempts(int maxAttempts)
        {
            _maxAttempts = maxAttempts;

            return this;
        }

        public RetryHelper DelayMilliseconds(int delayMilliseconds)
        {
            _delayMilliseconds = delayMilliseconds;

            return this;
        }

        public void Execute(Action action)
        {
            while (true)
            {
                try
                {
                    action();
                    break;
                }
                catch
                {
                    if (_maxAttempts-- <= 0)
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
            while (true)
            {
                try
                {
                    return func();
                }
                catch
                {
                    if (_maxAttempts-- <= 0)
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
    }
}
