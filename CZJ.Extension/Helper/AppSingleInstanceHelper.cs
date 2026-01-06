namespace CZJ.Extension
{
    /// <summary>
    /// 应用程序单实例运行
    /// 当你已经打开了一个软件，再次双击图标时，它不会弹出一个新窗口，而是把新启动时的参数发给“老窗口”，并让“老窗口”跳到最前面
    /// </summary>
    public sealed class AppSingleInstanceHelper : IDisposable
    {
        private readonly string _appId;
        private readonly Mutex _mutex;
        private readonly bool _isFirstInstance;
        private CancellationTokenSource? _cts;

        public bool IsFirstInstance => _isFirstInstance;

        /// <summary>
        /// 当接收到第二实例消息
        /// </summary>
        public event Action<string[]>? OnArgumentsReceived;

        /// <summary>
        /// 当需要激活窗口
        /// </summary>
        public event Action? OnActivate;

        public AppSingleInstanceHelper(string appId)
        {
            _appId = appId;
            _mutex = new Mutex(false, $"Global\\{appId}");

            try
            {
                _isFirstInstance = _mutex.WaitOne(0, false);
            }
            catch (AbandonedMutexException)
            {
                _isFirstInstance = true;
            }

            if (_isFirstInstance)
            {
                StartPipeServer();
            }
        }

        /// <summary>
        /// 非首实例时调用，向主实例发送参数
        /// </summary>
        public async Task SendArgumentsToFirstInstanceAsync(string[] args)
        {
            using var client = new NamedPipeClientStream(
                ".",
                _appId,
                PipeDirection.Out);

            await client.ConnectAsync(1000);

            var payload = string.Join('\n', args);
            var bytes = Encoding.UTF8.GetBytes(payload);

            await client.WriteAsync(bytes, 0, bytes.Length);
        }

        private void StartPipeServer()
        {
            _cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        using var server = new NamedPipeServerStream(
                            _appId,
                            PipeDirection.In,
                            1,
                            PipeTransmissionMode.Byte,
                            PipeOptions.Asynchronous);

                        await server.WaitForConnectionAsync(_cts.Token);

                        using var ms = new MemoryStream();
                        await server.CopyToAsync(ms, _cts.Token);

                        var msg = Encoding.UTF8.GetString(ms.ToArray());
                        var args = msg.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                        OnArgumentsReceived?.Invoke(args);
                        OnActivate?.Invoke();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        // 忽略单次异常，保证服务不中断
                    }
                }
            });
        }

        public void Dispose()
        {
            _cts?.Cancel();
            if (_isFirstInstance)
                _mutex.ReleaseMutex();

            _mutex.Dispose();
        }
    }
}
