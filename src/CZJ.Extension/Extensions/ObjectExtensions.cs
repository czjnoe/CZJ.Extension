namespace CZJ.Extension
{
    public static class ObjectExtensions
    {
        public static T? Clone<T>(this T source)
        {
            var serialized = source.ToJson();
            return serialized.ToObject<T>();
        }

        public static Dictionary<string, object> NonNullPropertiesToDictionary(this object @object)
        {
            Dictionary<string, object> dictionary = new();

            foreach (var propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);

                if (value is not null)
                {
                    dictionary[propertyInfo.Name] = value;
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object?> PropertiesToDictionary(this object @object)
        {
            Dictionary<string, object?> dictionary = new();

            foreach (var propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);

                dictionary[propertyInfo.Name] = value;
            }

            return dictionary;
        }

        /// <summary>
        /// 是否默认值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="value">值</param>
        public static bool IsDefault<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }
    }
}
