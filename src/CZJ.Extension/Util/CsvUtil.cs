namespace CZJ.Extension
{
    /// <summary>
    /// CSV 帮助类
    /// </summary>
    public class CsvUtil
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        private readonly char _delimiter;
        private readonly Encoding _encoding;

        public CsvUtil(char delimiter = ',', Encoding? encoding = null)
        {
            _delimiter = delimiter;
            _encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// 读取 CSV 文件到字符串列表
        /// </summary>
        public List<List<string>> Read(string filePath, bool hasHeader = true)
        {
            var result = new List<List<string>>();

            using var reader = new StreamReader(filePath, _encoding);
            if (hasHeader)
            {
                reader.ReadLine(); // 跳过标题行
            }

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                result.Add(ParseLine(line));
            }

            return result;
        }

        /// <summary>
        /// 读取 CSV 文件到泛型对象列表
        /// </summary>
        public List<T> Read<T>(string filePath) where T : new()
        {
            var result = new List<T>();
            using var reader = new StreamReader(filePath, _encoding);

            var headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
                return result;

            var headers = ParseLine(headerLine);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToList();

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = ParseLine(line);
                var obj = new T();

                for (int i = 0; i < Math.Min(headers.Count, values.Count); i++)
                {
                    var prop = properties.FirstOrDefault(p =>
                        p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));

                    if (prop != null)
                    {
                        SetPropertyValue(obj, prop, values[i]);
                    }
                }

                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// 写入字符串列表到 CSV 文件
        /// </summary>
        public void Write(string filePath, List<List<string>> data, List<string>? headers = null)
        {
            using var writer = new StreamWriter(filePath, false, _encoding);

            if (headers != null && headers.Count > 0)
            {
                writer.WriteLine(string.Join(_delimiter, headers.Select(EscapeField)));
            }

            foreach (var row in data)
            {
                writer.WriteLine(string.Join(_delimiter, row.Select(EscapeField)));
            }
        }

        /// <summary>
        /// 写入泛型对象列表到 CSV 文件
        /// </summary>
        public void Write<T>(string filePath, List<T> data)
        {
            if (data == null || data.Count == 0)
                return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToList();

            using var writer = new StreamWriter(filePath, false, _encoding);

            // 写入标题行
            writer.WriteLine(string.Join(_delimiter, properties.Select(p => EscapeField(p.Name))));

            // 写入数据行
            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);
                    return EscapeField(value?.ToString() ?? string.Empty);
                });
                writer.WriteLine(string.Join(_delimiter, values));
            }
        }

        /// <summary>
        /// 追加数据到 CSV 文件
        /// </summary>
        public void Append<T>(string filePath, List<T> data)
        {
            if (data == null || data.Count == 0)
                return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToList();

            using var writer = new StreamWriter(filePath, true, _encoding);

            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);
                    return EscapeField(value?.ToString() ?? string.Empty);
                });
                writer.WriteLine(string.Join(_delimiter, values));
            }
        }

        /// <summary>
        /// 解析 CSV 行，支持引号包裹的字段
        /// </summary>
        private List<string> ParseLine(string line)
        {
            var result = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == _delimiter && !inQuotes)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            result.Add(currentField.ToString());
            return result;
        }

        /// <summary>
        /// 转义 CSV 字段
        /// </summary>
        private string EscapeField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return field;

            if (field.Contains(_delimiter) || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }

        /// <summary>
        /// 设置属性值，支持类型转换
        /// </summary>
        private void SetPropertyValue(object obj, PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            try
            {
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (targetType == typeof(string))
                {
                    property.SetValue(obj, value);
                }
                else if (targetType == typeof(int))
                {
                    property.SetValue(obj, int.Parse(value));
                }
                else if (targetType == typeof(long))
                {
                    property.SetValue(obj, long.Parse(value));
                }
                else if (targetType == typeof(double))
                {
                    property.SetValue(obj, double.Parse(value, CultureInfo.InvariantCulture));
                }
                else if (targetType == typeof(decimal))
                {
                    property.SetValue(obj, decimal.Parse(value, CultureInfo.InvariantCulture));
                }
                else if (targetType == typeof(bool))
                {
                    property.SetValue(obj, bool.Parse(value));
                }
                else if (targetType == typeof(DateTime))
                {
                    property.SetValue(obj, DateTime.Parse(value));
                }
                else if (targetType.IsEnum)
                {
                    property.SetValue(obj, Enum.Parse(targetType, value));
                }
            }
            catch
            {
            }
        }
    }
}
