namespace CZJ.Extension
{
    public static class ZipUtil
    {
        public static void CreateZip(string sourceDirectory, string destinationZipFile)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new Exception($"Source directory does not exist or could not be found: {sourceDirectory}");
            }
            ZipFile.CreateFromDirectory(sourceDirectory, destinationZipFile); //创建ZIP文件
        }

        public static void ExtractZipFile(string zipFilePath, string extractFolderPath)
        {
            if (!Directory.Exists(zipFilePath))
                Directory.CreateDirectory(extractFolderPath);

            // 打开ZIP文件
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // 构建完整的文件路径
                    string completeFilePath = Path.Combine(extractFolderPath, entry.FullName);

                    // 创建目录
                    string directoryPath = Path.GetDirectoryName(completeFilePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // 如果是文件，则解压
                    if (!entry.FullName.EndsWith("/"))
                    {
                        // 写入文件
                        using (Stream entryStream = entry.Open())
                        {
                            using (FileStream fileStream = File.Create(completeFilePath))
                            {
                                entryStream.CopyTo(fileStream);
                            }
                        }
                    }
                }
            }
        }
    }
}
