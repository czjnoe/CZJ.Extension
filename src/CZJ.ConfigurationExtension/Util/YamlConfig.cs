namespace CZJ.ConfigurationExtension
{
    /// <summary>
    /// YAML配置文件帮助类
    /// </summary>
    public static class YamlConfig
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static IConfiguration _configuration;

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">配置键</param>
        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">配置键</param>
        public static T GetValue<T>(string key)
        {
            return GetConfiguration().GetValue<T>(key);
        }

        /// <summary>
        /// 获取配置选项
        /// </summary>
        /// <typeparam name="TOptions">配置选项类型</typeparam>
        /// <param name="section">配置节</param>
        public static TOptions Get<TOptions>(string section)
        {
            return GetSection(section).Get<TOptions>();
        }

        /// <summary>
        /// 获取配置节
        /// </summary>
        /// <param name="section">配置节</param>
        public static IConfigurationSection GetSection(string section)
        {
            return GetConfiguration().GetSection(section);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        private static IConfiguration GetConfiguration()
        {
            return _configuration ??= CreateConfiguration();
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="basePath">配置文件目录绝对路径</param>
        /// <param name="yamlFiles">YAML配置文件列表,默认已包含appsettings.yml</param>
        public static IConfiguration CreateConfiguration(string basePath = null, params string[] yamlFiles)
        {
            basePath ??= Common.ApplicationBaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddYamlFile("appsettings.yml", optional: true, reloadOnChange: false);

            var environment = Common.GetEnvironmentName();
            if (string.IsNullOrEmpty(environment) == false)
                builder.AddYamlFile($"appsettings.{environment}.yml", optional: true, reloadOnChange: false);

            builder.AddEnvironmentVariables();

            if (yamlFiles != null)
            {
                foreach (var file in yamlFiles)
                    builder.AddYamlFile(file, optional: true, reloadOnChange: false);
            }

            return builder.Build();
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="name">数据库连接字符串键名</param>
        public static string GetConnectionString(string name)
        {
            return GetConfiguration().GetConnectionString(name);
        }

        /// <summary>
        /// 从YAML文件直接反序列化为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="filePath">YAML文件路径</param>
        public static T DeserializeFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"YAML文件不存在: {filePath}");

            var yamlContent = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<T>(yamlContent);
        }

        /// <summary>
        /// 将对象序列化为YAML并保存到文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="filePath">保存的文件路径</param>
        public static void SerializeToFile<T>(T obj, string filePath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(obj);
            File.WriteAllText(filePath, yaml);
        }
    }
}