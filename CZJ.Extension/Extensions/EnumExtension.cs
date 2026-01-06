namespace CZJ.Extension
{
    public static class EnumExtension
    {
        public static string? GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault();

            return attribute?.Description;
        }

        public static List<Tuple<string, string>> GetEnumsToTuple<T>()
        {
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                list.Add(new Tuple<string, string>(Convert.ToInt64(item).ToString(), item.ToString()));
            }
            return list;
        }

        /// <summary>
        /// 枚举 int 转 枚举名称
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="itemValue">int值</param>
        /// <returns></returns>
        public static string ConvertEnumToString<T>(int itemValue)
        {
            return Enum.Parse(typeof(T), itemValue.ToString()).ToString();
        }

        public static Attribute GetEnumAttribute(this Enum value, Type attribute)
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                var fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    var attr = Attribute.GetCustomAttribute(fieldInfo,
                        attribute, false);
                    return attr;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取枚举描述内容
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value, string defaultValue = "")
        {
            var attr = GetEnumAttribute(value, typeof(DescriptionAttribute));
            return (attr as DescriptionAttribute)?.Description ?? defaultValue;
        }

        /// <summary>
        /// 获取枚举描述内容
        /// </summary>
        /// <param name="itemValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(this int itemValue, string defaultValue = "")
        {
            Enum enumModel = Enum.Parse(typeof(TEnum), itemValue.ToString()) as Enum;
            var attr = GetEnumAttribute(enumModel, typeof(DescriptionAttribute));
            return (attr as DescriptionAttribute)?.Description ?? defaultValue;
        }

        /// <summary>
        /// 判断key 在枚举 T 中是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(string key)
        {
            bool isDefined = Enum.IsDefined(typeof(T), key);
            return isDefined;
        }

        /// <summary>
        /// 根据枚举获取对应 value
        /// Enums.Sex.女.GetEnumToString()
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string GetEnumValueToString(this Enum @this)
        {
            return @this.GetEnumValueToInt().ToString();
        }

        public static int GetEnumValueToInt(this Enum @this)
        {
            return Convert.ToInt32(@this);
        }

        /// <summary>
        /// 获取枚举 
        /// </summary>
        /// <typeparam name="T">枚举 类型</typeparam>
        /// <param name="value">枚举 value</param>
        /// <returns></returns>
        public static T GetEnumByValue<T>(this object value)
        {
            var res = (T)Enum.Parse(typeof(T), value.ToString());
            return res;
        }

        /// <summary>
        /// 获取枚举 name
        /// </summary>
        /// <typeparam name="T">枚举 类型</typeparam>
        /// <param name="value">枚举 value</param>
        /// <returns></returns>
        public static string GetEnumNameByValue<T>(this object value)
        {
            var res = GetEnumByValue<T>(value);
            return res.ToString();
        }


        /// <summary>
        /// 通过枚举类型获取枚举列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetEnumNameList<T>()
        {
            List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            return list;
        }

        /// <summary>
        /// 通过枚举类型获取枚举 (特性DescriptionAttribute)描述 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetEnumDescriptionList<T>()
        {
            var enums = Enum.GetValues(typeof(T));
            List<string> list = new List<string>(enums.Length);
            foreach (Enum value in enums)
            {
                var attr = GetEnumAttribute(value, typeof(DescriptionAttribute));
                list.Add((attr as DescriptionAttribute)?.Description ?? value.ToString());
            }
            return list;
        }

        /// <summary>
        /// 通过枚举类型获取所有枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }
    }
}
