using CZJ.ExcelExtension;
using System.Data;
using System.Dynamic;

namespace CZJ.ExcelExtensionTest
{
    [TestClass]
    public sealed class NpoiTest
    {
        private string FilePath = Path.Combine(Common.ApplicationBaseDirectory, "Npoi.xlsx");

        [TestMethod]
        public void SaveExcelTest()
        {
            var list = new List<Person>
            {
                new Person { Id = 1, Name = "张三", Age = 20 },
                new Person { Id = 2, Name = "李四", Age = 25 }
            };

            NpoiExcelUtil.SaveAs(FilePath, list);
            NpoiExcelUtil.Append(FilePath, new List<Person> { new Person { Name = "Cindy", Age = 28 } });

            // 写入 ExpandoObject
            var expList = new List<ExpandoObject>();
            dynamic obj1 = new ExpandoObject();
            obj1.Name = "Jack"; obj1.Age = 35;
            dynamic obj2 = new ExpandoObject();
            obj2.Name = "Mary"; obj2.Age = 22;
            expList.Add(obj1); expList.Add(obj2);

            NpoiExcelUtil.SaveAs(FilePath, expList);
            NpoiExcelUtil.Append(FilePath, expList);

            // 写入 DataTable
            var dt = new DataTable();
            dt.Columns.Add("Product");
            dt.Columns.Add("Price");
            dt.Rows.Add("Apple", "10");
            dt.Rows.Add("Banana", "5");
            NpoiExcelUtil.SaveAs("dt.xlsx", dt);
            NpoiExcelUtil.Append("dt.xlsx", dt);

            // 流写入
            using var ms = NpoiExcelUtil.SaveToStream(list);
            File.WriteAllBytes("stream.xlsx", ms.ToArray());

            // 流追加
            using var ms2 = new MemoryStream(File.ReadAllBytes("stream.xlsx"));
            var newList = new List<Person> { new Person { Name = "David", Age = 40 } };
            using var ms3 = NpoiExcelUtil.AppendToStream(ms2, newList);
            File.WriteAllBytes("stream_append.xlsx", ms3.ToArray());

            // 读取
            var dtRead = NpoiExcelUtil.ReadToDataTable("dt.xlsx");
            var listRead = NpoiExcelUtil.ReadToList<Person>("test.xlsx");
        }
    }
}
