namespace CZJ.Extension
{
    public static class DirectoryHelper
    {
        public static void CopyDirectory(string sourceDir, string targetDir, string[] filterFiles = null, bool overWrite = true)
        {
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            // 获取源目录中的所有文件，并复制到目标目录
            string[] files = Directory.GetFiles(sourceDir);
            if (filterFiles != null && filterFiles.Length > 0)
            {
                files = files.Where(w => filterFiles.Contains(Path.GetFileName(w))).ToArray();
            }
            foreach (string file in files)
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                if (overWrite || !File.Exists(destFile))
                    File.Copy(file, destFile, overWrite); // true 表示如果目标文件已存在，则覆盖它
            }
            // 获取源目录中的所有子目录，并递归复制
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string destDirectory = Path.Combine(targetDir, Path.GetFileName(directory));
                Directory.CreateDirectory(destDirectory);
                CopyDirectory(directory, destDirectory, filterFiles, overWrite);
            }
        }

        /// <summary>
        /// 删除目录下文件的方法
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteFilesWithDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
            Directory.CreateDirectory(directoryPath);
        }
    }
}
