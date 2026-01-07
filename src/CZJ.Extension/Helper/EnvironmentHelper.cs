namespace CZJ.Extension
{
    public static class EnvironmentHelper
    {
        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="name">环境变量名</param>
        /// <param name="value">值</param>
        public static void SetEnvironmentVariable(string name, object value)
        {
            System.Environment.SetEnvironmentVariable(name, value.ToString());
        }

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="name">环境变量名</param>
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name);
        }

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="name">环境变量名</param>
        public static T GetEnvironmentVariable<T>(string name) where T : IConvertible
        {
            return GetEnvironmentVariable(name).ConvertTo<T>();
        }
    }
}
