using NPOI.SS.Formula.Functions;

namespace CZJ.ExcelExtension
{
    public static class EPPlusExtension
    {
        static EPPlusExtension()
        {
            ExcelPackage.License.SetNonCommercialPersonal("EPPlus");
        }

        #region 写入

        public static void SaveStreamToFile(this MemoryStream stream, string filePath)
        {
            EPPlusUtil.SaveStreamToFile(stream, filePath);
        }

        /// <summary>
        /// 追加多行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="count"></param>
        public static void AddEmptyRows(this ExcelWorksheet sheet, int count)
        {
            EPPlusUtil.AddEmptyRows(sheet, count);
        }

        /// <summary>
        /// 追加一行
        /// </summary>
        /// <param name="sheet"></param>
        public static void AddEmptyRow(this ExcelWorksheet sheet)
        {
            EPPlusUtil.AddEmptyRow(sheet);
        }

        /// <summary>
        /// 插入中间空行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="count"></param>
        public static void InsertEmptyRows(this ExcelWorksheet sheet, int rowIndex, int count)
        {
            EPPlusUtil.InsertEmptyRows(sheet, rowIndex, count);
        }

        #endregion

        #region 读取

        public static DataTable ToDataTable(this ExcelWorksheet sheet)
        {
            return EPPlusUtil.ToDataTable(sheet);
        }

        public static List<T> ReadSheetToList<T>(ExcelWorksheet sheet) where T : new()
        {
            return EPPlusUtil.ReadSheetToList<T>(sheet);
        }

        public static List<Dictionary<string, object>> ReadSheetToDictionary(ExcelWorksheet sheet)
        {
            return EPPlusUtil.ReadSheetToDictionary(sheet);
        }

        #endregion

        #region ExcelPackage

        /// <summary>
        /// 保存 ExcelPackage 到文件
        /// </summary>
        public static void SaveAsFile(this ExcelPackage package, string filePath)
        {
            EPPlusUtil.SaveAsFile(package, filePath);
        }

        /// <summary>
        /// 获取或创建 Sheet
        /// </summary>
        public static ExcelWorksheet GetOrCreateSheet(this ExcelPackage package, string sheetName)
        {
            return EPPlusUtil.GetOrCreateSheet(package, sheetName);
        }

        #endregion
    }
}


