namespace CZJ.Extension
{
    /// <summary>
    /// 字典操作扩展
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="source">字典数据</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">如果字典中不存在该键，则返回的默认值</param>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue = default)
        {
            if (source == null)
                return defaultValue;
            return source.TryGetValue(key, out var obj) ? obj : defaultValue;
        }

        public static void AddOrModify<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.TryGetValue(key, out _))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        /// 返回字典中键的集合
        /// </summary>
        /// <param name="dictionary">要获取键的字典</param>
        /// <returns>字典中所有键的集合</returns>
        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.Keys;
        }

        /// <summary>
        /// 返回字典中值的集合
        /// </summary>
        /// <param name="dictionary">要获取值的字典</param>
        /// <returns>字典中所有值的集合</returns>
        public static IEnumerable<TValue> GetValues<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.Values;
        }

        /// <summary>
        /// 将一个字典的所有键值对添加到另一个字典中
        /// </summary>
        /// <param name="destination">要添加键值对的目标字典</param>
        /// <param name="source">包含要添加键值对的源字典</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> destination, IDictionary<TKey, TValue> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (KeyValuePair<TKey, TValue> pair in source)
            {
                destination[pair.Key] = pair.Value;
            }
        }
    }
}
