namespace CZJ.Extension
{
    public static class FileHelper
    {
        public static byte[] FileToByte(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new byte[0];
            }
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];
            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return buff;
        }

        public static void ByteToFile(byte[] fileBytes, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(fileBytes, 0, fileBytes.Length);
            bw.Close();
            fs.Close();
        }

        public static void ByteToFile2(byte[] fileBytes, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            System.IO.File.WriteAllBytes(filePath, fileBytes);//将字节写入文件
        }

        /// <summary>
        /// 创建并且检索文件夹结构
        /// </summary>
        /// <param name="rootPath">根目录</param>
        /// <param name="folderPaths">文件目录</param>
        /// <returns></returns>
        public static ConcurrentDictionary<string, string> CreateAndRetrieveFolderStructure(string rootPath, string[] folderPaths)
        {
            var folderStructures = new ConcurrentDictionary<string, string>();
            foreach (var path in folderPaths)
            {
                folderStructures[path] = Path.Combine(rootPath, path.Replace("_", "\\"));
            }
            folderStructures["RootPath"] = rootPath;

            Parallel.ForEach(folderStructures, folderDic =>
            {
                if (!Directory.Exists(folderDic.Value))
                {
                    Directory.CreateDirectory(folderDic.Value);
                }

            });

            return folderStructures;
        }

        /// <summary>
        /// 判断文件是否被其他程序占用打开
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件被占用返回true，否则返回false</returns>
        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath))
                return false;
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        #region 文件操作

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 创建文件（如果目录不存在则自动创建）
        /// </summary>
        public static void CreateFile(string filePath, string content = "")
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        public static string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 写入文件内容（覆盖）
        /// </summary>
        public static void WriteFile(string filePath, string content)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// 追加内容到文件
        /// </summary>
        public static void AppendFile(string filePath, string content)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.AppendAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        public static void CopyFile(string sourceFile, string destFile, bool overwrite = true)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException($"源文件不存在: {sourceFile}");

            string directory = Path.GetDirectoryName(destFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(sourceFile, destFile, overwrite);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveFile(string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException($"源文件不存在: {sourceFile}");

            string directory = Path.GetDirectoryName(destFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Move(sourceFile, destFile);
        }

        /// <summary>
        /// 获取文件大小（字节）
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        public static string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// 获取文件名（不含扩展名）
        /// </summary>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 获取文件名（含扩展名）
        /// </summary>
        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static string Read(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
            }

            return File.Exists(path)
                ? File.ReadAllText(path)
                : throw new FileNotFoundException($"The file at path '{path}' does not exist", path);
        }

        public static Dictionary<string, string> ReadAsDictionary(string path, char separator)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file at path '{path}' does not exist", path);
            }

            var lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                throw new InvalidOperationException("File must have at least a header and one data line");
            }

            var keys = lines[0].Split(separator);
            var dict = new Dictionary<string, string>();

            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(separator);

                for (var j = 0; j < Math.Min(keys.Length, values.Length); j++)
                {
                    dict[keys[j]] = values[j];
                }
            }

            return dict;
        }

        public static string[] ReadLines(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
            }

            return File.Exists(path)
                ? File.ReadAllLines(path)
                : throw new FileNotFoundException($"The file at path '{path}' does not exist", path);
        }

        public static T? ReadAndDeserialize<T>(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file at path '{path}' does not exist", path);
            }

            var content = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(content);
        }

        #endregion

        #region 文件夹操作

        /// <summary>
        /// 检查文件夹是否存在
        /// </summary>
        public static bool DirectoryExists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static bool DeleteDirectory(string dirPath, bool recursive = true)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, recursive);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 清空文件夹内容（保留文件夹本身）
        /// </summary>
        public static void ClearDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return;

            DirectoryInfo dir = new DirectoryInfo(dirPath);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                subDir.Delete(true);
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        public static void CopyDirectory(string sourceDir, string destDir, bool overwrite = true)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"源文件夹不存在: {sourceDir}");

            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string destFile = Path.Combine(destDir, file.Name);
                file.CopyTo(destFile, overwrite);
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string destSubDir = Path.Combine(destDir, subDir.Name);
                CopyDirectory(subDir.FullName, destSubDir, overwrite);
            }
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        public static void MoveDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"源文件夹不存在: {sourceDir}");

            Directory.Move(sourceDir, destDir);
        }

        /// <summary>
        /// 获取文件夹大小（字节）
        /// </summary>
        public static long GetDirectorySize(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException($"文件夹不存在: {dirPath}");

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            long size = 0;

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }

            return size;
        }

        /// <summary>
        /// 获取文件夹中的所有文件
        /// </summary>
        public static List<string> GetFiles(string dirPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException($"文件夹不存在: {dirPath}");

            return Directory.GetFiles(dirPath, searchPattern, searchOption).ToList();
        }

        /// <summary>
        /// 获取文件夹中的所有子文件夹
        /// </summary>
        public static List<string> GetDirectories(string dirPath, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException($"文件夹不存在: {dirPath}");

            return Directory.GetDirectories(dirPath, searchPattern, searchOption).ToList();
        }

        /// <summary>
        /// 统计文件夹中的文件数量
        /// </summary>
        public static int GetFileCount(string dirPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(dirPath))
                return 0;

            return Directory.GetFiles(dirPath, searchPattern, searchOption).Length;
        }

        /// <summary>
        /// 统计文件夹中的子文件夹数量
        /// </summary>
        public static int GetDirectoryCount(string dirPath, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(dirPath))
                return 0;

            return Directory.GetDirectories(dirPath, "*", searchOption).Length;
        }

        #endregion

        #region 路径操作

        /// <summary>
        /// 合并路径
        /// </summary>
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// 获取目录路径
        /// </summary>
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 获取临时文件路径
        /// </summary>
        public static string GetTempFilePath()
        {
            return Path.GetTempFileName();
        }

        /// <summary>
        /// 获取临时文件夹路径
        /// </summary>
        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量删除文件
        /// </summary>
        public static int BatchDeleteFiles(List<string> filePaths)
        {
            int count = 0;
            foreach (var file in filePaths)
            {
                if (DeleteFile(file))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 批量重命名文件
        /// </summary>
        public static void BatchRenameFiles(string dirPath, string prefix = "", string suffix = "")
        {
            if (!Directory.Exists(dirPath))
                return;

            var files = Directory.GetFiles(dirPath);
            int index = 1;

            foreach (var file in files)
            {
                string dir = Path.GetDirectoryName(file);
                string ext = Path.GetExtension(file);
                string newName = $"{prefix}{index}{suffix}{ext}";
                string newPath = Path.Combine(dir, newName);

                File.Move(file, newPath);
                index++;
            }
        }

        #endregion

        /// <summary>
        /// 获取当前目录路径
        /// </summary>
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        #region ReadToBytes

        /// <summary>
        /// 读取流转换成字节数组
        /// </summary>
        /// <param name="stream">流</param>
        public static byte[] ReadToBytes(Stream stream)
        {
            if (stream == null)
                return null;
            if (stream.CanRead == false)
                return null;
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            return buffer;
        }

        /// <summary>
        /// 将文件读取到字节流中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static byte[] ReadToBytes(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            var fileInfo = new FileInfo(filePath);
            using var reader = new BinaryReader(fileInfo.Open(FileMode.Open));
            return reader.ReadBytes((int)fileInfo.Length);
        }

        #endregion

        #region ReadToStream

        /// <summary>
        /// 读取文件流
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public static Stream ReadToStream(string filePath)
        {
            try
            {
                return new FileStream(filePath, FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region ReadToString

        /// <summary>
        /// 读取文件到字符串
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public static string ReadToString(string filePath)
        {
            return ReadToString(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件到字符串
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static string ReadToString(string filePath, Encoding encoding)
        {
            if (System.IO.File.Exists(filePath) == false)
                return string.Empty;
            using var reader = new StreamReader(filePath, encoding);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 读取流转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="isCloseStream">读取完成是否释放流，默认为true</param>
        public static string ReadToString(Stream stream, Encoding encoding = null, int bufferSize = 1024 * 2, bool isCloseStream = true)
        {
            if (stream == null)
                return string.Empty;
            encoding ??= Encoding.UTF8;
            if (stream.CanRead == false)
                return string.Empty;
            using var reader = new StreamReader(stream, encoding, true, bufferSize, !isCloseStream);
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            var result = reader.ReadToEnd();
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            return result;
        }

        #endregion

        #region ReadToStringAsync

        /// <summary>
        /// 读取文件到字符串
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public static async Task<string> ReadToStringAsync(string filePath)
        {
            return await ReadToStringAsync(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件到字符串
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static async Task<string> ReadToStringAsync(string filePath, Encoding encoding)
        {
            if (System.IO.File.Exists(filePath) == false)
                return string.Empty;
            using var reader = new StreamReader(filePath, encoding);
            return await reader.ReadToEndAsync();
        }

        #endregion

        #region ToStream

        /// <summary>
        /// 字符串转换成流
        /// </summary>
        /// <param name="data">数据</param>
        public static Stream ToStream(string data)
        {
            return ToStream(data, Encoding.UTF8);
        }

        /// <summary>
        /// 字符串转换成流
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoding">字符编码</param>
        public static Stream ToStream(string data, Encoding encoding)
        {
            if (data.IsEmpty())
                return Stream.Null;
            return new MemoryStream(ToBytes(data, encoding));
        }

        #endregion

        #region ToBytes

        /// <summary>
        /// 流转换为字节数组
        /// </summary>
        /// <param name="stream">流</param>
        public static byte[] ToBytes(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="data">数据,默认字符编码utf-8</param>        
        public static byte[] ToBytes(string data)
        {
            return ToBytes(data, Encoding.UTF8);
        }

        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] ToBytes(string data, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(data))
                return Array.Empty<byte>();
            return encoding.GetBytes(data);
        }

        #endregion

        #region ToBytesAsync

        /// <summary>
        /// 流转换为字节数组
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task<byte[]> ToBytesAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return buffer;
        }

        #endregion
    }
}
