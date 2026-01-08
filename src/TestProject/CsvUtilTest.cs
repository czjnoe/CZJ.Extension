using CZJ.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class CsvUtilTest
    {
        [TestMethod]
        public void WriteTest()
        {
            var csvHelper = new CsvUtil();

            // 写入对象列表
            var people = new List<Person>
            {
                new Person { Name = "张三", Age = 25, Email = "zhangsan@example.com" },
                new Person { Name = "李四", Age = 30, Email = "lisi@example.com" }
            };
            csvHelper.Write("people.csv", people);
            Assert.IsTrue(File.Exists("people.csv"));
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
