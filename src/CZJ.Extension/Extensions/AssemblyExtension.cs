using Microsoft.Extensions.DependencyModel;
using System.Runtime.Loader;

namespace CZJ.Extension
{
    public static class AssemblyExtension
    {
        #region Assembly

        /// <summary>
        /// 获取当前项目引用的所有程序集
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAllAssemblies()
        {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                list.Add(assembly);
            }

            return list;
        }

        /// <summary>
        /// 获取当前应用程序域中的所有程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssemblies(this AppDomain appDomain)
        {
            return appDomain.GetAssemblies();
        }

        /// <summary>
        /// 获取当前应用程序域中指定条件的程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssemblies(
            this AppDomain appDomain,
            Func<Assembly, bool> predicate)
        {
            return appDomain.GetAssemblies().Where(predicate);
        }

        /// <summary>
        /// 获取所有已加载的程序集（包括动态加载的）
        /// </summary>
        public static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            return AssemblyLoadContext.Default.Assemblies;
        }

        /// <summary>
        /// 根据名称模式获取程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssembliesByPattern(string pattern)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取应用程序自身的程序集
        /// </summary>
        public static IEnumerable<Assembly> GetApplicationAssemblies(string rootNamespace = null)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (string.IsNullOrWhiteSpace(rootNamespace))
            {
                // 尝试获取入口程序集的根命名空间
                var entryAssembly = Assembly.GetEntryAssembly();
                rootNamespace = entryAssembly?.GetName().Name;
            }

            return assemblies.Where(a =>
                !a.IsDynamic &&
                (string.IsNullOrWhiteSpace(rootNamespace) ||
                 a.FullName.StartsWith(rootNamespace, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 获取包含指定类型的程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssembliesContainingType<T>()
        {
            var targetType = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && GetTypesFromAssembly(a).Any(t =>
                    targetType.IsAssignableFrom(t) && t != targetType));
        }

        /// <summary>
        /// 获取包含指定特性的程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssembliesWithAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetCustomAttribute<TAttribute>() != null);
        }

        /// <summary>
        /// 从程序集中安全地获取所有类型（处理加载异常）
        /// </summary>
        public static IEnumerable<Type> GetTypesFromAssembly(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// 获取程序集中所有实现了指定接口的类型
        /// </summary>
        public static IEnumerable<Type> GetImplementations<TInterface>(this Assembly assembly)
        {
            var interfaceType = typeof(TInterface);
            return assembly.GetTypesFromAssembly()
                .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
        }

        /// <summary>
        /// 获取程序集中所有继承自指定基类的类型
        /// </summary>
        public static IEnumerable<Type> GetSubclassesOf<TBase>(this Assembly assembly)
        {
            var baseType = typeof(TBase);
            return assembly.GetTypesFromAssembly()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseType));
        }

        /// <summary>
        /// 获取程序集的简短名称
        /// </summary>
        public static string GetShortName(this Assembly assembly)
        {
            return assembly.GetName().Name;
        }

        /// <summary>
        /// 获取程序集的版本信息
        /// </summary>
        public static Version GetAssemblyVersion(this Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        #endregion

        public static string? Title(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            return attributes.Length > 0 ? ((AssemblyTitleAttribute)attributes[0]).Title : default;
        }

        public static string? Description(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            return attributes.Length > 0 ? ((AssemblyDescriptionAttribute)attributes[0]).Description : default;
        }

        public static string? Company(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            return attributes.Length > 0 ? ((AssemblyCompanyAttribute)attributes[0]).Company : default;
        }

        public static string? Product(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            return attributes.Length > 0 ? ((AssemblyProductAttribute)attributes[0]).Product : default;
        }

        public static string? Version(this Assembly? assembly)
        {
            return assembly?.GetName().Version?.ToString();
        }

        public static string? InformationalVersion(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            return attributes.Length > 0
                ? ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion
                : default;
        }

        public static string? FileVersion(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            return attributes.Length > 0 ? ((AssemblyFileVersionAttribute)attributes[0]).Version : default;
        }

        public static string? Copyright(this Assembly? assembly)
        {
            if (assembly == default)
            {
                return default;
            }

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            return attributes.Length > 0 ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : default;
        }

        public static DateTime? CompileTime(this Assembly? assembly)
        {
            var exePath = Process.GetCurrentProcess()?.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                return default;
            }

            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;
            const int ReadCount = 2048;
            var buffer = new byte[ReadCount];
            using (var s = new FileStream(exePath, FileMode.Open, FileAccess.Read))
            {
                s.Read(buffer, 0, ReadCount);
            }

            var i = BitConverter.ToInt32(buffer, PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, i + LinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            return dt.ToLocalTime();
        }
    }
}
