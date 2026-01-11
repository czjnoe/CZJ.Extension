namespace CZJ.Extension
{
    /// <summary>
    /// 环境操作
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// DOTNET_ENVIRONMENT
        /// </summary>
        private const string DOTNET_ENVIRONMENT = "DOTNET_ENVIRONMENT";

        /// <summary>
        /// ASPNETCORE_ENVIRONMENT
        /// </summary>
        private const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// Development
        /// </summary>
        private const string Development = "Development";

        /// <summary>
        /// 换行符
        /// </summary>
        public static string NewLine => System.Environment.NewLine;

        /// <summary>
        /// 是否测试环境
        /// </summary>
        public static bool IsTest { get; set; }

        /// <summary>
        /// 获取环境名称
        /// </summary>
        public static string GetEnvironmentName()
        {
            var environment = RuntimeUtil.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT);
            if (environment.IsEmpty() == false)
                return environment;
            return RuntimeUtil.GetEnvironmentVariable(DOTNET_ENVIRONMENT);
        }

        /// <summary>
        /// 设置开发环境变量,如果环境变量已设置则忽略
        /// </summary>
        public static void SetDevelopment()
        {
            var environment = RuntimeUtil.GetEnvironmentVariable(DOTNET_ENVIRONMENT);
            if (environment.IsEmpty() == false)
                return;
            environment = RuntimeUtil.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT);
            if (environment.IsEmpty() == false)
                return;
            RuntimeUtil.SetEnvironmentVariable(DOTNET_ENVIRONMENT, Development);
            RuntimeUtil.SetEnvironmentVariable(ASPNETCORE_ENVIRONMENT, Development);
        }

        /// <summary>
        /// 是否开发环境
        /// </summary>
        public static bool IsDevelopment()
        {
            var environment = RuntimeUtil.GetEnvironmentVariable(DOTNET_ENVIRONMENT);
            if (environment == Development)
                return true;
            environment = RuntimeUtil.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT);
            if (environment == Development)
                return true;
            return false;
        }

        /// <summary>
        /// 获取当前区域文化
        /// </summary>
        public static CultureInfo GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// 获取当前UI区域文化
        /// </summary>
        public static CultureInfo GetCurrentUICulture()
        {
            return CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// 获取当前区域文化名称
        /// </summary>
        public static string GetCurrentCultureName()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        /// <summary>
        /// 获取当前UI区域文化名称
        /// </summary>
        public static string GetCurrentUICultureName()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        /// <summary>
        /// 获取当前区域文化信息列表,包含所有父区域文化
        /// </summary>
        public static List<CultureInfo> GetCurrentCultures()
        {
            return GetCultures(GetCurrentCulture());
        }

        /// <summary>
        /// 获取当前UI区域文化信息列表,包含所有父区域文化
        /// </summary>
        public static List<CultureInfo> GetCurrentUICultures()
        {
            return GetCultures(GetCurrentUICulture());
        }

        /// <summary>
        /// 获取区域文化信息列表,包含所有父区域文化
        /// </summary>
        /// <param name="culture">区域文化信息</param>
        public static List<CultureInfo> GetCultures(CultureInfo culture)
        {
            var result = new List<CultureInfo>();
            if (culture == null)
                return result;
            while (culture.Equals(culture.Parent) == false)
            {
                result.Add(culture);
                culture = culture.Parent;
            }
            return result;
        }

        /// <summary>
        /// 换行符
        /// </summary>
        public static string Line => System.Environment.NewLine;
        /// <summary>
        /// 是否Linux操作系统
        /// </summary>
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// 是否Windows操作系统
        /// </summary>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// 获取当前应用程序基路径
        /// </summary>
        public static string ApplicationBaseDirectory => AppContext.BaseDirectory;

        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        public static string Platform { get; } = RuntimeInformation.RuntimeIdentifier ?? "Unknown";

        public static string Framework { get; } = RuntimeInformation.FrameworkDescription ?? "Unknown";

        public static string BuildDate { get; } = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd");

        public static string OSString => $"{OS} {Environment.OSVersion.Version}";

        private static string OS
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return "Windows";
                if (OperatingSystem.IsMacOS())
                    return "macOS";
                if (OperatingSystem.IsLinux())
                    return "Linux";
                return "Unknown";
            }
        }
    }
}
