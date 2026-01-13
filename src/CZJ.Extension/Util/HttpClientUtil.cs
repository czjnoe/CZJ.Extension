namespace CZJ.Extension
{
    /// <summary>
    /// HttpClient 帮助类
    /// </summary>
    public class HttpClientUtil
    {
        private static readonly Lazy<IHttpClientFactory> _factory = new(() =>
            new DefaultHttpClientFactory());

        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 构造函数（依赖注入）
        /// </summary>
        public HttpClientUtil(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// 构造函数（单例模式）
        /// </summary>
        public HttpClientUtil() : this(_factory.Value)
        {
        }

        #region GET 请求

        /// <summary>
        /// GET 请求（异步）
        /// </summary>
        public async Task<HttpResult<T>> GetAsync<T>(string url, HttpRequestOption options = null, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, url, null, options, cancellationToken);
        }

        /// <summary>
        /// GET 请求（同步）
        /// </summary>
        public HttpResult<T> Get<T>(string url, HttpRequestOption options = null)
        {
            return GetAsync<T>(url, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// GET 请求返回字符串（异步）
        /// </summary>
        public async Task<HttpResult<string>> GetStringAsync(string url, HttpRequestOption options = null, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<string>(HttpMethod.Get, url, null, options, cancellationToken);
        }

        /// <summary>
        /// GET 请求返回字符串（同步）
        /// </summary>
        public HttpResult<string> GetString(string url, HttpRequestOption options = null)
        {
            return GetStringAsync(url, options).GetAwaiter().GetResult();
        }

        #endregion

        #region POST 请求

        /// <summary>
        /// POST 请求（异步）
        /// </summary>
        public async Task<HttpResult<TResponse>> PostAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Post, url, data, options, cancellationToken);
        }

        /// <summary>
        /// POST 请求（同步）
        /// </summary>
        public HttpResult<TResponse> Post<TRequest, TResponse>(string url, TRequest data, HttpRequestOption options = null)
        {
            return PostAsync<TRequest, TResponse>(url, data, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// POST 请求无返回值（异步）
        /// </summary>
        public async Task<HttpResult<string>> PostAsync<TRequest>(
            string url,
            TRequest data,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<string>(HttpMethod.Post, url, data, options, cancellationToken);
        }

        /// <summary>
        /// POST 请求无返回值（同步）
        /// </summary>
        public HttpResult<string> Post<TRequest>(string url, TRequest data, HttpRequestOption options = null)
        {
            return PostAsync(url, data, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// POST Form 表单数据（异步）
        /// </summary>
        public async Task<HttpResult<T>> PostFormAsync<T>(
            string url,
            Dictionary<string, string> formData,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= new HttpRequestOption();
            options.ContentType = "application/x-www-form-urlencoded";

            return await SendRequestAsync<T>(HttpMethod.Post, url, formData, options, cancellationToken);
        }

        #endregion

        #region PUT 请求

        /// <summary>
        /// PUT 请求（异步）
        /// </summary>
        public async Task<HttpResult<TResponse>> PutAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Put, url, data, options, cancellationToken);
        }

        /// <summary>
        /// PUT 请求（同步）
        /// </summary>
        public HttpResult<TResponse> Put<TRequest, TResponse>(string url, TRequest data, HttpRequestOption options = null)
        {
            return PutAsync<TRequest, TResponse>(url, data, options).GetAwaiter().GetResult();
        }

        #endregion

        #region DELETE 请求

        /// <summary>
        /// DELETE 请求（异步）
        /// </summary>
        public async Task<HttpResult<T>> DeleteAsync<T>(string url, HttpRequestOption options = null, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<T>(HttpMethod.Delete, url, null, options, cancellationToken);
        }

        /// <summary>
        /// DELETE 请求（同步）
        /// </summary>
        public HttpResult<T> Delete<T>(string url, HttpRequestOption options = null)
        {
            return DeleteAsync<T>(url, options).GetAwaiter().GetResult();
        }

        #endregion

        #region 流式请求

        /// <summary>
        /// GET 流式请求（逐行读取）
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="onDataReceived">每接收到一行数据时的回调</param>
        /// <param name="options">请求配置</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task<HttpStreamResult> GetStreamAsync(
            string url,
            Action<string> onDataReceived,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            return await SendStreamRequestAsync(HttpMethod.Get, url, null, onDataReceived, options, cancellationToken);
        }

        /// <summary>
        /// POST 流式请求（逐行读取）
        /// </summary>
        public async Task<HttpStreamResult> PostStreamAsync<TRequest>(
            string url,
            TRequest data,
            Action<string> onDataReceived,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            return await SendStreamRequestAsync(HttpMethod.Post, url, data, onDataReceived, options, cancellationToken);
        }

        /// <summary>
        /// GET 流式请求（返回 IAsyncEnumerable）
        /// </summary>
        public async IAsyncEnumerable<string> GetStreamAsyncEnumerable(
            string url,
            HttpRequestOption options = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var line in SendStreamAsyncEnumerable(HttpMethod.Get, url, null, options, cancellationToken))
            {
                yield return line;
            }
        }

        /// <summary>
        /// POST 流式请求（返回 IAsyncEnumerable）
        /// </summary>
        public async IAsyncEnumerable<string> PostStreamAsyncEnumerable<TRequest>(
            string url,
            TRequest data,
            HttpRequestOption options = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var line in SendStreamAsyncEnumerable(HttpMethod.Post, url, data, options, cancellationToken))
            {
                yield return line;
            }
        }

        /// <summary>
        /// 下载文件流到本地
        /// </summary>
        public async Task<HttpStreamResult> DownloadFileAsync(
            string url,
            string localFilePath,
            Action<long, long> onProgressChanged = null,
            HttpRequestOption options = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            options ??= new HttpRequestOption();

            var result = new HttpStreamResult
            {
                Success = false
            };

            try
            {
                var client = _httpClientFactory.CreateClient();
                ConfigureHttpClient(client, options);

                using var request = CreateHttpRequestMessage(HttpMethod.Get, url, null, options);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers;

                if (!response.IsSuccessStatusCode)
                {
                    result.ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    return result;
                }

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                var downloadedBytes = 0L;

                using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    downloadedBytes += bytesRead;

                    onProgressChanged?.Invoke(downloadedBytes, totalBytes);
                }

                stopwatch.Stop();
                result.Success = true;
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.TotalBytesReceived = downloadedBytes;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.CancellationToken.IsCancellationRequested
                    ? "Download was cancelled"
                    : "Download timeout";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = $"Download error: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 发送流式请求（使用回调）
        /// </summary>
        private async Task<HttpStreamResult> SendStreamRequestAsync(
            HttpMethod method,
            string url,
            object data,
            Action<string> onDataReceived,
            HttpRequestOption options,
            CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            options ??= new HttpRequestOption();

            var result = new HttpStreamResult
            {
                Success = false
            };

            try
            {
                var client = _httpClientFactory.CreateClient();
                ConfigureHttpClient(client, options);

                using var request = CreateHttpRequestMessage(method, url, data, options);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers;

                if (!response.IsSuccessStatusCode)
                {
                    result.ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    return result;
                }

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream, options.Encoding);

                var lineCount = 0;
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    lineCount++;
                    onDataReceived?.Invoke(line);
                }

                stopwatch.Stop();
                result.Success = true;
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.LinesReceived = lineCount;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.CancellationToken.IsCancellationRequested
                    ? "Stream was cancelled"
                    : "Stream timeout";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = $"Stream error: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 发送流式请求（返回 IAsyncEnumerable）
        /// </summary>
        private async IAsyncEnumerable<string> SendStreamAsyncEnumerable(
            HttpMethod method,
            string url,
            object data,
            HttpRequestOption options,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            options ??= new HttpRequestOption();

            var client = _httpClientFactory.CreateClient();
            ConfigureHttpClient(client, options);

            using var request = CreateHttpRequestMessage(method, url, data, options);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream, options.Encoding);

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return line;
            }
        }

        #endregion

        #region 核心请求方法

        /// <summary>
        /// 发送 HTTP 请求的核心方法
        /// </summary>
        private async Task<HttpResult<TResponse>> SendRequestAsync<TResponse>(
            HttpMethod method,
            string url,
            object data,
            HttpRequestOption options,
            CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            options ??= new HttpRequestOption();

            var result = new HttpResult<TResponse>
            {
                Success = false
            };

            HttpClient client = null;

            try
            {
                client = _httpClientFactory.CreateClient();
                ConfigureHttpClient(client, options);

                using var request = CreateHttpRequestMessage(method, url, data, options);
                using var response = await client.SendAsync(request, cancellationToken);

                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers;

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                result.RawContent = content;

                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;

                    if (typeof(TResponse) == typeof(string))
                    {
                        result.Data = (TResponse)(object)content;
                    }
                    else if (!string.IsNullOrWhiteSpace(content))
                    {
                        result.Data = content.ToObject<TResponse>(options.JsonSetting);
                    }
                }
                else
                {
                    result.ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                }
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.CancellationToken.IsCancellationRequested
                    ? "Request was cancelled"
                    : "Request timeout";
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = $"Network error: {ex.Message}";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 配置 HttpClient
        /// </summary>
        private void ConfigureHttpClient(HttpClient client, HttpRequestOption options)
        {
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            if (!string.IsNullOrWhiteSpace(options.BearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.BearerToken);
            }

            if (!string.IsNullOrWhiteSpace(options.BasicAuthUsername))
            {
                var credentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{options.BasicAuthUsername}:{options.BasicAuthPassword}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);
            }
        }

        /// <summary>
        /// 创建 HTTP 请求消息
        /// </summary>
        private HttpRequestMessage CreateHttpRequestMessage(
            HttpMethod method,
            string url,
            object data,
            HttpRequestOption options)
        {
            var request = new HttpRequestMessage(method, url);

            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
            {
                if (options.ContentType.Contains("form-urlencoded") && data is Dictionary<string, string> formData)
                {
                    request.Content = new FormUrlEncodedContent(formData);
                }
                else
                {
                    var json = data.ToJson(options.JsonSetting);
                    request.Content = new StringContent(json, options.Encoding, options.ContentType);
                }
            }

            return request;
        }

        #endregion
    }

    /// <summary>
    /// 默认的 HttpClientFactory 实现
    /// </summary>
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        private static readonly Lazy<HttpClient> _sharedClient = new(() =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                MaxConnectionsPerServer = 10
            };

            return new HttpClient(handler, disposeHandler: false);
        });

        public HttpClient CreateClient(string name)
        {
            return _sharedClient.Value;
        }
    }

    public class HttpRequestOption
    {
        /// <summary>
        /// 请求超时时间（秒），默认100秒
        /// </summary>
        public int TimeoutSeconds { get; set; } = 100;

        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <summary>
        /// 内容类型，默认 application/json
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 字符编码，默认 UTF-8
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 是否自动重定向，默认 true
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = true;

        /// <summary>
        /// Bearer Token（用于授权）
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// 基础认证用户名
        /// </summary>
        public string BasicAuthUsername { get; set; }

        /// <summary>
        /// 基础认证密码
        /// </summary>
        public string BasicAuthPassword { get; set; }

        /// <summary>
        /// 是否忽略 SSL 证书验证（仅开发环境使用）
        /// </summary>
        public bool IgnoreSslErrors { get; set; } = false;

        /// <summary>
        /// JSON 序列化选项
        /// </summary>
        public JsonSerializerSettings JsonSetting { get; set; } = new()
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
        };
    }

    /// <summary>
    /// HTTP 响应结果
    /// </summary>
    public class HttpResult<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// HTTP 状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 原始响应内容
        /// </summary>
        public string RawContent { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 响应头
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        /// 请求耗时（毫秒）
        /// </summary>
        public long ElapsedMilliseconds { get; set; }
    }

    /// <summary>
    /// HTTP 流式响应结果
    /// </summary>
    public class HttpStreamResult
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public int LinesReceived { get; set; }
        public long TotalBytesReceived { get; set; }
    }
}