namespace CZJ.ExcelExtension
{
    public static class EPPlusUtil
    {
        static EPPlusUtil()
        {
            ExcelPackage.License.SetNonCommercialPersonal("EPPlus");
        }

        #region 写入

        /// <summary>
        /// 全量写入 Excel（覆盖文件）
        /// </summary>
        public static void SaveAs<T>(string filePath, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add(sheetName);
            sheet.Cells["A1"].LoadFromCollection(data, true);

            package.SaveAs(new FileInfo(filePath));
        }

        /// <summary>
        /// 增量写入（追加到尾部）
        /// </summary>
        public static void Append<T>(string filePath, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            var file = new FileInfo(filePath);

            using var package = file.Exists
                ? new ExcelPackage(file)
                : new ExcelPackage();

            var sheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName)
                        ?? package.Workbook.Worksheets.Add(sheetName);

            int lastRow = sheet.Dimension?.End.Row ?? 0;

            if (lastRow == 0)
            {
                sheet.Cells["A1"].LoadFromCollection(data, true);
            }
            else
            {
                sheet.Cells[lastRow + 1, 1].LoadFromCollection(data, false);
            }

            package.SaveAs(file);
        }

        /// <summary>
        /// 追加新 Sheet
        /// </summary>
        public static void AppendSheet<T>(string filePath, string sheetName, IEnumerable<T> data)
        {
            var file = new FileInfo(filePath);

            using var package = file.Exists
                ? new ExcelPackage(file)
                : new ExcelPackage();

            if (package.Workbook.Worksheets.Any(x => x.Name == sheetName))
                throw new Exception($"Sheet {sheetName} 已存在");

            var sheet = package.Workbook.Worksheets.Add(sheetName);
            sheet.Cells["A1"].LoadFromCollection(data, true);

            package.SaveAs(file);
        }

        /// <summary>
        /// 写入 Stream
        /// </summary>
        public static void SaveAsStream<T>(Stream stream, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets.Add(sheetName);
            sheet.Cells["A1"].LoadFromCollection(data, true);

            package.Save();
        }

        public static void AddSheet(string filePath, DataTable table, string sheetName)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var sheet = package.Workbook.Worksheets.Add(sheetName);

            sheet.Cells["A1"].LoadFromDataTable(table, true);

            AutoFit(sheet);

