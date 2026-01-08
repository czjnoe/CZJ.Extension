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

        public static List<Tuple<string, string>> GetEnumTuple<T>() where T : struct, Enum
        {
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                list.Add(new Tuple<string, string>(Convert.ToInt64(item).ToString(), item.ToString()));
            }
            return list;
        }

        /// <summary>
        /// 根据枚举获取字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="isReverse">是否反转</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDictionary<T>(bool isReverse = false) where T : struct, Enum
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                if (!isReverse)
                {
                    dic.Add(Convert.ToInt64(item).ToString(), item.ToString());
                }
                else
                {
                    dic.Add(item.ToString(), Convert.ToInt64(item).ToString());
                }
            }
            return dic;
        }

        /// <summary>
        /// 枚举 int 转 枚举名称
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="itemValue">int值</param>
        /// <returns></returns>
        public static string ConvertEnumToString<T>(int itemValue) where T : struct, Enum
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
        public static string GetEnumDescription<T>(this int itemValue, string defaultValue = "") where T : struct, Enum
        {
            Enum enumModel = Enum.Parse(typeof(T), itemValue.ToString()) as Enum;
            var attr = GetEnumAttribute(enumModel, typeof(DescriptionAttribute));
            return (attr as DescriptionAttribute)?.Description ?? defaultValue;
        }

        /// <summary>
        /// 判断key 在枚举 T 中是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(string key) where T : struct, Enum
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
        public static T ToEnum<T>(this string value, T defaultValue = default) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 从整数转换为枚举
        /// </summary>
        public static T ToEnum<T>(this int value, T defaultValue = default) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)Enum.ToObject(typeof(T), value);

            return defaultValue;
        }

        /// <summary>
        /// 获取枚举 name
        /// </summary>
        /// <typeparam name="T">枚举 类型</typeparam>
        /// <param name="value">枚举 value</param>
        /// <returns></returns>
        public static string GetEnumNameByValue<T>(this string value) where T : struct, Enum
        {
            var res = ToEnum<T>(value);
            return res.ToString();
        }

        /// <summary>
        /// 获取枚举 name
        /// </summary>
        /// <typeparam name="T">枚举 类型</typeparam>
        /// <param name="value">枚举 value</param>
        /// <returns></returns>
        public static string GetEnumNameByValue<T>(this int value) where T : struct, Enum
        {
            var res = ToEnum<T>(value);
            return res.ToString();
        }

        /// <summary>
        /// 通过枚举类型获取枚举列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetEnumNameList<T>() where T : struct, Enum
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
        /// 获取枚举值列表
        /// </summary>
        public static List<EnumItem<T>> GetEnumItems<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new EnumItem<T>
                {
                    Value = e,
                    Text = e.GetDescription(),
                    IntValue = ConvertUtil.To<int>(e),
                })
                .ToList();
        }

        /// <summary>
        /// 检查枚举是否有指定的标志位（用于 Flags 枚举）
        /// </summary>
        public static bool HasFlag<T>(this T value, T flag) where T : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (valueAsLong & flagAsLong) == flagAsLong;
        }

        /// <summary>
        /// 添加标志位（用于 Flags 枚举）
        /// </summary>
        public static T AddFlag<T>(this T value, T flag) where T : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (T)Enum.ToObject(typeof(T), valueAsLong | flagAsLong);
        }

        /// <summary>
        /// 移除标志位（用于 Flags 枚举）
        /// </summary>
        public static T RemoveFlag<T>(this T value, T flag) where T : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (T)Enum.ToObject(typeof(T), valueAsLong & ~flagAsLong);
        }
    }

    public class EnumItem<T> where T : Enum
    {
        public T Value { get; set; }
        public string Text { get; set; }
        public int IntValue { get; set; }
    }
}
