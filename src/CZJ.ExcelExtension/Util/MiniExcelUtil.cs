namespace CZJ.ExcelExtension
{
    public static class MiniExcelUtil
    {
        /// <summary>
        /// 全量写入 Excel（覆盖原文件）
        /// </summary>
        public static void SaveAs<T>(string filePath, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            MiniExcel.SaveAs(filePath, data, sheetName: sheetName, overwriteFile: true);
        }

        /// <summary>
        /// 增量写入 Excel（追加到文件尾部，不覆盖原文件）,如果文件不存在，会自动创建
        /// </summary>
        public static void Append<T>(string filePath, IEnumerable<T> newData, string sheetName = "Sheet1")
        {
            // 如果文件不存在，直接写入
            if (!File.Exists(filePath))
            {
                MiniExcel.SaveAs(filePath, newData, sheetName: sheetName, overwriteFile: true);
                return;
            }

            // 文件存在，直接追加
            MiniExcel.SaveAs(filePath, newData, sheetName: sheetName, overwriteFile: false);
        }

        /// <summary>
        /// 读取 Excel 为实体集合
        /// </summary>
        public static List<T> Read<T>(string filePath, string sheetName = null) where T : class, new()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            var data = string.IsNullOrEmpty(sheetName)
                ? MiniExcel.Query<T>(filePath)
                : MiniExcel.Query<T>(filePath, sheetName);

            return data.ToList();
        }

        /// <summary>
        /// 读取 Excel 为 Dictionary 集合(包含列头)
        /// </summary>
        public static List<Dictionary<string, object>> ReadAsDictionary(string filePath, string sheetName = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            var data = string.IsNullOrEmpty(sheetName)
                ? MiniExcel.Query(filePath)
                : MiniExcel.Query(filePath, sheetName: sheetName);

            return data.Cast<IDictionary<string, object>>()
                       .Select(d => d.ToDictionary(k => k.Key, v => v.Value))
                       .ToList();
        }

        /// <summary>
        /// 创建空表格，仅写入表头
        /// </summary>
        public static void CreateEmpty<T>(string filePath, string sheetName = "Sheet1")
        {
            var emptyList = new List<T>();
            MiniExcel.SaveAs(filePath, emptyList, sheetName: sheetName, overwriteFile: true);
        }

        /// <summary>
        /// 查询 Excel 中指定列包含某值的行
        /// </summary>
        public static List<Dictionary<string, object>> QueryByColumn(string filePath, string columnName, object value, string sheetName = null)
        {
            var data = ReadAsDictionary(filePath, sheetName);
            return data.Where(d => d.ContainsKey(columnName) && d[columnName]?.Equals(value) == true).ToList();
        }
    }
}