            package.Save();
        }

        public static void AppendRows(string filePath, DataTable table, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var sheet = package.Workbook.Worksheets[sheetName]
                        ?? package.Workbook.Worksheets.Add(sheetName);

            int startRow = sheet.Dimension?.End.Row + 1 ?? 1;

            if (startRow == 1)
            {
                // 写表头
                sheet.Cells["A1"].LoadFromDataTable(table, true);
            }
            else
            {
                sheet.Cells[startRow, 1].LoadFromDataTable(table, false);
            }

            AutoFit(sheet);

            package.Save();
        }

        public static void AppendDynamicRows(string filePath, List<ExpandoObject> data, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var sheet = package.Workbook.Worksheets[sheetName]
                        ?? package.Workbook.Worksheets.Add(sheetName);

            int startRow = sheet.Dimension?.End.Row + 1 ?? 1;

            var dicts = data.Select(d => (IDictionary<string, object>)d).ToList();

            if (startRow == 1)
            {
                // 写表头
                var headers = dicts.First().Keys.ToList();
                for (int i = 0; i < headers.Count; i++)
                {
                    sheet.Cells[1, i + 1].Value = headers[i];
                }
                startRow = 2;
            }

            foreach (var item in dicts)
            {
                int col = 1;
                foreach (var val in item.Values)
                {
                    sheet.Cells[startRow, col++].Value = val;
                }
                startRow++;
            }

            AutoFit(sheet);

            package.Save();
        }

        public static byte[] ExportDataTable(DataTable table, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            sheet.Cells["A1"].LoadFromDataTable(table, true);

            AutoFit(sheet);

            return package.GetAsByteArray();
        }

        public static byte[] ExportDataTables(Dictionary<string, DataTable> tables)
        {
            using var package = new ExcelPackage();

            foreach (var kv in tables)
            {
                var sheet = package.Workbook.Worksheets.Add(kv.Key);
                sheet.Cells["A1"].LoadFromDataTable(kv.Value, true);

                AutoFit(sheet);
            }

            return package.GetAsByteArray();
        }

        public static byte[] ExportList<T>(IEnumerable<T> list, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            sheet.Cells["A1"].LoadFromCollection(list, true);

            AutoFit(sheet);

            return package.GetAsByteArray();
        }

        public static byte[] SaveStreamToExcel(Stream stream, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add(sheetName);

            using var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            sheet.Cells["A1"].Value = content;

            return package.GetAsByteArray();
        }

        public static byte[] AppendStreamData(byte[] excelBytes, Stream stream, string sheetName = "Sheet1")
        {
            using var ms = new MemoryStream(excelBytes);
            using var package = new ExcelPackage(ms);

            var sheet = package.Workbook.Worksheets[sheetName]
                        ?? package.Workbook.Worksheets.Add(sheetName);

            int row = sheet.Dimension?.End.Row + 1 ?? 1;

            using var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            sheet.Cells[row, 1].Value = content;

            return package.GetAsByteArray();
        }

        public static void SaveStreamToFile(this MemoryStream stream, string filePath)
        {
            stream.Position = 0;

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fs);
        }

        public static void CreateEmpty<T>(string filePath, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add(sheetName);
            sheet.Cells["A1"].LoadFromCollection(new List<T>(), true);

            package.SaveAs(new FileInfo(filePath));
        }

        /// <summary>
        /// 追加多行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="count"></param>
        public static void AddEmptyRows(this ExcelWorksheet sheet, int count)
        {
            int startRow = sheet.Dimension?.End.Row + 1 ?? 1;

            for (int i = 0; i < count; i++)
            {
                sheet.Cells[startRow + i, 1].Value = string.Empty;
            }
        }

        /// <summary>
        /// 追加一行
        /// </summary>
        /// <param name="sheet"></param>
        public static void AddEmptyRow(this ExcelWorksheet sheet)
        {
            sheet.AddEmptyRows(1);
        }

        /// <summary>
        /// 插入中间空行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="count"></param>
        public static void InsertEmptyRows(this ExcelWorksheet sheet, int rowIndex, int count)
        {
            sheet.InsertRow(rowIndex, count);
        }

        /// <summary>
        /// 在指定行号位置插入空行
        /// </summary>
        public static void InsertEmptyRows(string filePath, int rowIndex, int count, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var sheet = package.Workbook.Worksheets[sheetName];

            if (sheet == null)
                throw new Exception($"sheet {sheetName} 不存在");

            sheet.InsertRow(rowIndex, count);

            package.Save();
        }


        #endregion

        #region 读取

        /// <summary>
        /// 读取为实体
        /// </summary>
        public static List<T> Read<T>(string filePath, string sheetName = null) where T : new()
        {
            var file = new FileInfo(filePath);

            if (!file.Exists)
                throw new FileNotFoundException(filePath);

            using var package = new ExcelPackage(file);

            var sheet = string.IsNullOrEmpty(sheetName)
                ? package.Workbook.Worksheets.First()
                : package.Workbook.Worksheets[sheetName];

            return ReadSheetToList<T>(sheet);
        }

        /// <summary>
        /// 读取为 Dictionary
        /// </summary>
        public static List<Dictionary<string, object>> ReadAsDictionary(string filePath, string sheetName = null)
        {
            var file = new FileInfo(filePath);

            using var package = new ExcelPackage(file);

            var sheet = string.IsNullOrEmpty(sheetName)
                ? package.Workbook.Worksheets.First()
                : package.Workbook.Worksheets[sheetName];

            return ReadSheetToDictionary(sheet);
        }

        /// <summary>
        /// 从 Stream 读取实体
        /// </summary>
        public static List<T> ReadFromStream<T>(Stream stream, string sheetName = null) where T : new()
        {
            using var package = new ExcelPackage(stream);

            var sheet = string.IsNullOrEmpty(sheetName)
                ? package.Workbook.Worksheets.First()
                : package.Workbook.Worksheets[sheetName];

            return ReadSheetToList<T>(sheet);
        }

        public static List<Dictionary<string, object>> QueryByColumn(string filePath, string columnName, object value, string sheetName = null)
        {
            var data = ReadAsDictionary(filePath, sheetName);

            return data
                .Where(d => d.ContainsKey(columnName) &&
                            d[columnName]?.ToString() == value?.ToString())
                .ToList();
        }

        public static DataTable ReadToDataTable(string filePath, string sheetName = null)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var sheet = sheetName == null
                ? package.Workbook.Worksheets.First()
                : package.Workbook.Worksheets[sheetName];

            var table = new DataTable();

            int colCount = sheet.Dimension.End.Column;
            int rowCount = sheet.Dimension.End.Row;

            for (int col = 1; col <= colCount; col++)
            {
                table.Columns.Add(sheet.Cells[1, col].Text);
            }

            for (int row = 2; row <= rowCount; row++)
            {
                var dr = table.NewRow();
                for (int col = 1; col <= colCount; col++)
                {
                    dr[col - 1] = sheet.Cells[row, col].Text;
                }
                table.Rows.Add(dr);
            }

            return table;
        }

        public static byte[] ExportDynamicList(List<ExpandoObject> list, string sheetName = "Sheet1")
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            if (!list.Any())
                return package.GetAsByteArray();

            var dicts = list.Select(d => (IDictionary<string, object>)d).ToList();

            var headers = dicts.First().Keys.ToList();

            for (int i = 0; i < headers.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
            }

            int row = 2;
            foreach (var item in dicts)
            {
                int col = 1;
                foreach (var val in item.Values)
                {
                    sheet.Cells[row, col++].Value = val;
                }
                row++;
            }

            AutoFit(sheet);

            return package.GetAsByteArray();
        }

        public static byte[] ExtractSheet(string filePath, string sheetName)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            var source = package.Workbook.Worksheets[sheetName];
            if (source == null)
                throw new Exception($"sheet {sheetName} 不存在");

            using var newPkg = new ExcelPackage();

            newPkg.Workbook.Worksheets.Add(sheetName, source);

            return newPkg.GetAsByteArray();
        }

        public static DataTable ToDataTable(this ExcelWorksheet sheet)
        {
            var dt = new DataTable(sheet.Name);

            if (sheet.Dimension == null)
                return dt;

            int cols = sheet.Dimension.End.Column;
            int rows = sheet.Dimension.End.Row;

            for (int col = 1; col <= cols; col++)
            {
                dt.Columns.Add(sheet.Cells[1, col].Text);
            }

            for (int row = 2; row <= rows; row++)
            {
                var dr = dt.NewRow();

                for (int col = 1; col <= cols; col++)
                {
                    dr[col - 1] = sheet.Cells[row, col].Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static List<T> ReadSheetToList<T>(ExcelWorksheet sheet) where T : new()
        {
            var result = new List<T>();

            if (sheet.Dimension == null)
                return result;

            var headers = new List<string>();

            for (int col = 1; col <= sheet.Dimension.End.Column; col++)
            {
                headers.Add(sheet.Cells[1, col].Text);
            }

            for (int row = 2; row <= sheet.Dimension.End.Row; row++)
            {
                var obj = new T();

                for (int col = 1; col <= headers.Count; col++)
                {
                    var prop = typeof(T).GetProperty(headers[col - 1]);
                    if (prop != null)
                    {
                        var value = sheet.Cells[row, col].Value;
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                    }
                }

                result.Add(obj);
            }

            return result;
        }

        public static List<Dictionary<string, object>> ReadSheetToDictionary(ExcelWorksheet sheet)
        {
            var list = new List<Dictionary<string, object>>();

            if (sheet.Dimension == null)
                return list;

            var headers = new List<string>();

            for (int col = 1; col <= sheet.Dimension.End.Column; col++)
            {
                headers.Add(sheet.Cells[1, col].Text);
            }

            for (int row = 2; row <= sheet.Dimension.End.Row; row++)
            {
                var dict = new Dictionary<string, object>();

                for (int col = 1; col <= headers.Count; col++)
                {
                    dict[headers[col - 1]] = sheet.Cells[row, col].Value;
                }

                list.Add(dict);
            }

            return list;
        }

        public static List<string> GetSheetNames(string filePath)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));

            return package.Workbook.Worksheets
                .Select(s => s.Name)
                .ToList();
        }

        #endregion

        #region ExcelPackage

        /// <summary>
        /// 保存 ExcelPackage 到文件
        /// </summary>
        public static void SaveAsFile(this ExcelPackage package, string filePath)
        {
            var file = new FileInfo(filePath);
            package.SaveAs(file);
        }

        /// <summary>
        /// 获取或创建 Sheet
        /// </summary>
        public static ExcelWorksheet GetOrCreateSheet(this ExcelPackage package, string sheetName)
        {
            var sheet = package.Workbook.Worksheets.FirstOrDefault(s => s.Name == sheetName)
                        ?? package.Workbook.Worksheets.Add(sheetName);
            return sheet;
        }

        #endregion

        /// <summary>
        /// 自动列宽
        /// </summary>
        private static void AutoFitColumns(ExcelWorksheet sheet)
        {
            if (sheet.Dimension != null)
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        }

        /// <summary>
        /// 设置自动列宽
        /// </summary>
        /// <param name="sheet"></param>
        private static void AutoFit(ExcelWorksheet sheet)
        {
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        }
    }
}


