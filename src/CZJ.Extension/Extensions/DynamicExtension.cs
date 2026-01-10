using System.Dynamic;

namespace CZJ.Extension
{
    /// <summary>
    /// Dynamic 对象扩展类
    /// </summary>
    public static class DynamicExtension
    {
        /// <summary>
        /// 添加或更新属性
        /// </summary>
        public static void SetProperty(this ExpandoObject obj, string propertyName, object value)
        {
            var dict = (IDictionary<string, object>)obj;
            dict[propertyName] = value;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public static T GetProperty<T>(this ExpandoObject obj, string propertyName, T defaultValue = default)
        {
            var dict = (IDictionary<string, object>)obj;
            if (dict.TryGetValue(propertyName, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取属性值（object类型）
        /// </summary>
        public static object GetProperty(this ExpandoObject obj, string propertyName)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.TryGetValue(propertyName, out var value) ? value : null;
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        public static bool RemoveProperty(this ExpandoObject obj, string propertyName)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.Remove(propertyName);
        }

        /// <summary>
        /// 检查属性是否存在
        /// </summary>
        public static bool HasProperty(this ExpandoObject obj, string propertyName)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.ContainsKey(propertyName);
        }

        /// <summary>
        /// 获取所有属性名
        /// </summary>
        public static IEnumerable<string> GetPropertyNames(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.Keys;
        }

        /// <summary>
        /// 获取所有属性值
        /// </summary>
        public static IEnumerable<object> GetPropertyValues(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.Values;
        }

        /// <summary>
        /// 获取属性数量
        /// </summary>
        public static int GetPropertyCount(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.Count;
        }

        /// <summary>
        /// 清空所有属性
        /// </summary>
        public static void ClearProperties(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            dict.Clear();
        }

        /// <summary>
        /// 转换为字典
        /// </summary>
        public static Dictionary<string, object> ToDictionary(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            return new Dictionary<string, object>(dict);
        }

        /// <summary>
        /// 从字典创建 ExpandoObject
        /// </summary>
        public static ExpandoObject FromDictionary(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var kvp in dictionary)
            {
                expandoDict[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        /// <summary>
        /// 合并另一个 ExpandoObject 的属性（覆盖已存在的）
        /// </summary>
        public static void Merge(this ExpandoObject obj, ExpandoObject source, bool overwrite = true)
        {
            var targetDict = (IDictionary<string, object>)obj;
            var sourceDict = (IDictionary<string, object>)source;

            foreach (var kvp in sourceDict)
            {
                if (overwrite || !targetDict.ContainsKey(kvp.Key))
                {
                    targetDict[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// 克隆 ExpandoObject
        /// </summary>
        public static ExpandoObject Clone(this ExpandoObject obj)
        {
            var clone = new ExpandoObject();
            var cloneDict = (IDictionary<string, object>)clone;
            var sourceDict = (IDictionary<string, object>)obj;

            foreach (var kvp in sourceDict)
            {
                cloneDict[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        /// <summary>
        /// 批量设置属性
        /// </summary>
        public static void SetProperties(this ExpandoObject obj, IDictionary<string, object> properties)
        {
            var dict = (IDictionary<string, object>)obj;
            foreach (var kvp in properties)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// 批量删除属性
        /// </summary>
        public static void RemoveProperties(this ExpandoObject obj, params string[] propertyNames)
        {
            var dict = (IDictionary<string, object>)obj;
            foreach (var name in propertyNames)
            {
                dict.Remove(name);
            }
        }

        /// <summary>
        /// 从匿名对象创建 ExpandoObject
        /// </summary>
        public static ExpandoObject FromAnonymous(object anonymousObject)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (PropertyInfo property in anonymousObject.GetType().GetProperties())
            {
                expandoDict[property.Name] = property.GetValue(anonymousObject);
            }

            return expando;
        }

        /// <summary>
        /// 转换为强类型对象
        /// </summary>
        public static T ToObject<T>(this ExpandoObject obj) where T : class, new()
        {
            var result = new T();
            var dict = (IDictionary<string, object>)obj;
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (dict.TryGetValue(property.Name, out var value))
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(result, convertedValue);
                    }
                    catch
                    {
                        // 忽略转换失败的属性
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 从强类型对象创建 ExpandoObject
        /// </summary>
        public static ExpandoObject FromObject(object obj)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                expandoDict[property.Name] = property.GetValue(obj);
            }

            return expando;
        }

        /// <summary>
        /// 转换为 JSON 字符串
        /// </summary>
        public static string ToJson(this ExpandoObject obj)
        {
            return obj.ToJson();
        }

        /// <summary>
        /// 从 JSON 字符串创建 ExpandoObject
        /// </summary>
        public static ExpandoObject FromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
            return obj ?? new ExpandoObject();
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        public static bool TryGetProperty<T>(this ExpandoObject obj, string propertyName, out T value)
        {
            var dict = (IDictionary<string, object>)obj;
            if (dict.TryGetValue(propertyName, out var objValue))
            {
                try
                {
                    value = (T)Convert.ChangeType(objValue, typeof(T));
                    return true;
                }
                catch
                {
                    value = default;
                    return false;
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 过滤属性（返回包含指定属性的新对象）
        /// </summary>
        public static ExpandoObject FilterProperties(this ExpandoObject obj, params string[] propertyNames)
        {
            var result = new ExpandoObject();
            var sourceDict = (IDictionary<string, object>)obj;
            var resultDict = (IDictionary<string, object>)result;

            foreach (var name in propertyNames)
            {
                if (sourceDict.TryGetValue(name, out var value))
                {
                    resultDict[name] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// 排除属性（返回不包含指定属性的新对象）
        /// </summary>
        public static ExpandoObject ExcludeProperties(this ExpandoObject obj, params string[] propertyNames)
        {
            var result = new ExpandoObject();
            var sourceDict = (IDictionary<string, object>)obj;
            var resultDict = (IDictionary<string, object>)result;
            var excludeSet = new HashSet<string>(propertyNames);

            foreach (var kvp in sourceDict)
            {
                if (!excludeSet.Contains(kvp.Key))
                {
                    resultDict[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// 重命名属性
        /// </summary>
        public static bool RenameProperty(this ExpandoObject obj, string oldName, string newName)
        {
            var dict = (IDictionary<string, object>)obj;
            if (dict.TryGetValue(oldName, out var value))
            {
                dict.Remove(oldName);
                dict[newName] = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查对象是否为空（没有任何属性）
        /// </summary>
        public static bool IsEmpty(this ExpandoObject obj)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.Count == 0;
        }

        /// <summary>
        /// 获取属性值，如果不存在则设置默认值
        /// </summary>
        public static T GetOrAdd<T>(this ExpandoObject obj, string propertyName, T defaultValue)
        {
            var dict = (IDictionary<string, object>)obj;
            if (dict.TryGetValue(propertyName, out var value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            dict[propertyName] = defaultValue;
            return defaultValue;
        }

        /// <summary>
        /// 从 DataTable 转换为 ExpandoObject 列表
        /// </summary>
        public static List<ExpandoObject> FromDataTable(System.Data.DataTable dataTable)
        {
            var result = new List<ExpandoObject>();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return result;

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                var expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expando;

                foreach (System.Data.DataColumn column in dataTable.Columns)
                {
                    expandoDict[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
                }

                result.Add(expando);
            }

            return result;
        }

        /// <summary>
        /// 从单个 DataRow 转换为 ExpandoObject
        /// </summary>
        public static ExpandoObject FromDataRow(System.Data.DataRow row)
        {
            if (row == null)
                return new ExpandoObject();

            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (System.Data.DataColumn column in row.Table.Columns)
            {
                expandoDict[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
            }

            return expando;
        }

        /// <summary>
        /// 将 ExpandoObject 列表转换为 DataTable
        /// </summary>
        public static System.Data.DataTable ToDataTable(this IEnumerable<ExpandoObject> expandoObjects)
        {
            var dataTable = new System.Data.DataTable();

            if (expandoObjects == null || !expandoObjects.Any())
                return dataTable;

            // 从第一个对象获取列信息
            var firstObject = expandoObjects.First();
            var firstDict = (IDictionary<string, object>)firstObject;

            // 创建列
            foreach (var key in firstDict.Keys)
            {
                var value = firstDict[key];
                var columnType = value?.GetType() ?? typeof(object);
                dataTable.Columns.Add(key, columnType);
            }

            // 添加数据行
            foreach (var expando in expandoObjects)
            {
                var dict = (IDictionary<string, object>)expando;
                var row = dataTable.NewRow();

                foreach (var column in dataTable.Columns.Cast<System.Data.DataColumn>())
                {
                    if (dict.TryGetValue(column.ColumnName, out var value))
                    {
                        row[column.ColumnName] = value ?? DBNull.Value;
                    }
                    else
                    {
                        row[column.ColumnName] = DBNull.Value;
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// 将单个 ExpandoObject 转换为 DataTable（单行）
        /// </summary>
        public static System.Data.DataTable ToDataTable(this ExpandoObject expandoObject)
        {
            return ToDataTable(new[] { expandoObject });
        }

        /// <summary>
        /// 从 DataTable 转换为 ExpandoObject 列表（带类型映射）
        /// </summary>
        public static List<ExpandoObject> FromDataTableWithMapping(
            System.Data.DataTable dataTable,
            Dictionary<string, Func<object, object>> columnMappings = null)
        {
            var result = new List<ExpandoObject>();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return result;

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                var expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expando;

                foreach (System.Data.DataColumn column in dataTable.Columns)
                {
                    var value = row[column] == DBNull.Value ? null : row[column];

                    // 如果有自定义映射函数，则使用映射
                    if (columnMappings != null && columnMappings.TryGetValue(column.ColumnName, out var mapper))
                    {
                        value = mapper(value);
                    }

                    expandoDict[column.ColumnName] = value;
                }

                result.Add(expando);
            }

            return result;
        }

        /// <summary>
        /// 将 ExpandoObject 列表转换为 DataTable（带列名映射）
        /// </summary>
        public static System.Data.DataTable ToDataTableWithMapping(
            this IEnumerable<ExpandoObject> expandoObjects,
            Dictionary<string, string> columnNameMappings = null)
        {
            var dataTable = new System.Data.DataTable();

            if (expandoObjects == null || !expandoObjects.Any())
                return dataTable;

            var firstObject = expandoObjects.First();
            var firstDict = (IDictionary<string, object>)firstObject;

            // 创建列（使用映射后的列名）
            foreach (var key in firstDict.Keys)
            {
                var columnName = columnNameMappings?.TryGetValue(key, out var mappedName) == true
                    ? mappedName
                    : key;

                var value = firstDict[key];
                var columnType = value?.GetType() ?? typeof(object);
                dataTable.Columns.Add(columnName, columnType);
            }

            // 添加数据行
            foreach (var expando in expandoObjects)
            {
                var dict = (IDictionary<string, object>)expando;
                var row = dataTable.NewRow();

                foreach (var kvp in dict)
                {
                    var columnName = columnNameMappings?.TryGetValue(kvp.Key, out var mappedName) == true
                        ? mappedName
                        : kvp.Key;

                    if (dataTable.Columns.Contains(columnName))
                    {
                        row[columnName] = kvp.Value ?? DBNull.Value;
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
