using System.Collections.Specialized;
using System.Dynamic;

namespace CZJ.Extension
{
    /// <summary>
    /// 字典操作扩展
    /// </summary>
    public static class DictionaryExtension
    {
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> @this)
        {
            if (@this == null)
            {
                return null;
            }

            var col = new NameValueCollection();
            foreach (var item in @this)
            {
                col.Add(item.Key, item.Value);
            }
            return col;
        }

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

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, value));
            }

            return @this[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, valueFactory(key)));
            }

            return @this[key];
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

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                @this[key] = value;
            }

            return @this[key];
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValue));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValueFactory(key)));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        public static bool ContainsAllKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, params TKey[] keys)
        {
            foreach (TKey value in keys)
            {
                if (!@this.ContainsKey(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAnyKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, params TKey[] keys)
        {
            foreach (TKey value in keys)
            {
                if (@this.ContainsKey(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RemoveIfContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.ContainsKey(key))
            {
                @this.Remove(key);
            }
        }

        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> @this)
        {
            return new SortedDictionary<TKey, TValue>(@this);
        }

        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> @this, IComparer<TKey> comparer)
        {
            return new SortedDictionary<TKey, TValue>(@this, comparer);
        }

        public static ExpandoObject ToExpando(this IDictionary<string, object> @this)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var item in @this)
            {
                if (item.Value is IDictionary<string, object>)
                {
                    var d = (IDictionary<string, object>)item.Value;
                    expandoDict.Add(item.Key, d.ToExpando());
                }
                else
                {
                    expandoDict.Add(item);
                }
            }

            return expando;
        }

        public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, value);
                return true;
            }

            return false;
        }

        public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, valueFactory());
                return true;
            }

            return false;
        }

        public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, valueFactory(key));
                return true;
            }

            return false;
        }

        public static Hashtable ToHashtable(this IDictionary @this)
        {
            return new Hashtable(@this);
        }

        public static IDictionary<string, object> ToDictionary(this NameValueCollection @this)
        {
            var dict = new Dictionary<string, object>();

            if (@this != null)
            {
                foreach (string key in @this.AllKeys)
                {
                    dict.Add(key, @this[key]);
                }
            }

            return dict;
        }
    }
}
