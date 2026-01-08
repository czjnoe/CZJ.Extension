namespace CZJ.Extension
{
    public static class CancellationTokenExtension
    {
        #region CancellationToken

        /// <summary>
        /// 检查 Token 关联的 Source 是否已被释放
        /// </summary>
        public static bool IsSourceDisposed(this CancellationToken token)
        {
            try
            {
                _ = token.WaitHandle;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        /// <summary>
        /// 安全地检查是否已请求取消（Source 释放时返回 true）
        /// </summary>
        public static bool SafeIsCancellationRequested(this CancellationToken token)
        {
            try
            {
                return token.IsCancellationRequested;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        /// <summary>
        /// 尝试获取取消状态，返回是否成功
        /// </summary>
        public static bool TryGetCancellationRequested(this CancellationToken token, out bool isCancelled)
        {
            try
            {
                isCancelled = token.IsCancellationRequested;
                return true;
            }
            catch (ObjectDisposedException)
            {
                isCancelled = false;
                return false;
            }
        }

        /// <summary>
        /// 如果 Source 已释放，返回 CancellationToken.None，否则返回原 Token
        /// </summary>
        public static CancellationToken GetSafeToken(this CancellationToken token)
        {
            return token.IsSourceDisposed() ? CancellationToken.None : token;
        }

        /// <summary>
        /// 安全地抛出取消异常（Source 已释放时不抛出）
        /// </summary>
        public static void SafeThrowIfCancellationRequested(this CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
            }
            catch (ObjectDisposedException)
            {
                // Source 已释放，不抛出异常
            }
        }

        /// <summary>
        /// 带超时的等待，返回是否已取消
        /// </summary>
        public static async Task<bool> WaitForCancellationAsync(this CancellationToken token, TimeSpan timeout)
        {
            try
            {
                await Task.Delay(timeout, token);
                return false; // 超时未取消
            }
            catch (OperationCanceledException)
            {
                return true; // 已取消
            }
            catch (ObjectDisposedException)
            {
                return true; // Source 已释放
            }
        }

        /// <summary>
        /// 创建一个在指定时间后自动取消的 Token
        /// </summary>
        public static CancellationToken WithTimeout(this CancellationToken token, TimeSpan timeout)
        {
            if (token.IsSourceDisposed() || token == CancellationToken.None)
            {
                var cts = new CancellationTokenSource(timeout);
                return cts.Token;
            }

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            linkedCts.CancelAfter(timeout);
            return linkedCts.Token;
        }

        /// <summary>
        /// 安全地注册回调（Source 已释放时不注册）
        /// </summary>
        public static CancellationTokenRegistration SafeRegister(
            this CancellationToken token,
            Action callback,
            bool useSynchronizationContext = false)
        {
            try
            {
                return token.Register(callback, useSynchronizationContext);
            }
            catch (ObjectDisposedException)
            {
                return default;
            }
        }

        /// <summary>
        /// 转换为 Task，当取消时 Task 完成
        /// </summary>
        public static Task AsTask(this CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            token.Register(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        #endregion

        #region CancellationTokenSource

        /// <summary>
        /// 检查 CancellationTokenSource 是否已被释放
        /// </summary>
        public static bool IsDisposed(this CancellationTokenSource cts)
        {
            try
            {
                _ = cts.Token;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        /// <summary>
        /// 安全地取消（已释放时不操作）
        /// </summary>
        public static bool SafeCancel(this CancellationTokenSource cts)
        {
            try
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                    return true;
                }
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 安全地在指定时间后取消
        /// </summary>
        public static bool SafeCancelAfter(this CancellationTokenSource cts, TimeSpan delay)
        {
            try
            {
                cts.CancelAfter(delay);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 安全地获取 Token（已释放时返回 CancellationToken.None）
        /// </summary>
        public static CancellationToken GetSafeToken(this CancellationTokenSource cts)
        {
            try
            {
                return cts.Token;
            }
            catch (ObjectDisposedException)
            {
                return CancellationToken.None;
            }
        }

        /// <summary>
        /// 重置 CancellationTokenSource（释放旧的并创建新的）
        /// </summary>
        public static CancellationTokenSource Reset(this CancellationTokenSource cts)
        {
            cts?.Dispose();
            return new CancellationTokenSource();
        }

        /// <summary>
        /// 安全地释放
        /// </summary>
        public static void SafeDispose(this CancellationTokenSource cts)
        {
            try
            {
                cts?.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // 已经释放，忽略
            }
        }

        /// <summary>
        /// 创建带超时的 CancellationTokenSource
        /// </summary>
        public static CancellationTokenSource CreateWithTimeout(TimeSpan timeout)
        {
            return new CancellationTokenSource(timeout);
        }

        /// <summary>
        /// 创建链接的 CancellationTokenSource，并在任意 Token 取消时释放
        /// </summary>
        public static CancellationTokenSource CreateLinkedWithAutoDispose(params CancellationToken[] tokens)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(tokens);

            // 当任意 token 取消时，自动释放 linkedCts
            linkedCts.Token.Register(() => linkedCts.Dispose());

            return linkedCts;
        }

        #endregion
    }
}
