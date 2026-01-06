using CZJ.Extension;

namespace TestProject
{
    [TestClass]
    public sealed class ProcessTest
    {
        [TestMethod]
        public void GetProcessesTest()
        {
            var processes = ProcessHelper.GetProcesses(
                @"C:\Program Files\PEER Group\PTO 8.6 SP2\Code\ConsoleApp1");
        }

        [TestMethod]
        public void UnlockTest()
        {
            ProcessHelper.Unlock(@"C:\Program Files\PEER Group\PTO 8.6 SP2\Code\ConsoleApp1");
        }
    }
}
