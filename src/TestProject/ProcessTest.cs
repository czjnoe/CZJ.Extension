using CZJ.Extension;

namespace TestProject
{
    [TestClass]
    public sealed class ProcessTest
    {
        [TestMethod]
        public void GetProcessesTest()
        {
            var processes = ProcessUtil.GetProcesses(
                @"C:\Program Files\PEER Group\PTO 8.6 SP2\Code\ConsoleApp1");
        }

        [TestMethod]
        public void UnlockTest()
        {
            ProcessUtil.Unlock(@"C:\Program Files\PEER Group\PTO 8.6 SP2\Code\ConsoleApp1");
        }
    }
}
