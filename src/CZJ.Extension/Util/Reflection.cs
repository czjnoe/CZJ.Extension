namespace CZJ.Extension
{
    /// <summary>
    /// 反射操作
    /// </summary>
    public static class Reflection
    {

        #region GetDescription(获取描述)

        /// <summary>
        /// 获取类型成员描述，使用DescriptionAttribute设置描述
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        public static string GetDescription(Type type, string memberName)
        {
            if (type == null)
                return string.Empty;
            if (string.IsNullOrWhiteSpace(memberName))
                return string.Empty;
            return GetDescription(type.GetTypeInfo().GetMember(memberName).FirstOrDefault());
        }

        /// <summary>
        /// 获取类型成员描述，使用DescriptionAttribute设置描述
        /// </summary>
        /// <param name="member">成员</param>
        public static string GetDescription(MemberInfo member)
        {
            if (member == null)
                return string.Empty;
            return member.GetCustomAttribute<DescriptionAttribute>() is { } attribute ? attribute.Description : member.Name;
        }

        #endregion

        #region GetDisplayName(获取显示名称)


        /// <summary>
        /// 获取显示名称，使用DisplayAttribute或DisplayNameAttribute设置显示名称
        /// </summary>
        public static string GetDisplayName(MemberInfo member)
        {
            if (member == null)
                return string.Empty;
            if (member.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>() is { } displayAttribute)
                return displayAttribute.Name;
            if (member.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() is { } displayNameAttribute)
                return displayNameAttribute.DisplayName;
            return string.Empty;
        }

        #endregion

        #region GetDisplayNameOrDescription(获取显示名称或描述)

        /// <summary>
        /// 获取属性显示名称或描述,使用DisplayAttribute或DisplayNameAttribute设置显示名称,使用DescriptionAttribute设置描述
        /// </summary>
        public static string GetDisplayNameOrDescription(MemberInfo member)
        {
            var result = GetDisplayName(member);
            return string.IsNullOrWhiteSpace(result) ? GetDescription(member) : result;
        }

        #endregion

        #region CreateInstance(动态创建实例)

        /// <summary>
        /// 动态创建实例
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static T CreateInstance<T>(params object[] parameters)
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        /// <summary>
        /// 创建指定类型的实例
        /// </summary>
        /// <typeparam name="T">要创建实例的类型</typeparam>
        /// <returns>类型的实例</returns>
        public static T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        #endregion

        #region FindImplementTypes(查找实现类型列表)

        /// <summary>
        /// 在指定的程序集中查找实现类型列表
        /// </summary>
        /// <typeparam name="TFind">查找类型</typeparam>
        /// <param name="assemblies">待查找的程序集列表</param>
        public static List<Type> FindImplementTypes<TFind>(params Assembly[] assemblies)
        {
            return FindImplementTypes(typeof(TFind), assemblies);
        }

        /// <summary>
        /// 在指定的程序集中查找实现类型列表
        /// </summary>
        /// <param name="findType">查找类型</param>
        /// <param name="assemblies">待查找的程序集列表</param>
        public static List<Type> FindImplementTypes(Type findType, params Assembly[] assemblies)
        {
            var result = new List<Type>();
            foreach (var assembly in assemblies)
                result.AddRange(GetTypes(findType, assembly));
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 获取类型列表
        /// </summary>
        private static List<Type> GetTypes(Type findType, Assembly assembly)
        {
            var result = new List<Type>();
            if (assembly == null)
                return result;
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return result;
            }
            foreach (var type in types)
                AddType(result, findType, type);
            return result;
        }

        /// <summary>
        /// 添加类型
        /// </summary>
        private static void AddType(List<Type> result, Type findType, Type type)
        {
            if (type.IsInterface || type.IsAbstract)
                return;
            if (findType.IsAssignableFrom(type) == false && MatchGeneric(findType, type) == false)
                return;
            result.Add(type);
        }

        /// <summary>
        /// 泛型匹配
        /// </summary>
        private static bool MatchGeneric(Type findType, Type type)
        {
            if (findType.IsGenericTypeDefinition == false)
                return false;
            var definition = findType.GetGenericTypeDefinition();
            foreach (var implementedInterface in type.FindInterfaces((filter, criteria) => true, null))
            {
                if (implementedInterface.IsGenericType == false)
                    continue;
                return definition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
            }
            return false;
        }

        #endregion

        #region GetDirectInterfaceTypes(获取直接接口类型列表)

        /// <summary>
        /// 获取直接接口类型列表,排除基接口类型
        /// </summary>
        /// <typeparam name="T">在该类型上查找接口</typeparam>
        /// <param name="baseInterfaceTypes">基接口类型列表,只返回继承了基接口的直接接口</param>
        public static List<Type> GetDirectInterfaceTypes<T>(params Type[] baseInterfaceTypes)
        {
            return GetDirectInterfaceTypes(typeof(T), baseInterfaceTypes);
        }

        /// <summary>
        /// 获取直接接口类型列表,排除基接口类型
        /// </summary>
        /// <param name="type">在该类型上查找接口</param>
        /// <param name="baseInterfaceTypes">基接口类型列表,只返回继承了基接口的直接接口</param>
        public static List<Type> GetDirectInterfaceTypes(Type type, params Type[] baseInterfaceTypes)
        {
            var interfaceTypes = type.GetInterfaces();
            var directInterfaceTypes = interfaceTypes.Except(interfaceTypes.SelectMany(t => t.GetInterfaces())).ToList();
            if (baseInterfaceTypes == null || baseInterfaceTypes.Length == 0)
                return directInterfaceTypes;
            return GetInterfaceTypes(directInterfaceTypes, baseInterfaceTypes);
        }

        /// <summary>
        /// 获取接口类型
        /// </summary>
        private static List<Type> GetInterfaceTypes(IEnumerable<Type> interfaceTypes, Type[] baseInterfaceTypes)
        {
            var result = new List<Type>();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.GetInterfaces().Any(baseInterfaceTypes.Contains) == false)
                    continue;
                if (interfaceType.IsGenericType && !interfaceType.IsGenericTypeDefinition && interfaceType.FullName == null)
                {
                    result.Add(interfaceType.GetGenericTypeDefinition());
                    continue;
                }
                result.Add(interfaceType);
            }
            return result;
        }

        #endregion

        #region GetInterfaceTypes(获取接口类型列表)

        /// <summary>
        /// 获取接口类型列表,排除基接口类型
        /// </summary>
        /// <typeparam name="T">在该类型上查找接口</typeparam>
        /// <param name="baseInterfaceTypes">基接口类型列表</param>
        public static List<Type> GetInterfaceTypes<T>(params Type[] baseInterfaceTypes)
        {
            return GetInterfaceTypes(typeof(T), baseInterfaceTypes);
        }

        /// <summary>
        /// 获取接口类型列表,排除基接口类型
        /// </summary>
        /// <param name="type">在该类型上查找接口</param>
        /// <param name="baseInterfaceTypes">基接口类型列表</param>
        public static List<Type> GetInterfaceTypes(Type type, params Type[] baseInterfaceTypes)
        {
            var interfaceTypes = type.GetInterfaces();
            if (baseInterfaceTypes == null || baseInterfaceTypes.Length == 0)
                return interfaceTypes.ToList();
            return GetInterfaceTypes(interfaceTypes, baseInterfaceTypes);
        }

        #endregion

        #region IsCollection(是否集合)

        /// <summary>
        /// 是否集合
        /// </summary>
        /// <param name="type">类型</param>
        public static bool IsCollection(Type type)
        {
            if (type.IsArray)
                return true;
            return type.IsGenericCollection();
        }

        #endregion

        #region IsBool(是否布尔类型)

        /// <summary>
        /// 是否布尔类型
        /// </summary>
        /// <param name="member">成员</param>
        public static bool IsBool(MemberInfo member)
        {
            if (member == null)
                return false;
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    return member.ToString() == "System.Boolean";
                case MemberTypes.Property:
                    return IsBool((PropertyInfo)member);
            }
            return false;
        }

        /// <summary>
        /// 是否布尔类型
        /// </summary>
        private static bool IsBool(PropertyInfo property)
        {
            return property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?);
        }

        #endregion

        #region IsEnum(是否枚举类型)

        /// <summary>
        /// 是否枚举类型
        /// </summary>
        /// <param name="member">成员</param>
        public static bool IsEnum(MemberInfo member)
        {
            if (member == null)
                return false;
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    return ((TypeInfo)member).IsEnum;
                case MemberTypes.Property:
                    return IsEnum((PropertyInfo)member);
            }
            return false;
        }

        /// <summary>
        /// 是否枚举类型
        /// </summary>
        private static bool IsEnum(PropertyInfo property)
        {
            if (property.PropertyType.GetTypeInfo().IsEnum)
                return true;
            var value = Nullable.GetUnderlyingType(property.PropertyType);
            if (value == null)
                return false;
            return value.GetTypeInfo().IsEnum;
        }

        #endregion

        #region IsDate(是否日期类型)

        /// <summary>
        /// 是否日期类型
        /// </summary>
        /// <param name="member">成员</param>
        public static bool IsDate(MemberInfo member)
        {
            if (member == null)
                return false;
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    return member.ToString() == "System.DateTime";
                case MemberTypes.Property:
                    return IsDate((PropertyInfo)member);
            }
            return false;
        }

        /// <summary>
        /// 是否日期类型
        /// </summary>
        private static bool IsDate(PropertyInfo property)
        {
            if (property.PropertyType == typeof(DateTime))
                return true;
            if (property.PropertyType == typeof(DateTime?))
                return true;
            return false;
        }

        #endregion

        #region IsInt(是否整型)

        /// <summary>
        /// 是否整型
        /// </summary>
        /// <param name="member">成员</param>
        public static bool IsInt(MemberInfo member)
        {
            if (member == null)
                return false;
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    return member.ToString() == "System.Int32" || member.ToString() == "System.Int16" || member.ToString() == "System.Int64";
                case MemberTypes.Property:
                    return IsInt((PropertyInfo)member);
            }
            return false;
        }

        /// <summary>
        /// 是否整型
        /// </summary>
        private static bool IsInt(PropertyInfo property)
        {
            if (property.PropertyType == typeof(int))
                return true;
            if (property.PropertyType == typeof(int?))
                return true;
            if (property.PropertyType == typeof(short))
                return true;
            if (property.PropertyType == typeof(short?))
                return true;
            if (property.PropertyType == typeof(long))
                return true;
            if (property.PropertyType == typeof(long?))
                return true;
            return false;
        }

        #endregion

        #region IsNumber(是否数值类型)

        /// <summary>
        /// 是否浮点型
        /// </summary>
        /// <param name="member">成员</param>
        public static bool IsNumber(MemberInfo member)
        {
            if (member == null)
                return false;
            if (IsInt(member))
                return true;
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    return member.ToString() == "System.Double" || member.ToString() == "System.Decimal" || member.ToString() == "System.Single";
                case MemberTypes.Property:
                    return IsNumber((PropertyInfo)member);
            }
            return false;
        }

        /// <summary>
        /// 是否数值类型
        /// </summary>
        private static bool IsNumber(PropertyInfo property)
        {
            if (property.PropertyType == typeof(double))
                return true;
            if (property.PropertyType == typeof(double?))
                return true;
            if (property.PropertyType == typeof(decimal))
                return true;
            if (property.PropertyType == typeof(decimal?))
                return true;
            if (property.PropertyType == typeof(float))
                return true;
            if (property.PropertyType == typeof(float?))
                return true;
            return false;
        }

        #endregion

        #region GetElementType(获取元素类型)

        /// <summary>
        /// 获取元素类型，如果是集合，返回集合的元素类型
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetElementType(Type type)
        {
            if (IsCollection(type) == false)
                return type;
            if (type.IsArray)
                return type.GetElementType();
            var genericArgumentsTypes = type.GetTypeInfo().GetGenericArguments();
            if (genericArgumentsTypes == null || genericArgumentsTypes.Length == 0)
                throw new ArgumentException(nameof(genericArgumentsTypes));
            return genericArgumentsTypes[0];
        }

        #endregion

        #region GetTopBaseType(获取顶级基类)

        /// <summary>
        /// 获取顶级基类
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public static Type GetTopBaseType<T>()
        {
            return GetTopBaseType(typeof(T));
        }

        /// <summary>
        /// 获取顶级基类
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetTopBaseType(Type type)
        {
            if (type == null)
                return null;
            if (type.IsInterface)
                return type;
            if (type.BaseType == typeof(object))
                return type;
            return GetTopBaseType(type.BaseType);
        }

        #endregion

        #region GetPropertyValue(获取属性值)

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">属性名</param>
        public static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance == null)
                return null;
            var property = instance.GetType().GetProperty(propertyName);
            return property == null ? null : property.GetValue(instance);
        }

        #endregion

        /// <summary>
        /// 获取类的静态属性的值
        /// </summary>
        /// <param name="type">要获取静态属性的类</param>
        /// <param name="propertyName">要获取的静态属性的名称</param>
        /// <returns>静态属性的值</returns>
        public static object GetStaticPropertyValue(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
            return property.GetValue(null);
        }

        /// <summary>
        /// 设置类的静态属性的值
        /// </summary>
        /// <param name="type">要设置静态属性的类</param>
        /// <param name="propertyName">要设置的静态属性的名称</param>
        /// <param name="value">要设置的静态属性的值</param>
        public static void SetStaticPropertyValue(Type type, string propertyName, object value)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
            property.SetValue(null, value);
        }

        /// <summary>
        /// 获取类的静态字段的值
        /// </summary>
        /// <param name="type">要获取静态字段的类</param>
        /// <param name="fieldName">要获取的静态字段的名称</param>
        /// <returns>静态字段的值</returns>
        public static object GetStaticFieldValue(Type type, string fieldName)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
            return field.GetValue(null);
        }

        /// <summary>
        /// 设置类的静态字段的值
        /// </summary>
        /// <param name="type">要设置静态字段的类</param>
        /// <param name="fieldName">要设置的静态字段的名称</param>
        /// <param name="value">要设置的静态字段的值</param>
        public static void SetStaticFieldValue(Type type, string fieldName, object value)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, value);
        }

        /// <summary>
        /// 动态调用类的静态方法
        /// </summary>
        /// <param name="type">要调用静态方法的类</param>
        /// <param name="methodName">要调用的静态方法的名称</param>
        /// <param name="arguments">要传递给静态方法的参数</param>
        /// <returns>静态方法的返回值</returns>
        public static object InvokeStaticMethod(Type type, string methodName, object[] arguments)
        {
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            return method.Invoke(null, arguments);
        }

        /// <summary>
        /// 动态调用类的实例方法
        /// </summary>
        /// <param name="instance">要调用实例方法的类实例</param>
        /// <param name="methodName">要调用的实例方法的名称</param>
        /// <param name="arguments">要传递给实例方法的参数</param>
        /// <returns>实例方法的返回值</returns>
        public static object InvokeMethod(object instance, string methodName, object[] arguments)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            return method.Invoke(instance, arguments);
        }

        /// <summary>
        /// 动态创建类的实例
        /// </summary>
        /// <param name="type">要创建实例的类</param>
        /// <param name="constructorArguments">要传递给构造函数的参数</param>
        /// <returns>类的新实例</returns>
        public static object CreateInstance(Type type, params object[] constructorArguments)
        {
            ConstructorInfo constructor = type.GetConstructor(GetParameterTypes(constructorArguments));
            return constructor.Invoke(constructorArguments);
        }

        /// <summary>
        /// 获取构造函数参数类型的数组
        /// </summary>
        /// <param name="parameters">要获取参数类型的参数数组</param>
        /// <returns>参数类型的数组</returns>
        private static Type[] GetParameterTypes(object[] parameters)
        {
            if (parameters == null)
            {
                return Type.EmptyTypes;
            }
            Type[] parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    parameterTypes[i] = typeof(object);
                }
                else
                {
                    parameterTypes[i] = parameters[i].GetType();
                }
            }
            return parameterTypes;
        }

        /// <summary>
        /// 获取指定类型的指定类型的特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性对象</returns>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }

        /// <summary>
        /// 获取指定类型的指定类型的特性数组
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性数组</returns>
        public static T[] GetAttributes<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().ToArray();
        }

        /// <summary>
        /// 获取类型的基类
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>基类</returns>
        public static Type GetBaseType(Type type)
        {
            return type.BaseType;
        }

        /// <summary>
        /// 判断类型是否实现了某个接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>是否实现</returns>
        public static bool HasInterface(Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 获取类型的所有属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性数组</returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 获取类型的所有字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>字段数组</returns>
        public static FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 获取类型的所有方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>方法数组</returns>
        public static MethodInfo[] GetMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 获取类型的所有事件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>事件数组</returns>
        public static EventInfo[] GetEvents(Type type)
        {
            return type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}
