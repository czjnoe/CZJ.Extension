namespace CZJ.Extension
{
    public static class NumericExtension
    {
        /// <summary>
        /// 四舍五入到指定小数位
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="decimals">小数位数</param>
        /// <param name="mode">舍入模式</param>
        public static double Round(this double value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, decimals, mode);
        }

        /// <summary>
        /// 格式化为指定小数位的字符串
        /// </summary>
        public static string ToFixedString(this string currentValue, int decimalPlaces = 9)
        {
            return double.Parse(currentValue).ToFixedString(decimalPlaces);
        }

        /// <summary>
        /// 格式化为指定小数位的字符串
        /// </summary>
        public static string ToFixedString(this double value, int decimals = 2)
        {
            return value.ToString($"F{decimals}");
        }

        /// <summary>
        /// 格式化为百分比字符串
        /// </summary>
        public static string ToPercent(this double value, int decimals = 2)
        {
            return value.ToString($"P{decimals}");
        }

        /// <summary>
        /// 格式化为科学计数法
        /// </summary>
        public static string ToScientific(this double value, int decimals = 2)
        {
            return value.ToString($"E{decimals}");
        }

        /// <summary>
        /// 转换为绝对值
        /// </summary>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 取相反数
        /// </summary>
        public static double Negate(this double value)
        {
            return -value;
        }

        public static int ToInt32(this double value, int defaultValue = 0)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long ToInt64(this double value, long defaultValue = 0)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转换为 Decimal
        /// </summary>
        public static decimal ToDecimal(this double value, decimal defaultValue = 0)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 计算百分比
        /// </summary>
        public static double PercentOf(this double value, double total)
        {
            if (total.IsZero())
                return 0;
            return (value / total) * 100;
        }

        public static bool TryParseDouble(this string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// 判断是否为零（考虑浮点精度）
        /// </summary>
        public static bool IsZero(this double value, double tolerance = 1e-10)
        {
            return Math.Abs(value) < tolerance;
        }

        /// <summary>
        /// 判断是否接近另一个值（考虑浮点精度）
        /// </summary>
        public static bool IsCloseTo(this double value, double other, double tolerance = 1e-10)
        {
            return Math.Abs(value - other) < tolerance;
        }

        /// <summary>
        /// 判断是否为正数
        /// </summary>
        public static bool IsPositive(this double value)
        {
            return value > 0;
        }

        /// <summary>
        /// 判断是否为负数
        /// </summary>
        public static bool IsNegative(this double value)
        {
            return value < 0;
        }

        /// <summary>
        /// 判断是否在指定范围内（包含边界）
        /// </summary>
        public static bool IsBetween(this double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断是否为有效数字（非 NaN 和 Infinity）
        /// </summary>
        public static bool IsValid(this double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// 判断是否为整数
        /// </summary>
        public static bool IsInteger(this double value, double tolerance = 1e-10)
        {
            return Math.Abs(value - Math.Round(value)) < tolerance;
        }

        /// <summary>
        /// 判断是否为偶数
        /// </summary>
        public static bool IsEven(this double value)
        {
            return value.IsInteger() && (int)value % 2 == 0;
        }

        /// <summary>
        /// 判断是否为奇数
        /// </summary>
        public static bool IsOdd(this double value)
        {
            return value.IsInteger() && (int)value % 2 != 0;
        }

        #region 长度转换

        /// <summary>
        /// 米转千米
        /// </summary>
        public static double MeterToKilometer(this double meter)
        {
            return meter / 1000;
        }

        /// <summary>
        /// 千米转米
        /// </summary>
        public static double KilometerToMeter(this double kilometer)
        {
            return kilometer * 1000;
        }

        /// <summary>
        /// 米转厘米
        /// </summary>
        public static double MeterToCentimeter(this double meter)
        {
            return meter * 100;
        }

        /// <summary>
        /// 厘米转米
        /// </summary>
        public static double CentimeterToMeter(this double centimeter)
        {
            return centimeter / 100;
        }

        /// <summary>
        /// 米转毫米
        /// </summary>
        public static double MeterToMillimeter(this double meter)
        {
            return meter * 1000;
        }

        /// <summary>
        /// 毫米转米
        /// </summary>
        public static double MillimeterToMeter(this double millimeter)
        {
            return millimeter / 1000;
        }

        /// <summary>
        /// 米转微米
        /// </summary>
        public static double MeterToMicrometer(this double meter)
        {
            return meter * 1_000_000;
        }

        /// <summary>
        /// 微米转米
        /// </summary>
        public static double MicrometerToMeter(this double micrometer)
        {
            return micrometer / 1_000_000;
        }

        /// <summary>
        /// 米转纳米
        /// </summary>
        public static double MeterToNanometer(this double meter)
        {
            return meter * 1_000_000_000;
        }

        /// <summary>
        /// 纳米转米
        /// </summary>
        public static double NanometerToMeter(this double nanometer)
        {
            return nanometer / 1_000_000_000;
        }

        /// <summary>
        /// 米转皮米
        /// </summary>
        public static double MeterToPicometer(this double meter)
        {
            return meter * 1_000_000_000_000;
        }

        /// <summary>
        /// 皮米转米
        /// </summary>
        public static double PicometerToMeter(this double picometer)
        {
            return picometer / 1_000_000_000_000;
        }

        /// <summary>
        /// 毫米转微米
        /// </summary>
        public static double MillimeterToMicrometer(this double millimeter)
        {
            return millimeter * 1000;
        }

        /// <summary>
        /// 微米转毫米
        /// </summary>
        public static double MicrometerToMillimeter(this double micrometer)
        {
            return micrometer / 1000;
        }

        /// <summary>
        /// 微米转纳米
        /// </summary>
        public static double MicrometerToNanometer(this double micrometer)
        {
            return micrometer * 1000;
        }

        /// <summary>
        /// 纳米转微米
        /// </summary>
        public static double NanometerToMicrometer(this double nanometer)
        {
            return nanometer / 1000;
        }

        /// <summary>
        /// 纳米转皮米
        /// </summary>
        public static double NanometerToPicometer(this double nanometer)
        {
            return nanometer * 1000;
        }

        /// <summary>
        /// 皮米转纳米
        /// </summary>
        public static double PicometerToNanometer(this double picometer)
        {
            return picometer / 1000;
        }

        /// <summary>
        /// 米转英寸
        /// </summary>
        public static double MeterToInch(this double meter)
        {
            return meter * 39.3701;
        }

        /// <summary>
        /// 英寸转米
        /// </summary>
        public static double InchToMeter(this double inch)
        {
            return inch / 39.3701;
        }

        /// <summary>
        /// 米转英尺
        /// </summary>
        public static double MeterToFeet(this double meter)
        {
            return meter * 3.28084;
        }

        /// <summary>
        /// 英尺转米
        /// </summary>
        public static double FeetToMeter(this double feet)
        {
            return feet / 3.28084;
        }

        /// <summary>
        /// 米转码
        /// </summary>
        public static double MeterToYard(this double meter)
        {
            return meter * 1.09361;
        }

        /// <summary>
        /// 码转米
        /// </summary>
        public static double YardToMeter(this double yard)
        {
            return yard / 1.09361;
        }

        /// <summary>
        /// 米转英里
        /// </summary>
        public static double MeterToMile(this double meter)
        {
            return meter / 1609.344;
        }

        /// <summary>
        /// 英里转米
        /// </summary>
        public static double MileToMeter(this double mile)
        {
            return mile * 1609.344;
        }

        /// <summary>
        /// 千米转英里
        /// </summary>
        public static double KilometerToMile(this double kilometer)
        {
            return kilometer / 1.60934;
        }

        /// <summary>
        /// 英里转千米
        /// </summary>
        public static double MileToKilometer(this double mile)
        {
            return mile * 1.60934;
        }

        /// <summary>
        /// 英寸转厘米
        /// </summary>
        public static double InchToCentimeter(this double inch)
        {
            return inch * 2.54;
        }

        /// <summary>
        /// 厘米转英寸
        /// </summary>
        public static double CentimeterToInch(this double centimeter)
        {
            return centimeter / 2.54;
        }

        /// <summary>
        /// 英尺转厘米
        /// </summary>
        public static double FeetToCentimeter(this double feet)
        {
            return feet * 30.48;
        }

        /// <summary>
        /// 厘米转英尺
        /// </summary>
        public static double CentimeterToFeet(this double centimeter)
        {
            return centimeter / 30.48;
        }

        /// <summary>
        /// 通用长度单位转换
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="fromUnit">源单位</param>
        /// <param name="toUnit">目标单位</param>
        public static double ConvertLength(this double value, LengthUnit fromUnit, LengthUnit toUnit)
        {
            if (fromUnit == toUnit)
                return value;

            // 先转换为米（基准单位）
            double meters = fromUnit switch
            {
                LengthUnit.Picometer => value / 1_000_000_000_000,
                LengthUnit.Nanometer => value / 1_000_000_000,
                LengthUnit.Micrometer => value / 1_000_000,
                LengthUnit.Millimeter => value / 1000,
                LengthUnit.Centimeter => value / 100,
                LengthUnit.Decimeter => value / 10,
                LengthUnit.Meter => value,
                LengthUnit.Kilometer => value * 1000,
                LengthUnit.Inch => value / 39.3701,
                LengthUnit.Feet => value / 3.28084,
                LengthUnit.Yard => value / 1.09361,
                LengthUnit.Mile => value * 1609.344,
                _ => value
            };

            // 从米转换为目标单位
            return toUnit switch
            {
                LengthUnit.Picometer => meters * 1_000_000_000_000,
                LengthUnit.Nanometer => meters * 1_000_000_000,
                LengthUnit.Micrometer => meters * 1_000_000,
                LengthUnit.Millimeter => meters * 1000,
                LengthUnit.Centimeter => meters * 100,
                LengthUnit.Decimeter => meters * 10,
                LengthUnit.Meter => meters,
                LengthUnit.Kilometer => meters / 1000,
                LengthUnit.Inch => meters * 39.3701,
                LengthUnit.Feet => meters * 3.28084,
                LengthUnit.Yard => meters * 1.09361,
                LengthUnit.Mile => meters / 1609.344,
                _ => meters
            };
        }

        #endregion
    }

    /// <summary>
    /// 长度单位枚举
    /// </summary>
    public enum LengthUnit
    {
        /// <summary>皮米 (pm)</summary>
        Picometer,
        /// <summary>纳米 (nm)</summary>
        Nanometer,
        /// <summary>微米 (μm)</summary>
        Micrometer,
        /// <summary>毫米 (mm)</summary>
        Millimeter,
        /// <summary>厘米 (cm)</summary>
        Centimeter,
        /// <summary>分米 (dm)</summary>
        Decimeter,
        /// <summary>米 (m)</summary>
        Meter,
        /// <summary>千米 (km)</summary>
        Kilometer,
        /// <summary>英寸 (in)</summary>
        Inch,
        /// <summary>英尺 (ft)</summary>
        Feet,
        /// <summary>码 (yd)</summary>
        Yard,
        /// <summary>英里 (mi)</summary>
        Mile
    }
}
