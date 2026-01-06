namespace CZJ.Extension
{
    public static class JsonExtension
    {
        /// <summary>
        ///     对象转json
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T o)
        {
            if (o == null) return "";
            return JsonConvert.SerializeObject(o);
        }

        /// <summary>
        ///     json转对象
        /// </summary>
        /// <param name="str"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToObject<T>(this string str)
        {
            if (str == null) return default;
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObjectIgnoreNullWithCamel<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

                Formatting = Newtonsoft.Json.Formatting.Indented,

                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    }
                }
            });
        }

        public static bool IsValidJson<T>(this string jsonString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                    return false;

                JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch { }
            return false;
        }
    }
}
