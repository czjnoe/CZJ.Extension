using CZJ.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class AppConfigTest
    {
        [TestMethod]
        public void TestAppConfigSettings()
        {
            var appConfig = AppConfigHelper.OpenConfig(Path.Combine(Common.ApplicationBaseDirectory, "Configs/App.config"));
            var flag = appConfig.TryGet<bool>("Exist", out bool value);
            Assert.IsTrue(flag);
            flag = appConfig.TryGet("Language", out string language);
            Assert.IsTrue(flag);
        }
    }
}
