namespace CZJ.Extension
{
    // <summary>
    /// JSON文件读写帮助类
    /// </summary>
    public class JsonConfigHelper
    {
        private readonly string _filePath;
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// 获取当前JSON文件路径
        /// </summary>
        public string FilePath => _filePath;

        /// <summary>
        /// 初始化JSON文件帮助类
        /// </summary>
        /// <param name="filePath">JSON文件路径（支持相对路径和绝对路径）</param>
        /// <param name="settings">JSON序列化设置（可选）</param>
        public JsonConfigHelper(string filePath, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            // 如果是相对路径，转换为基于应用程序目录的绝对路径
            _filePath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            _jsonSettings = settings ?? new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        /// <summary>
        /// 读取整个JSON文件并反序列化为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>反序列化后的对象，文件不存在则返回默认值</returns>
        public T Read<T>() where T : class
        {
            if (!File.Exists(_filePath))
                return default;

            try
            {
                var jsonContent = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<T>(jsonContent, _jsonSettings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"读取JSON文件失败 [{_filePath}]: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 根据键路径获取配置值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="keyPath">键路径，使用冒号分隔，例如 "Database:ConnectionString"</param>
        /// <returns>指定路径的值</returns>
        public T GetValue<T>(string keyPath)
        {
            if (string.IsNullOrWhiteSpace(keyPath))
                throw new ArgumentException("键路径不能为空", nameof(keyPath));

            if (!File.Exists(_filePath))
                return default;

            try
            {
                var jsonContent = File.ReadAllText(_filePath);
                var jObject = JObject.Parse(jsonContent);
                var token = NavigateToToken(jObject, keyPath);

                return token == null ? default : token.ToObject<T>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"获取配置值失败 [{keyPath}]: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 更新指定键路径的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="keyPath">键路径，使用冒号分隔</param>
        /// <param name="value">新值</param>
        /// <param name="createIfNotExists">如果文件不存在是否创建，默认true</param>
        public void UpdateValue<T>(string keyPath, T value, bool createIfNotExists = true)
        {
            if (string.IsNullOrWhiteSpace(keyPath))
                throw new ArgumentException("键路径不能为空", nameof(keyPath));

            try
            {
                JObject jObject;

                if (File.Exists(_filePath))
                {
                    var jsonContent = File.ReadAllText(_filePath);
                    jObject = JObject.Parse(jsonContent);
                }
                else if (createIfNotExists)
                {
                    EnsureDirectoryExists();
                    jObject = new JObject();
                }
                else
                {
                    throw new FileNotFoundException($"文件不存在: {_filePath}");
                }

                var pathSegments = keyPath.Split(':');
                JToken currentToken = jObject;

                // 导航到父节点，不存在则创建
                for (int i = 0; i < pathSegments.Length - 1; i++)
                {
                    var segment = pathSegments[i];

                    if (currentToken[segment] == null)
                    {
                        currentToken[segment] = new JObject();
                    }

                    currentToken = currentToken[segment];
                }

                // 设置最终值
                var lastSegment = pathSegments[^1];
                currentToken[lastSegment] = JToken.FromObject(value);

                // 保存
                var json = JsonConvert.SerializeObject(jObject, _jsonSettings);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"更新配置失败 [{keyPath}]: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 将对象序列化并写入JSON文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">要保存的对象</param>
        /// <param name="createIfNotExists">如果目录不存在是否创建，默认true</param>
        public void Write<T>(T data, bool createIfNotExists = true) where T : class
        {
            try
            {
                if (createIfNotExists)
                {
                    EnsureDirectoryExists();
                }

                var json = JsonConvert.SerializeObject(data, _jsonSettings);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"写入JSON文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 删除指定键路径的值
        /// </summary>
        /// <param name="keyPath">键路径</param>
        public void DeleteValue(string keyPath)
        {
            if (string.IsNullOrWhiteSpace(keyPath))
                throw new ArgumentException("键路径不能为空", nameof(keyPath));

            if (!File.Exists(_filePath))
                return;

            try
            {
                var jsonContent = File.ReadAllText(_filePath);
                var jObject = JObject.Parse(jsonContent);
                var pathSegments = keyPath.Split(':');
                JToken currentToken = jObject;

                // 导航到父节点
                for (int i = 0; i < pathSegments.Length - 1; i++)
                {
                    if (currentToken[pathSegments[i]] == null)
                        return;
                    currentToken = currentToken[pathSegments[i]];
                }

                // 删除目标节点
                if (currentToken is JObject jObj)
                {
                    jObj.Remove(pathSegments[^1]);
                }

                // 保存
                var json = JsonConvert.SerializeObject(jObject, _jsonSettings);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"删除配置失败 [{keyPath}]: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 检查JSON文件是否存在
        /// </summary>
        /// <returns>文件是否存在</returns>
        public bool Exists()
        {
            return File.Exists(_filePath);
        }

        /// <summary>
        /// 检查JSON文件中是否存在指定键路径
        /// </summary>
        /// <param name="keyPath">键路径</param>
        /// <returns>键路径是否存在</returns>
        public bool KeyExists(string keyPath)
        {
            if (!Exists() || string.IsNullOrWhiteSpace(keyPath))
                return false;

            try
            {
                var jsonContent = File.ReadAllText(_filePath);
                var jObject = JObject.Parse(jsonContent);
                var token = NavigateToToken(jObject, keyPath);
                return token != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除JSON文件
        /// </summary>
        public void DeleteFile()
        {
            if (Exists())
            {
                File.Delete(_filePath);
            }
        }

        /// <summary>
        /// 创建空的JSON文件（根对象为空对象）
        /// </summary>
        public void CreateEmpty()
        {
            EnsureDirectoryExists();
            File.WriteAllText(_filePath, "{}");
        }

        /// <summary>
        /// 合并另一个JSON对象到当前文件
        /// </summary>
        /// <param name="jsonToMerge">要合并的JSON字符串或对象</param>
        public void Merge(object jsonToMerge)
        {
            try
            {
                JObject existingObject;

                if (File.Exists(_filePath))
                {
                    var jsonContent = File.ReadAllText(_filePath);
                    existingObject = JObject.Parse(jsonContent);
                }
                else
                {
                    EnsureDirectoryExists();
                    existingObject = new JObject();
                }

                JObject mergeObject;
                if (jsonToMerge is string jsonString)
                {
                    mergeObject = JObject.Parse(jsonString);
                }
                else
                {
                    mergeObject = JObject.FromObject(jsonToMerge);
                }

                existingObject.Merge(mergeObject, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });

                var json = JsonConvert.SerializeObject(existingObject, _jsonSettings);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"合并JSON失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 导航到指定路径的Token
        /// </summary>
        private static JToken NavigateToToken(JObject rootObject, string keyPath)
        {
            var pathSegments = keyPath.Split(':');
            JToken currentToken = rootObject;

            foreach (var segment in pathSegments)
            {
                if (currentToken == null) return null;
                currentToken = currentToken[segment];
            }

            return currentToken;
        }

        /// <summary>
        /// 确保文件所在目录存在
        /// </summary>
        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
