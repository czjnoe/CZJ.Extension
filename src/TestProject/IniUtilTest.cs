using TestProject.Models;

namespace TestProject
{
    [TestClass]
    public sealed class IniUtilTestTest
    {
        [TestMethod]
        public void Test()
        {
            var ini = new IniUtil(AppDomain.CurrentDomain.BaseDirectory + "/Configs/config.ini");

            string host = ini.Read<string>("Database", "Host");
            Assert.AreEqual("localhost", host);

            var dbConfig = ini.ReadSection<DatabaseConfig>("Database");
            Assert.IsNotNull(host);
        }
    }
}
