using CZJ.Extension;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class FileWatcherTest
    {
        [TestMethod]
        public void FileWatcherSample()
        {
            string path = "";
            var watcher = new FileWatcher();
            watcher.Path(path)
           .Filter("*.cshtml")
           .OnChangedAsync(async (_, e) =>
           {
               Debug.WriteLine($"文件已更改: {e.FullPath}");
           })
           .OnRenamedAsync(async (_, e) =>
           {
               Debug.WriteLine($"文件重命名: {e.FullPath}");
           })
           .Start();
        }

    }
}
