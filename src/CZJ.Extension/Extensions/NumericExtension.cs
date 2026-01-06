namespace CZJ.Extension
{
    public static class NumericExtension
    {
        /// <summary>
        /// double 防止丢失精度 转换字符串(四舍五入)
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="reserveDigit">保留的小数位数</param>
        /// <returns></returns>
        public static string DoubleToStr(this double currentValue, string reserveDigit = "9")
        {
            return currentValue.ToString($"N{reserveDigit}");
        }

        /// <summary>
        /// double 防止丢失精度 (四舍五入)
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="reserveDigit">保留的小数位数</param>
        /// <returns></returns>
        public static double DoubleAccuracy(this double currentValue, string reserveDigit = "9")
        {
            return double.Parse(currentValue.ToString($"N{reserveDigit}"));
        }

        public static string ToFixedString(this double currentValue, int decimalPlaces = 9)
        {
            return currentValue.ToString($"F{decimalPlaces}");
        }

        public static string ToFixedString(this string currentValue, int decimalPlaces = 9)
        {
            return double.Parse(currentValue).ToString($"F{decimalPlaces}");
        }

        /// <summary>
        /// double转换成科学计数法
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        public static string ToScientificNotationStr(this double currentValue)
        {
            return currentValue.ToString("E");
        }

        /// <summary>
        /// 科学计数法字符串转Double
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        public static double ScientificNotationStrToNumeric(this string scientificNotationStr)
        {
            return double.Parse(scientificNotationStr, System.Globalization.NumberStyles.Float);
        }

        /// <summary>
        /// 百分比字符串展示(四舍五入)
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="reserveDigit">保留几位小数,默认2位</param>
        /// <returns></returns>
        public static string ToPercentageStr(this double currentValue, string reserveDigit = "2")
        {
            return currentValue.ToString($"p{reserveDigit}");
        }

        /// <summary>
        /// 单位米转换成毫米
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public static double DoubleMeterToMillimeter(this double currentValue)
        {
            return (currentValue * 1000.0).DoubleAccuracy();
        }

        /// <summary>
        /// 单位毫米转换成米
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public static double DoubleMillimeterToMeter(this double currentValue)
        {
            return (currentValue / 1000.0).DoubleAccuracy();
        }

        /// <summary>
        /// 单位米转换成微米
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public static double DoubleMeterToMicron(this double currentValue)
        {
            return (currentValue * 1000000.0).DoubleAccuracy();
        }

        /// <summary>
        /// 单位微米转换成米
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public static double DoubleMicronToMeter(this double currentValue)
        {
            return (currentValue / 1000000.0).DoubleAccuracy();
        }

        public static double DoubleMeterToNanoMeter(this double currentValue)
        {
            return (currentValue * 1000000000.0).DoubleAccuracy();
        }
    }
}
