namespace CZJ.Extension
{
    public static class ReflectionExtension
    {
        /// <summary>
        /// 获取对象的所有公共实例属性名称
        /// </summary>
        public static string[] GetPropertyNames(this object obj, bool includeInherited = true)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return GetPropertyNames(obj.GetType(), includeInherited);
        }

        /// <summary>
        /// 获取类型的所有公共实例属性名称
        /// </summary>
        public static string[] GetPropertyNames(this Type type, bool includeInherited = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return GetPropertyInfos(type, includeInherited)
                .Select(p => p.Name)
                .ToArray();
        }

        /// <summary>
        /// 获取属性的真实显示名称（优先使用Display/DisplayName特性，不存在时返回属性名）
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="propertyName">属性名称</param>
        public static string GetPropertyDisplayName(this object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return GetPropertyDisplayName(obj.GetType(), propertyName);
        }

        /// <summary>
        /// 获取属性的真实显示名称（优先使用Display/DisplayName特性，不存在时返回属性名）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="propertyName">属性名称</param>
        public static string GetPropertyDisplayName(this Type type, string propertyName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("属性名不能为空", nameof(propertyName));

            // 获取属性信息
            PropertyInfo property = GetPropertyInfo(type, propertyName);
            if (property == null) return propertyName;

            string displayName = GetAttributeDisplayName(property);
            return displayName;
        }

        /// <summary>
        /// 获取特性名称（优先级：Display > DisplayName > 属性名）
        /// </summary>
        public static string GetAttributeDisplayName(this PropertyInfo property)
        {
            // 1. 检查DisplayAttribute（System.ComponentModel.DataAnnotations）
            var displayAttr = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayAttr != null && !string.IsNullOrWhiteSpace(displayAttr.DisplayName))
            {
                return displayAttr.DisplayName;
            }

            // 2. 检查DisplayNameAttribute（System.ComponentModel）
            var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && !string.IsNullOrWhiteSpace(displayNameAttr.DisplayName))
            {
                return displayNameAttr.DisplayName;
            }

            // 3. 返回原始属性名
            return property.Name;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            PropertyInfo property = GetPropertyInfo(obj, propertyName);
            return property?.GetValue(obj);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            PropertyInfo property = GetPropertyInfo(obj, propertyName);
            property?.SetValue(obj, value);
        }

        /// <summary>
        /// 检查对象是否包含指定属性
        /// </summary>
        public static bool HasProperty(this object obj, string propertyName)
        {
            return GetPropertyInfo(obj, propertyName) != null;
        }

        /// <summary>
        /// 获取类型的所有公共实例属性
        /// </summary>
        private static PropertyInfo[] GetPropertyInfos(this Type type, bool includeInherited)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (!includeInherited) flags |= BindingFlags.DeclaredOnly;
            return type.GetProperties(flags);
        }

        /// <summary>
        /// 获取类型的所有公共实例属性
        /// </summary>
        public static PropertyInfo[] GetPropertyInfos(this object obj, bool includeInherited = false)
        {
            return obj.GetType().GetPropertyInfos(includeInherited);
        }

        /// <summary>
        /// 获取对象的属性信息
        /// </summary>
        public static PropertyInfo GetPropertyInfo(this object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return GetPropertyInfo(obj.GetType(), propertyName);
        }

        /// <summary>
        /// 获取类型的属性信息
        /// </summary>
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            return GetPropertyInfos(type, true)
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取实例上的属性值
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <param name="instance">成员所在的类实例</param>
        public static object GetPropertyValue(this MemberInfo member, object instance)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            return instance.GetType().GetProperty(member.Name)?.GetValue(instance);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TAttributeType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<TAttributeType> GetCustomAttributes<TAttributeType>(this Type type) where TAttributeType : Attribute
        {
            var attributeType = typeof(TAttributeType);
            if (!type.IsDefined(attributeType, false))
            {
                return new List<TAttributeType>();
            }
            var attrs = type.GetCustomAttributes(attributeType, false);
            if (attrs.Length > 0)
            {
                List<TAttributeType> list = new List<TAttributeType>();
                int length = attrs.Length;
                for (int i = 0; i < length; i++)
                {
                    list.Add(attrs[i] as TAttributeType);
                }
                return list;
            }
            else
            {
                return new List<TAttributeType>();
            }
        }

        /// <summary>
        /// 获取特性 第一个
        /// </summary>
        /// <typeparam name="TAttributeType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TAttributeType GetFirstCustomAttribute<TAttributeType>(this Type type) where TAttributeType : Attribute
        {
            return GetCustomAttributes<TAttributeType>(type).FirstOrDefault();
        }


        /// <summary>
        /// 获取类的静态属性的值
        /// </summary>
        /// <param name="type">要获取静态属性的类</param>
        /// <param name="propertyName">要获取的静态属性的名称</param>
        /// <returns>静态属性的值</returns>
        public static object GetStaticPropertyValue(this Type type, string propertyName) => Reflection.GetStaticPropertyValue(type, propertyName);


        /// <summary>
        /// 设置类的静态属性的值
        /// </summary>
        /// <param name="type">要设置静态属性的类</param>
        /// <param name="propertyName">要设置的静态属性的名称</param>
        /// <param name="value">要设置的静态属性的值</param>
        public static void SetStaticPropertyValue(this Type type, string propertyName, object value) => Reflection.SetStaticPropertyValue(type, propertyName, value);


        /// <summary>
        /// 获取类的静态字段的值
        /// </summary>
        /// <param name="type">要获取静态字段的类</param>
        /// <param name="fieldName">要获取的静态字段的名称</param>
        /// <returns>静态字段的值</returns>
        public static object GetStaticFieldValue(this Type type, string fieldName) => Reflection.GetStaticFieldValue(type, fieldName);

        /// <summary>
        /// 设置类的静态字段的值
        /// </summary>
        /// <param name="type">要设置静态字段的类</param>
        /// <param name="fieldName">要设置的静态字段的名称</param>
        /// <param name="value">要设置的静态字段的值</param>
        public static void SetStaticFieldValue(this Type type, string fieldName, object value) => Reflection.SetStaticFieldValue(type, fieldName, value);


        /// <summary>
        /// 动态调用类的静态方法
        /// </summary>
        /// <param name="type">要调用静态方法的类</param>
        /// <param name="methodName">要调用的静态方法的名称</param>
        /// <param name="arguments">要传递给静态方法的参数</param>
        /// <returns>静态方法的返回值</returns>
        public static object InvokeStaticMethod(this Type type, string methodName, object[] arguments) => Reflection.InvokeStaticMethod(type, methodName, arguments);
    }
}
