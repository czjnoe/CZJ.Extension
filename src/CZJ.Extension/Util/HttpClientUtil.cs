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
                // 从工厂获取 HttpClient 实例
                client = _httpClientFactory.CreateClient();

                // 配置 HttpClient
                ConfigureHttpClient(client, options);

                // 创建请求消息
                using var request = CreateHttpRequestMessage(method, url, data, options);

                // 发送请求
                using var response = await client.SendAsync(request, cancellationToken);

                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers;

                // 读取响应内容
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                result.RawContent = content;

                // 判断请求是否成功
                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;

                    // 反序列化响应数据
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

            // 设置 Bearer Token
            if (!string.IsNullOrWhiteSpace(options.BearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.BearerToken);
            }

            // 设置基础认证
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

            // 添加自定义请求头
            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // 设置请求内容
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

    /// <summary>
    /// HTTP 请求配置选项
    /// </summary>
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
}
