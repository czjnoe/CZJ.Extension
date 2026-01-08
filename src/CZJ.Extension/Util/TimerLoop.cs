namespace CZJ.Extension
{
    /// <summary>
    /// 定时循环器
    /// </summary>
    public class TimerLoop : IDisposable
    {
        private readonly Func<CancellationToken, Task> _action;
        private readonly TimerLoopOptions _options;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _loopTask;
        private readonly SemaphoreSlim _pauseSemaphore = new(1, 1);
        private volatile TimerLoopState _state = TimerLoopState.Idle;
        private int _executionCount = 0;
        private bool _disposed = false;
        private readonly object _stateLock = new();

        public TimerLoopState State => _state;
        public int ExecutionCount => _executionCount;

        public TimerLoop(Func<CancellationToken, Task> action, TimerLoopOptions? options = null)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _options = options ?? new TimerLoopOptions();
        }

        /// <summary>
        /// 同步任务的构造函数重载
        /// </summary>
        public TimerLoop(Action action, TimerLoopOptions? options = null)
            : this(ct =>
            {
                action();
                return Task.CompletedTask;
            }, options)
        {
        }

        /// <summary>
        /// 启动定时循环
        /// </summary>
        public void Start()
        {
            lock (_stateLock)
            {
                ThrowIfDisposed();

                if (_state == TimerLoopState.Running)
                    return;

                if (_state == TimerLoopState.Paused)
                {
                    Resume();
                    return;
                }

                _state = TimerLoopState.Running;
                _cancellationTokenSource = new CancellationTokenSource();
                _executionCount = 0;

                _loopTask = Task.Run(async () => await ExecuteLoopAsync(_cancellationTokenSource.Token));
            }
        }

        /// <summary>
        /// 异步启动定时循环（等待首次执行完成）
        /// </summary>
        public async Task StartAsync(bool waitForFirstExecution = false)
        {
            Start();

            if (waitForFirstExecution)
            {
                // 等待首次执行完成
                var timeout = _options.InitialDelay + _options.Interval + TimeSpan.FromSeconds(5);
                var startTime = DateTime.Now;

                while (_executionCount == 0 && (DateTime.Now - startTime) < timeout)
                {
                    await Task.Delay(50);
                }
            }
        }

        /// <summary>
        /// 停止定时循环（同步方法）
        /// </summary>
        public void Stop(TimeSpan? timeout = null)
        {
            lock (_stateLock)
            {
                if (_state != TimerLoopState.Running && _state != TimerLoopState.Paused)
                    return;

                _state = TimerLoopState.Stopped;
                _cancellationTokenSource?.Cancel();
            }

            if (_loopTask != null)
            {
                try
                {
                    if (timeout.HasValue)
                    {
                        _loopTask.Wait(timeout.Value);
                    }
                    else
                    {
                        _loopTask.Wait();
                    }
                }
                catch (AggregateException ae)
                {
                    // 展开聚合异常
                    foreach (var ex in ae.InnerExceptions)
                    {
                        if (ex is not OperationCanceledException)
                        {
                            var errorContext = new TimerLoopExecutionContext
                            {
                                ExecutionCount = _executionCount,
                                IsSuccess = false,
                                Exception = ex
                            };
                            _options.OnError?.Invoke( errorContext);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常取消，忽略
                }
            }

            _options.OnStopped?.Invoke(ExecutionCount);
        }

        /// <summary>
        /// 停止定时循环
        /// </summary>
        public async Task StopAsync(TimeSpan? timeout = null)
        {
            lock (_stateLock)
            {
                if (_state != TimerLoopState.Running && _state != TimerLoopState.Paused)
                    return;

                _state = TimerLoopState.Stopped;
                _cancellationTokenSource?.Cancel();
            }

            if (_loopTask != null)
            {
                try
                {
                    if (timeout.HasValue)
                    {
                        await Task.WhenAny(_loopTask, Task.Delay(timeout.Value));
                    }
                    else
                    {
                        await _loopTask;
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常取消，忽略
                }
                catch (Exception ex)
                {
                    var errorContext = new TimerLoopExecutionContext
                    {
                        ExecutionCount = _executionCount,
                        IsSuccess = false,
                        Exception = ex
                    };
                    _options.OnError?.Invoke(errorContext);
                }
            }

            _options.OnStopped?.Invoke(ExecutionCount);
        }

        /// <summary>
        /// 暂停定时循环
        /// </summary>
        public void Pause()
        {
            lock (_stateLock)
            {
                ThrowIfDisposed();

                if (_state != TimerLoopState.Running)
                    return;

                _state = TimerLoopState.Paused;
            }
        }

        /// <summary>
        /// 恢复定时循环
        /// </summary>
        public void Resume()
        {
            lock (_stateLock)
            {
                ThrowIfDisposed();

                if (_state != TimerLoopState.Paused)
                    return;

                _state = TimerLoopState.Running;
            }
        }

        /// <summary>
        /// 主循环执行逻辑
        /// </summary>
        private async Task ExecuteLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                // 首次延迟
                if (_options.InitialDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_options.InitialDelay, cancellationToken);
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    // 检查暂停状态
                    while (_state == TimerLoopState.Paused && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(100, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // 检查最大执行次数
                    if (_options.MaxExecutionCount.HasValue && _executionCount >= _options.MaxExecutionCount.Value)
                    {
                        break;
                    }

                    var stopwatch = Stopwatch.StartNew();
                    var context = new TimerLoopExecutionContext
                    {
                        ExecutionCount = _executionCount + 1,
                        IsSuccess = false,
                        RetryCount = 0
                    };

                    // 执行任务（带重试机制）
                    bool success = false;
                    for (int retry = 0; retry <= _options.MaxRetryCount && !success; retry++)
                    {
                        try
                        {
                            context.RetryCount = retry;

                            if (retry > 0)
                            {
                                await Task.Delay(_options.RetryDelay, cancellationToken);
                            }

                            // 带超时的任务执行
                            if (_options.ExecutionTimeout.HasValue)
                            {
                                using var timeoutCts = new CancellationTokenSource(_options.ExecutionTimeout.Value);
                                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                                    cancellationToken, timeoutCts.Token);

                                await _action(linkedCts.Token);
                            }
                            else
                            {
                                await _action(cancellationToken);
                            }

                            success = true;
                            context.IsSuccess = true;
                        }
                        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                        {
                            throw; // 传播取消异常
                        }
                        catch (Exception ex)
                        {
                            context.Exception = ex;
                            context.IsSuccess = false;

                            _options.OnError?.Invoke( context);

                            if (retry >= _options.MaxRetryCount)
                            {
                                if (!_options.ContinueOnError)
                                {
                                    throw;
                                }
                            }
                        }
                    }

                    stopwatch.Stop();
                    context.ElapsedTime = stopwatch.Elapsed;

                    Interlocked.Increment(ref _executionCount);

                    // 执行完成回调
                    try
                    {
                        _options.OnExecuted?.Invoke(context);
                    }
                    catch (Exception ex)
                    {
                        var errorContext = new TimerLoopExecutionContext
                        {
                            ExecutionCount = _executionCount,
                            IsSuccess = false,
                            Exception = ex
                        };
                        _options.OnError?.Invoke( errorContext);
                    }

                    // 计算下次执行时间
                    if (_options.DelayAfterExecution)
                    {
                        // 任务执行完后等待
                        await Task.Delay(_options.Interval, cancellationToken);
                    }
                    else
                    {
                        // 从任务开始时计算，减去已用时间
                        var remainingTime = _options.Interval - stopwatch.Elapsed;
                        if (remainingTime > TimeSpan.Zero)
                        {
                            await Task.Delay(remainingTime, cancellationToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不需要处理
            }
            catch (Exception ex)
            {
                var errorContext = new TimerLoopExecutionContext
                {
                    ExecutionCount = _executionCount,
                    IsSuccess = false,
                    Exception = ex
                };
                _options.OnError?.Invoke( errorContext);
            }
            finally
            {
                lock (_stateLock)
                {
                    if (_state != TimerLoopState.Disposed)
                    {
                        _state = TimerLoopState.Stopped;
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                lock (_stateLock)
                {
                    _state = TimerLoopState.Disposed;
                }

                // 停止循环
                _cancellationTokenSource?.Cancel();

                // 等待任务完成（最多5秒）
                try
                {
                    _loopTask?.Wait(TimeSpan.FromSeconds(5));
                }
                catch
                {
                    // 忽略等待异常
                }

                // 释放资源
                _cancellationTokenSource?.Dispose();
                _pauseSemaphore?.Dispose();
            }

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimerLoop));
        }
    }

    /// <summary>
    /// 定时循环器管理器（管理多个循环器）
    /// </summary>
    public class TimerLoopManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, TimerLoop> _loops = new();
        private bool _disposed = false;
        private readonly object _disposeLock = new();

        /// <summary>
        /// 注册定时循环
        /// </summary>
        public void Register(string name, Func<CancellationToken, Task> action, TimerLoopOptions? options = null)
        {
            ThrowIfDisposed();

            var loop = new TimerLoop(action, options);
            if (!_loops.TryAdd(name, loop))
            {
                loop.Dispose();
                throw new InvalidOperationException($"Loop with name '{name}' already exists.");
            }
        }

        /// <summary>
        /// 启动指定循环
        /// </summary>
        public void Start(string name)
        {
            if (_loops.TryGetValue(name, out var loop))
            {
                loop.Start();
            }
        }

        /// <summary>
        /// 异步启动指定循环
        /// </summary>
        public Task StartAsync(string name, bool waitForFirstExecution = false)
        {
            if (_loops.TryGetValue(name, out var loop))
            {
                return loop.StartAsync(waitForFirstExecution);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动所有循环
        /// </summary>
        public void StartAll()
        {
            foreach (var loop in _loops.Values)
            {
                loop.Start();
            }
        }

        /// <summary>
        /// 异步启动所有循环
        /// </summary>
        public async Task StartAllAsync(bool waitForFirstExecution = false)
        {
            var tasks = new List<Task>();
            foreach (var loop in _loops.Values)
            {
                tasks.Add(loop.StartAsync(waitForFirstExecution));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 停止指定循环（同步）
        /// </summary>
        public void Stop(string name, TimeSpan? timeout = null)
        {
            if (_loops.TryGetValue(name, out var loop))
            {
                loop.Stop(timeout);
            }
        }

        /// <summary>
        /// 停止指定循环
        /// </summary>
        public async Task StopAsync(string name, TimeSpan? timeout = null)
        {
            if (_loops.TryGetValue(name, out var loop))
            {
                await loop.StopAsync(timeout);
            }
        }

        /// <summary>
        /// 停止所有循环
        /// </summary>
        public async Task StopAllAsync(TimeSpan? timeout = null)
        {
            var tasks = new List<Task>();
            foreach (var loop in _loops.Values)
            {
                tasks.Add(loop.StopAsync(timeout));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 移除指定循环
        /// </summary>
        public async Task<bool> RemoveAsync(string name)
        {
            if (_loops.TryRemove(name, out var loop))
            {
                await loop.StopAsync();
                loop.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取循环状态
        /// </summary>
        public TimerLoopState? GetState(string name)
        {
            return _loops.TryGetValue(name, out var loop) ? loop.State : null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源的实际实现
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            lock (_disposeLock)
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                    // 释放托管资源
                    foreach (var loop in _loops.Values)
                    {
                        try
                        {
                            loop.Stop(TimeSpan.FromSeconds(5));
                            loop.Dispose();
                        }
                        catch
                        {
                            // 忽略释放异常
                        }
                    }

                    _loops.Clear();
                }

                // 释放非托管资源（如果有的话）
                // ...

                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TimerLoopManager()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimerLoopManager));
        }
    }

    /// <summary>
    /// 定时循环器配置
    /// </summary>
    public class TimerLoopOptions
    {
        /// <summary>
        /// 循环间隔时间
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 首次执行延迟时间（默认立即执行）
        /// </summary>
        public TimeSpan InitialDelay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 是否在任务执行完成后才计算下次间隔（false则从开始执行时就计时）
        /// </summary>
        public bool DelayAfterExecution { get; set; } = true;

        /// <summary>
        /// 最大重试次数（任务失败时）
        /// </summary>
        public int MaxRetryCount { get; set; } = 0;

        /// <summary>
        /// 重试间隔时间
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 是否在异常时继续执行
        /// </summary>
        public bool ContinueOnError { get; set; } = true;

        /// <summary>
        /// 任务执行超时时间（null表示无限制）
        /// </summary>
        public TimeSpan? ExecutionTimeout { get; set; } = null;

        /// <summary>
        /// 最大执行次数（null表示无限制）
        /// </summary>
        public int? MaxExecutionCount { get; set; } = null;

        /// <summary>
        /// 异常处理回调
        /// </summary>
        public Action<TimerLoopExecutionContext>? OnError { get; set; }

        /// <summary>
        /// 执行完成回调
        /// </summary>
        public Action<TimerLoopExecutionContext>? OnExecuted { get; set; }

        /// <summary>
        /// 停止回调
        /// </summary>
        public Action<int>? OnStopped { get; set; }
    }

    /// <summary>
    /// 执行上下文
    /// </summary>
    public class TimerLoopExecutionContext
    {
        /// <summary>
        /// 当前执行次数
        /// </summary>
        public int ExecutionCount { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        public int RetryCount { get; set; }
    }

    /// <summary>
    /// 定时循环器状态
    /// </summary>
    public enum TimerLoopState
    {
        Idle,       // 空闲
        Running,    // 运行中
        Paused,     // 暂停
        Stopped,    // 已停止
        Disposed    // 已释放
    }
}