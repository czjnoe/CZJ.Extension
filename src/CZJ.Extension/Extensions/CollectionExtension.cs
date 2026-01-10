namespace CZJ.Extension.Extensions
{
    public static class CollectionExtension
    {
        public static bool AddIf<T>(this ICollection<T> @this, Func<T, bool> predicate, T value)
        {
            if (predicate(value))
            {
                @this.Add(value);
                return true;
            }
            return false;
        }

        public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
        {
            if (!@this.Contains(value))
            {
                @this.Add(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T value in values)
            {
                @this.Add(value);
            }
        }

        public static bool ContainsAll<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T value in values)
            {
                if (!@this.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAny<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T value in values)
            {
                if (@this.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEmpty<T>(this ICollection<T> @this)
        {
            return @this.Count == 0;
        }

        public static bool IsNotEmpty<T>(this ICollection<T> @this)
        {
            return @this.Count != 0;
        }

        public static bool IsNotNullOrEmpty<T>(this ICollection<T> @this)
        {
            return @this != null && @this.Count != 0;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> @this)
        {
            return @this == null || @this.Count == 0;
        }

        public static void RemoveIf<T>(this ICollection<T> @this, T value, Func<T, bool> predicate)
        {
            if (predicate(value))
            {
                @this.Remove(value);
            }
        }

        public static void RemoveIfContains<T>(this ICollection<T> @this, T value)
        {
            if (@this.Contains(value))
            {
                @this.Remove(value);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T value in values)
            {
                @this.Remove(value);
            }
        }

        public static void RemoveRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (T value in values)
            {
                if (predicate(value))
                {
                    @this.Remove(value);
                }
            }
        }

        public static void RemoveRangeIfContains<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T value in values)
            {
                if (@this.Contains(value))
                {
                    @this.Remove(value);
                }
            }
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
        {
            foreach (var obj in values)
            {
                @this.Add(obj);
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 添加不重复的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T obj in values)
            {
                if (!@this.Contains(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 移除符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="where"></param>
        public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> @where)
        {
            foreach (var obj in @this.Where(where).ToList())
            {
                @this.Remove(obj);
            }
        }
    }
}
