using CZJ.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Models;

namespace TestProject
{
    [TestClass]
    public sealed class JsonConfigTest
    {
        [TestMethod]
        public void JsonTest()
        {
            JsonConfigUtil appConfig = new JsonConfigUtil("Configs/appsettings.json");
            Assert.IsTrue(appConfig.Exists());
            // 读取整个文件
            var config = appConfig.Read<AppConfigOption>();

            // 获取指定节点值
            var target = appConfig.GetValue<string>("compilerOptions:target");
            var sourceMap = appConfig.GetValue<bool>("compilerOptions:sourceMap");


            // 更新指定节点值
            appConfig.UpdateValue("compilerOptions:removeComments", "true");

            // 写入整个对象
            appConfig.Write(config);
        }
    }
}
