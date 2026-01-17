using CZJ.ExcelExtension;

namespace CZJ.ExcelExtensionTest
{
    [TestClass]
    public sealed class EPPlusTest
    {
        private string FilePath = Path.Combine(Common.ApplicationBaseDirectory, "EPPlus.xlsx");

        [TestMethod]
        public void SaveExcelTest()
        {
            var list = new List<Person>
            {
                new Person { Id = 1, Name = "张三", Age = 20 },
                new Person { Id = 2, Name = "李四", Age = 25 }
            };
            EPPlusUtil.SaveAs(FilePath, list);
            Assert.IsTrue(File.Exists(FilePath));
        }

        [TestMethod]
        public void ExcelAppendTest()
        {
            var newList = new List<Person>
            {
                new Person { Id = 3, Name = "王五", Age = 30 },
                new Person { Id = 4, Name = "赵六", Age = 35 }
            };
            EPPlusUtil.Append(FilePath, newList);
        }

        [TestMethod]
        public void LoadExcelTest()
        {
            var all = EPPlusUtil.Read<Person>(FilePath);
            var dictAll = EPPlusUtil.ReadAsDictionary(FilePath);
        }
    }
}
