namespace CZJ.Extension
{
    public static class ProcessUtil
    {
        /// <summary>
        /// 打开 exe 程序
        /// </summary>
        /// <param name="exePath">exe 文件的完整路径</param>
        /// <param name="arguments">启动参数（可选）</param>
        /// <returns>启动的进程对象，失败返回 null</returns>
        public static Process StartProcess(string exePath, string arguments = "")
        {
            try
            {
                if (!File.Exists(exePath))
                {
                    throw new FileNotFoundException($"未找到文件: {exePath}");
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = true
                };

                Process process = Process.Start(startInfo);
                return process;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启动进程失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 判断指定路径的程序是否正在运行
        /// </summary>
        /// <param name="exePath">exe 文件的完整路径</param>
        /// <returns>如果进程正在运行返回 true，否则返回 false</returns>
        public static bool IsProcessRunningByPath(string exePath)
        {
            try
            {
                string fullPath = Path.GetFullPath(exePath);

                Process[] processes = Process.GetProcesses();
                return processes.Any(p =>
                {
                    try
                    {
                        return string.Equals(
                            p.MainModule?.FileName,
                            fullPath,
                            StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        // 某些系统进程可能无法访问
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查进程路径失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 如果进程未运行则启动，如果已运行则返回现有进程
        /// </summary>
        /// <param name="exePath">exe 文件路径</param>
        /// <param name="arguments">启动参数</param>
        /// <returns>进程对象</returns>
        public static Process StartOrGetExisting(string exePath, string arguments = "")
        {
            string processName = Path.GetFileNameWithoutExtension(exePath);

            if (IsProcessExists(processName))
            {
                var existingProcesses = GetProcesses(processName);
                return existingProcesses.FirstOrDefault();
            }
            else
            {
                return StartProcess(exePath, arguments);
            }
        }

        /// <summary>
        /// 通过进程名称获取进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <returns>进程</returns>
        public static Process GetProcessByName(string processName)
        {
            // 获取当前运行的所有进程
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                return processes[0];
            }
            return null;
        }

        /// <summary>
        /// 暂停进程
        /// </summary>
        /// <param name="process">进程</param>
        public static void SuspendProcess(Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
                CloseHandle(pOpenThread);
            }
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="process">进程</param>
        public static void ResumeProcess(Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
                CloseHandle(pOpenThread);
            }
        }

        /// <summary>
        /// 判断进程是否存在
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <returns>是否存在</returns>
        public static bool IsProcessExists(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        /// <summary>
        /// 解除文件或目录占用
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void Unlock(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DirectoryNotFoundException("No file or directory path was provided.");
            }
            var processes = GetProcesses(path);
            if (processes == null || processes.Length == 0)
            {
                return;
            }
            KillProcesses(processes);
        }

        /// <summary>
        /// 解除进程锁定
        /// </summary>
        /// <param name="processes"></param>
        public static void Unlock(Process[] processes)
        {
            if (processes == null || processes.Length == 0)
            {
                return;
            }
            KillProcesses(processes);
        }

        /// <summary>
        /// 获取文件或目录对应的占用进程
        /// 没有文件被占用时会返回空数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Process[] GetProcesses(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new DirectoryNotFoundException("No file or directory path was provided.");
            }

            var processes = Directory.Exists(filePath)
                ? GetProcessesFromDirectoryPath(filePath)
                : GetProcessesFromFilePath(filePath);
            return processes;
        }

        /// <summary>
        /// 获取目录对应的占用进程
        /// 没有文件被占用时会返回空数组
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static Process[] GetProcessesFromDirectoryPath(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

            var processesById = new Dictionary<int, Process>();

            foreach (string path in filePaths)
            {
                foreach (var process in FileHandleManager.GetProcesses(path))
                {
                    if (!processesById.ContainsKey(process.Id))
                    {
                        processesById[process.Id] = process;
                    }
                }
            }

            return new List<Process>(processesById.Values).ToArray();
        }

        /// <summary>
        /// 获取文件对应的占用进程
        /// 没有文件被占用时会返回空数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Process[] GetProcessesFromFilePath(string filePath)
        {
            var processesById = new Dictionary<int, Process>();

            foreach (var process in FileHandleManager.GetProcesses(filePath))
            {
                if (!processesById.ContainsKey(process.Id))
                {
                    processesById[process.Id] = process;
                }
            }

            return new List<Process>(processesById.Values).ToArray();
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processes"></param>
        public static void KillProcesses(Process[] processes)
        {
            foreach (Process process in processes)
            {
                try
                {
                    if (process.HasExited)
                    {
                        continue;
                    }

                    process.Kill(entireProcessTree: true);

                    process.WaitForExit(2000);
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is System.ComponentModel.Win32Exception)
                {
                }
                finally
                {
                    process.Dispose();
                }
            }
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="process">进程</param>
        public static void KillProcess(Process process)
        {
            process.Kill();
        }

        /// <summary>
        /// 关闭进程并等待结束
        /// </summary>
        /// <param name="process">进程</param>
        public static void KillProcessAndWait(Process process)
        {
            process.Kill();
            process.WaitForExit();
        }


        #region 打开文件夹或文件所在位置并置顶

        /// <summary>
        /// 打开文件夹并置顶
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Task OpenFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new Exception("文件夹路径不能为空！");
            }
            if (!Directory.Exists(folderPath))
            {
                throw new Exception("指定的文件夹不存在！");
            }
            return Task.Run(() =>
            {
                Process process = Process.Start("explorer.exe", folderPath);

                // 等待资源管理器进程启动
                Thread.Sleep(500);

                // 查找文件夹窗口句柄
                IntPtr hWnd = FindWindow(null, folderPath);

                // 如果找到窗口，尝试将其置顶
                if (hWnd != IntPtr.Zero)
                {
                    // 设置为最上层窗口
                    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

                    // 激活文件夹窗口
                    SetForegroundWindow(hWnd);
                    Thread.Sleep(500);  // 小的延迟，确保操作生效

                    // 查找 Chrome 浏览器窗口
                    IntPtr chromeWnd = FindWindow(null, "Chrome");
                    if (chromeWnd != IntPtr.Zero)
                    {
                        // 最小化 Chrome 浏览器窗口
                        ShowWindow(chromeWnd, SW_MINIMIZE);
                        Thread.Sleep(500);  // 等待一会，确保最小化

                        // 恢复 Chrome 浏览器窗口
                        ShowWindow(chromeWnd, SW_RESTORE);
                        Thread.Sleep(500);  // 等待一会，确保恢复

                        // 再将文件夹窗口置前
                        SetForegroundWindow(hWnd);
                    }
                }
            }).ContinueWith(p =>
            {
                if (p.IsFaulted)
                {
                    Console.WriteLine($"打开文件夹时出错: {p.Exception}");
                    throw new Exception($"打开文件夹时出错: {p.Exception}");
                }
            });
        }

        /// <summary>
        /// 打开文件所在位置并选中文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static Task OpenFileLocationAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("文件路径不能为空！");
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("指定的文件不存在！", filePath);
            }

            return Task.Run(() =>
            {
                // 获取文件所在目录
                string directory = Path.GetDirectoryName(filePath);

                // 使用 explorer 的 select 参数定位文件
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{filePath}\""
                });

            }).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    throw new Exception($"定位文件位置失败: {t.Exception.Message}");
                }
            });
        }

        // 引入必要的 Windows API
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // HWND_TOPMOST 用于将窗口置于最上层
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        // 显示窗口的常量
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        #endregion


        /// <summary>
        /// 使用浏览器打开网址
        /// </summary>
        /// <param name="link"></param>
        public static void OpenBrowserForVisitSite(string link)
        {
            var param = new ProcessStartInfo { FileName = link, UseShellExecute = true, Verb = "open" };
            Process.Start(param);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern IntPtr CloseHandle(IntPtr hObject);

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
    }

    public static class FileHandleManager
    {
        private const int RebootReasonNone = 0;
        private const int CCH_RM_MAX_APP_NAME = 255;
        private const int CCH_RM_MAX_SVC_NAME = 63;

        private enum RM_APP_TYPE
        {
            UnknownApp = 0,
            MainWindow = 1,
            OtherWindow = 2,
            Service = 3,
            Explorer = 4,
            Console = 5,
            Critical = 1000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RM_UNIQUE_PROCESS
        {
            public int ProcessId;
            public FILETIME ProcessStartTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;

            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;

            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmRegisterResources(uint pSessionHandle,
                                                      uint nFiles,
                                                      string[] rgsFilenames,
                                                      uint nApplications,
                                                      [In] RM_UNIQUE_PROCESS[] rgApplications,
                                                      uint nServices,
                                                      string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmGetList(uint dwSessionHandle,
                                            out uint pnProcInfoNeeded,
                                            ref uint pnProcInfo,
                                            [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                            ref uint lpdwRebootReasons);

        public static List<Process> GetProcesses(string path)
        {
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int resource = RmStartSession(out uint handle, 0, key);
            if (resource != 0)
            {
                throw new Exception("Could not begin restart session.  Unable to determine file locker.");
            }

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcessInfo = 0,
                     lpdwRebootReasons = RebootReasonNone;

                string[] resources = new string[] { path };

                resource = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                if (resource != 0) throw new Exception("Could not register resource.");

                resource = RmGetList(handle, out uint pnProcInfoNeeded, ref pnProcessInfo, null, ref lpdwRebootReasons);

                if (resource == ERROR_MORE_DATA)
                {
                    RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcessInfo = pnProcInfoNeeded;

                    resource = RmGetList(handle, out pnProcInfoNeeded, ref pnProcessInfo, processInfo, ref lpdwRebootReasons);
                    if (resource == 0)
                    {
                        processes = new List<Process>((int)pnProcessInfo);

                        for (int i = 0; i < pnProcessInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].Process.ProcessId));
                            }
                            catch (ArgumentException) { }
                        }
                    }
                    else throw new Exception("Could not list processes locking resource.");
                }
                else if (resource != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }
    }
}
