namespace CZJ.Extension
{
    public class IniUtil
    {
        private readonly string _filePath;
        private readonly ReaderWriterLockSlim _lock = new();

        public IniUtil(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        #region Public API

        public void Write<T>(string section, string key, T value)
        {
            _lock.EnterWriteLock();
            try
            {
                var data = Load();
                section = Normalize(section);
                key = Normalize(key);

                if (!data.TryGetValue(section, out var sectionDict))
                {
                    sectionDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    data[section] = sectionDict;
                }

                sectionDict[key] = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;

                Save(data);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T Read<T>(string section, string key, T defaultValue = default!)
        {
            _lock.EnterReadLock();
            try
            {
                var data = Load();
                section = Normalize(section);
                key = Normalize(key);

                if (data.TryGetValue(section, out var sectionDict) &&
                    sectionDict.TryGetValue(key, out var value))
                {
                    return ConvertUtil.To(value, defaultValue);
                }

                return defaultValue;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool ContainsKey(string section, string key)
        {
            _lock.EnterReadLock();
            try
            {
                var data = Load();
                return data.TryGetValue(section, out var s) && s.ContainsKey(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void RemoveKey(string section, string key)
        {
            _lock.EnterWriteLock();
            try
            {
                var data = Load();
                if (data.TryGetValue(section, out var s))
                {
                    s.Remove(key);
                    Save(data);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveSection(string section)
        {
            _lock.EnterWriteLock();
            try
            {
                var data = Load();
                data.Remove(section);
                Save(data);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #endregion

        #region Core

        private Dictionary<string, Dictionary<string, string>> Load()
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            string currentSection = string.Empty;

            foreach (var rawLine in File.ReadAllLines(_filePath, Encoding.UTF8))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = Normalize(line[1..^1]);
                    if (!result.ContainsKey(currentSection))
                        result[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                var index = line.IndexOf('=');
                if (index <= 0)
                    continue;

                var key = Normalize(line[..index]);
                var value = line[(index + 1)..].Trim();

                if (!result.ContainsKey(currentSection))
                    result[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                result[currentSection][key] = value;
            }

            return result;
        }

        private void Save(Dictionary<string, Dictionary<string, string>> data)
        {
            var sb = new StringBuilder();

            foreach (var section in data)
            {
                if (!string.IsNullOrEmpty(section.Key))
                    sb.AppendLine($"[{section.Key}]");

                foreach (var kv in section.Value)
                    sb.AppendLine($"{kv.Key}={kv.Value}");

                sb.AppendLine();
            }

            File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string Normalize(string value) => value?.Trim() ?? string.Empty;

        private void EnsureFileExists()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, string.Empty, Encoding.UTF8);
        }

        #endregion

        public T ReadSection<T>(string section) where T : new()
        {
            _lock.EnterReadLock();
            try
            {
                var data = Load();
                section = Normalize(section);

                if (!data.TryGetValue(section, out var sectionDict))
                    return new T();

                var result = new T();
                var type = typeof(T);

                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanWrite)
                        continue;

                    if (!sectionDict.TryGetValue(prop.Name, out var rawValue))
                        continue;

                    try
                    {
                        object? value;

                        if (prop.PropertyType.IsEnum)
                        {
                            value = Enum.Parse(prop.PropertyType, rawValue, true);
                        }
                        else
                        {
                            value = Convert.ChangeType(
                                rawValue,
                                prop.PropertyType,
                                CultureInfo.InvariantCulture
                            );
                        }

                        prop.SetValue(result, value);
                    }
                    catch
                    {
                        // 忽略转换失败，保留默认值
                    }
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


        public bool ContainsSection(string section)
        {
            _lock.EnterReadLock();
            try
            {
                return Load().ContainsKey(Normalize(section));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


    }
}
