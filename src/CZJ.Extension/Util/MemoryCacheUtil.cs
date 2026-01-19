using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZJ.Extension.Util
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public class MemoryCacheUtil
    {
        private static readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly object lockObject = new object();

        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存的值，如果不存在则返回默认值</returns>
        public static T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            cache.TryGetValue(key, out T value);
            return value;
        }

        /// <summary>
        /// 设置缓存项（永不过期）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public static void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            cache.Set(key, value);
        }

        /// <summary>
        /// 设置缓存项（绝对过期时间）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        public static void Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            cache.Set(key, value, options);
        }

        /// <summary>
        /// 设置缓存项（滑动过期时间）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        public static void Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration
            };

            cache.Set(key, value, options);
        }

        /// <summary>
        /// 设置缓存项（指定分钟数后过期）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expirationMinutes">过期分钟数</param>
        public static void Set<T>(string key, T value, int expirationMinutes)
        {
            Set(key, value, DateTimeOffset.Now.AddMinutes(expirationMinutes));
        }

        /// <summary>
        /// 设置缓存项（自定义选项）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="options">缓存选项</param>
        public static void Set<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            cache.Set(key, value, options);
        }

        /// <summary>
        /// 获取或添加缓存项（如果不存在则创建）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="valueFactory">值工厂函数</param>
        /// <param name="expirationMinutes">过期分钟数</param>
        /// <returns>缓存的值</returns>
        public static T GetOrAdd<T>(string key, Func<T> valueFactory, int expirationMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            return cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes);
                return valueFactory();
            });
        }

        /// <summary>
        /// 获取或添加缓存项（自定义选项）
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="valueFactory">值工厂函数</param>
        /// <param name="options">缓存选项</param>
        /// <returns>缓存的值</returns>
        public static T GetOrAdd<T>(string key, Func<T> valueFactory, MemoryCacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            return cache.GetOrCreate(key, entry =>
            {
                entry.SetOptions(options);
                return valueFactory();
            });
        }

        /// <summary>
        /// 尝试获取缓存项
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">输出缓存值</param>
        /// <returns>存在返回 true，否则返回 false</returns>
        public static bool TryGetValue<T>(string key, out T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                value = default(T);
                return false;
            }

            return cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 判断缓存项是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>存在返回 true，否则返回 false</returns>
        public static bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            return cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 移除缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            cache.Remove(key);
        }

        /// <summary>
        /// 批量移除缓存项
        /// </summary>
        /// <param name="keys">缓存键集合</param>
        public static void RemoveRange(params string[] keys)
        {
            if (keys == null || keys.Length == 0)
                return;

            foreach (var key in keys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                    cache.Remove(key);
            }
        }
    }
}
