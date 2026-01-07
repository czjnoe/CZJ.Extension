using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class RetryTest
    {
        [TestMethod]
        public void Test()
        {
            RetryHelper.New
             .MaxAttempts(3)
             .DelayMilliseconds(1000 * 5).Execute((index) =>
             {
                 Console.WriteLine($"第{index}次重试");
                 throw new Exception("测试异常");
             });
        }
    }
}
