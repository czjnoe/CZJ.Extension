namespace CZJ.ExcelExtension
{
    public static class NpoiExcelUtil
    {
        #region 工厂方法

        private static IWorkbook CreateWorkbook(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath)) return new XSSFWorkbook(); // 默认xlsx
            var ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".xls" => new HSSFWorkbook(),
                ".xlsx" => new XSSFWorkbook(),
                _ => throw new NotSupportedException("只支持 .xls 或 .xlsx")
            };
        }

        private static IWorkbook LoadWorkbook(string filePath)
        {
            if (!File.Exists(filePath)) return CreateWorkbook(filePath);
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".xls" => new HSSFWorkbook(fs),
                ".xlsx" => new XSSFWorkbook(fs),
                _ => throw new NotSupportedException("只支持 .xls 或 .xlsx")
            };
        }

        private static IWorkbook LoadWorkbook(Stream stream, string extension = ".xlsx")
        {
            if (extension.ToLower() == ".xls") return new HSSFWorkbook(stream);
            return new XSSFWorkbook(stream);
        }

        #endregion

        #region 文件写入 / 流写入

        public static void SaveAs<T>(string filePath, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            var workbook = CreateWorkbook(filePath);
            var sheet = workbook.CreateSheet(sheetName);
            FillSheet(sheet, data, true);
            SaveWorkbook(workbook, filePath);
        }

        public static void Append<T>(string filePath, IEnumerable<T> data, string sheetName = "Sheet1")
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);

            int startRow = sheet.LastRowNum + 1;
            FillSheet(sheet, data, startRow == 0, startRow);

            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 保存到 MemoryStream
        /// </summary>
        public static MemoryStream SaveToStream<T>(IEnumerable<T> data, string sheetName = "Sheet1", bool includeHeader = true)
        {
            using var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);
            FillSheet(sheet, data, includeHeader);

            using var tempStream = new MemoryStream();
            workbook.Write(tempStream);

            var result = new MemoryStream(tempStream.ToArray());
            result.Position = 0;
            return result;
        }

        /// <summary>
        /// 追加流式数据到已有 Excel 流
        /// </summary>
        public static MemoryStream AppendToStream<T>(Stream excelStream, IEnumerable<T> data, string sheetName = "Sheet1", string extension = ".xlsx")
        {
            excelStream.Position = 0;
            var workbook = LoadWorkbook(excelStream, extension);
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
            int startRow = sheet.LastRowNum + 1;
            FillSheet(sheet, data, startRow == 0, startRow);

            using var tempStream = new MemoryStream();
            workbook.Write(tempStream);
            var ms = new MemoryStream(tempStream.ToArray());
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 保存 DataTable 到 Sheet
        /// </summary>
        public static void SaveAs(string filePath, DataTable table, string sheetName = "Sheet1")
        {
            var workbook = CreateWorkbook(filePath);
            var sheet = workbook.CreateSheet(sheetName);
            FillSheet(sheet, table, true);
            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 追加 DataTable
        /// </summary>
        public static void Append(string filePath, DataTable table, string sheetName = "Sheet1")
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);

            int startRow = sheet.LastRowNum + 1;
            if (startRow == 0) FillSheet(sheet, table, true);
            else FillSheet(sheet, table, false, startRow);

            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 写入 ExpandoObject 列表
        /// </summary>
        public static void SaveAs(string filePath, List<ExpandoObject> data, string sheetName = "Sheet1")
        {
            var workbook = CreateWorkbook(filePath);
            var sheet = workbook.CreateSheet(sheetName);
            FillSheet(sheet, data, true);
            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 追加 ExpandoObject 列表
        /// </summary>
        public static void Append(string filePath, List<ExpandoObject> data, string sheetName = "Sheet1")
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
            int startRow = sheet.LastRowNum + 1;
            FillSheet(sheet, data, startRow == 0, startRow);
            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 追加空行
        /// </summary>
        public static void AddEmptyRows(string filePath, int count, string sheetName = "Sheet1")
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);

            int startRow = sheet.LastRowNum + 1;
            for (int i = 0; i < count; i++)
            {
                sheet.CreateRow(startRow + i);
            }

            SaveWorkbook(workbook, filePath);
        }

        /// <summary>
        /// 插入空行
        /// </summary>
        public static void InsertEmptyRows(string filePath, int rowIndex, int count, string sheetName = "Sheet1")
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = workbook.GetSheet(sheetName) ?? throw new Exception($"Sheet {sheetName} 不存在");
            sheet.ShiftRows(rowIndex - 1, sheet.LastRowNum, count, true, false);
            SaveWorkbook(workbook, filePath);
        }

        #endregion

        #region 读取

        public static DataTable ReadToDataTable(string filePath, string sheetName = null)
        {
            var workbook = LoadWorkbook(filePath);
            var sheet = string.IsNullOrEmpty(sheetName) ? workbook.GetSheetAt(0) : workbook.GetSheet(sheetName);
            return SheetToDataTable(sheet);
        }

        public static DataTable ReadToDataTable(Stream stream, string sheetName = null, string extension = ".xlsx")
        {
            var workbook = LoadWorkbook(stream, extension);
            var sheet = string.IsNullOrEmpty(sheetName) ? workbook.GetSheetAt(0) : workbook.GetSheet(sheetName);
            return SheetToDataTable(sheet);
        }

        public static List<T> ReadToList<T>(string filePath, string sheetName = null) where T : new()
        {
            var dt = ReadToDataTable(filePath, sheetName);
            var list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                var obj = new T();
                foreach (DataColumn col in dt.Columns)
                {
                    var prop = typeof(T).GetProperty(col.ColumnName);
                    if (prop != null && dr[col] != DBNull.Value)
                        prop.SetValue(obj, Convert.ChangeType(dr[col], prop.PropertyType));
                }
                list.Add(obj);
            }
            return list;
        }

        public static List<Dictionary<string, object>> ReadToDictionary(string filePath, string sheetName = null)
        {
            var dt = ReadToDataTable(filePath, sheetName);
            return dt.AsEnumerable()
                     .Select(r => dt.Columns.Cast<DataColumn>()
                     .ToDictionary(c => c.ColumnName, c => r[c]))
                     .ToList();
        }

        #endregion

        #region 内部方法

        private static void FillSheet<T>(ISheet sheet, IEnumerable<T> data, bool includeHeader, int startRow = 0)
        {
            if (data == null || !data.Any()) return;

            if (data.First() is ExpandoObject)
            {
                var dicts = data.Cast<ExpandoObject>().Select(d => (IDictionary<string, object>)d).ToList();
                if (includeHeader)
                {
                    var headerRow = sheet.CreateRow(startRow++);
                    int colIndex = 0;
                    foreach (var h in dicts.First().Keys)
                    {
                        var cell = headerRow.CreateCell(colIndex++);
                        cell.SetCellValue(h);
                        var style = sheet.Workbook.CreateCellStyle();
                        var font = sheet.Workbook.CreateFont();
                        font.IsBold = true;
                        style.SetFont(font);
                        cell.CellStyle = style;
                    }
                }

                foreach (var rowDict in dicts)
                {
                    var row = sheet.CreateRow(startRow++);
                    int col = 0;
                    foreach (var val in rowDict.Values)
                        row.CreateCell(col++).SetCellValue(val?.ToString() ?? "");
                }
            }
            else if (data.First() is DataRow dr)
            {
                FillSheet(sheet, dr.Table, includeHeader, startRow);
            }
            else
            {
                var props = typeof(T).GetProperties();
                if (includeHeader)
                {
                    var headerRow = sheet.CreateRow(startRow++);
                    for (int i = 0; i < props.Length; i++)
                    {
                        var cell = headerRow.CreateCell(i);
                        cell.SetCellValue(props[i].Name);
                        var style = sheet.Workbook.CreateCellStyle();
                        var font = sheet.Workbook.CreateFont();
                        font.IsBold = true;
                        style.SetFont(font);
                        cell.CellStyle = style;
                    }
                }

                foreach (var item in data)
                {
                    var row = sheet.CreateRow(startRow++);
                    for (int i = 0; i < props.Length; i++)
                    {
                        var val = props[i].GetValue(item);
                        row.CreateCell(i).SetCellValue(val?.ToString() ?? "");
                    }
                }
            }

            AutoFit(sheet);
        }

        private static void FillSheet(ISheet sheet, DataTable table, bool includeHeader, int startRow = 0)
        {
            if (includeHeader)
            {
                var headerRow = sheet.CreateRow(startRow++);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(table.Columns[i].ColumnName);
                    var style = sheet.Workbook.CreateCellStyle();
                    var font = sheet.Workbook.CreateFont();
                    font.IsBold = true;
                    style.SetFont(font);
                    cell.CellStyle = style;
                }
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = sheet.CreateRow(startRow++);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(table.Rows[i][j]?.ToString() ?? "");
                }
            }

            AutoFit(sheet);
        }

        private static DataTable SheetToDataTable(ISheet sheet)
        {
            var dt = new DataTable(sheet.SheetName);
            if (sheet.PhysicalNumberOfRows == 0) return dt;

            var headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.LastCellNum; i++)
                dt.Columns.Add(headerRow.GetCell(i)?.ToString() ?? $"Column{i}");

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;
                var dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                    dr[j] = row.GetCell(j)?.ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static void AutoFit(ISheet sheet)
        {
            if (sheet.PhysicalNumberOfRows == 0) return;
            for (int i = 0; i < sheet.GetRow(0).LastCellNum; i++)
                sheet.AutoSizeColumn(i);
        }

        private static void SaveWorkbook(IWorkbook workbook, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fs);
        }

        #endregion
    }
}
