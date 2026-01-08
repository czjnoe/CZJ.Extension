using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class DynamicExtensionTest
    {
        [TestMethod]
        public void Test()
        {
            ExpandoObject obj = new ExpandoObject();
            obj.SetProperty("Name", "张三");
            obj.SetProperty("Age", 25);
            obj.SetProperty("Email", "zhangsan@example.com");

            // 读取属性
            string name = obj.GetProperty<string>("Name");
            Assert.AreEqual("张三", name);
            int age = obj.GetProperty<int>("Age");
            Assert.AreEqual(25, age);
            // 检查属性是否存在
            bool hasName = obj.HasProperty("Name");
            Assert.IsTrue(hasName);
        }
    }
}
