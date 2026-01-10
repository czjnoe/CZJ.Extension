namespace CZJ.Extension
{
    public static class StringExtension
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static bool IsUrl(this string input)
        {
            var pattern = @"^(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
            return Regex.IsMatch(input, pattern);
        }

        public static string GetUrlLastNumRouteName(this string url, int index = 1)
        {
            if (url == null)
                return url;
            var urlArray = url.Split('/');
            var name = urlArray[urlArray.Length - index];
            return name;
        }

        public static string FirstCharUpper(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return string.Concat(value[0].ToString().ToUpper(), value.AsSpan(1));
            return value;
        }

        public static string FirstCharLower(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return string.Concat(value[0].ToString().ToLower(), value.AsSpan(1));
            return value;
        }

        public static string GetFirstNormalConn(this string conn)
        {
            if (string.IsNullOrEmpty(conn)) return conn;

            var dict = conn.Split(';')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToDictionary(x => x.Split('=')[0].ToLower(), x => x.Split('=')[1]);
            var item = dict.FirstOrDefault(x => x.Key == "server");

            if (null == item.Value || !item.Value.Contains(',')) return conn;

            var masterServer = item.Value.Split(',')[0];
            var port = masterServer.Contains(':') ? masterServer.Split(':')[1] : "5432";
            dict[item.Key] = masterServer.Split(':')[0];
            if (dict.ContainsKey("port"))
                dict["port"] = port;
            else
                dict.Add("port", port);

            var items = dict.Select(x => $"{x.Key}={x.Value}");
            var singleConn = string.Join(";", items);
            return singleConn;
        }

        public static List<int> ConvertStringToListInt(this string convertInfo, char splitChar)
        {
            var convertList = new List<int>();

            if (string.IsNullOrEmpty(convertInfo)) return convertList;
            var splitStrList = convertInfo.Split(splitChar);

            splitStrList.ToList().ForEach(obj =>
            {
                if (int.TryParse(obj, out var num)) convertList.Add(num);
            });

            return convertList;
        }

        #region 空判断

        /// <summary>
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string inputStr)
        {
            return string.IsNullOrEmpty(inputStr);
        }

        /// <summary>
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string inputStr)
        {
            return string.IsNullOrWhiteSpace(inputStr);
        }

        /// <summary>
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string inputStr)
        {
            return !string.IsNullOrEmpty(inputStr);
        }

        /// <summary>
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string inputStr)
        {
            return !string.IsNullOrWhiteSpace(inputStr);
        }

        #endregion

        public static bool IsInt(this object obj)
        {
            if (obj == null)
                return false;
            bool reslut = Int32.TryParse(obj.ToString(), out int _number);
            return reslut;

        }

        public static StringBuilder AppendIfNotNull(this StringBuilder @this, string condition, string appendStr)
        {
            return condition.IsNullOrWhiteSpace() ? @this : @this.Append(appendStr);
        }

        public static StringBuilder AppendIfNotNull<T>(this StringBuilder @this, T? condition, string appendStr) where T : struct
        {
            return !condition.HasValue ? @this : @this.Append(appendStr);
        }

        public static StringBuilder AppendIf(this StringBuilder @this, bool condition, string appendStr)
        {
            return condition ? @this.Append(appendStr) : @this;
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            DateTime.TryParse(value, out var result);
            return result;
        }

        #region 常用正则表达式

        private static readonly Regex EmailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

        private static readonly Regex MobileRegex = new Regex("^1[0-9]{10}$");

        private static readonly Regex PhoneRegex = new Regex(@"^(\d{3,4}-?)?\d{7,8}$");

        private static readonly Regex IpRegex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");

        private static readonly Regex DateRegex = new Regex(@"(\d{4})-(\d{1,2})-(\d{1,2})");

        private static readonly Regex NumericRegex = new Regex(@"^[-]?[0-9]+(\.[0-9]+)?$");

        private static readonly Regex ZipcodeRegex = new Regex(@"^\d{6}$");

        private static readonly Regex IdRegex = new Regex(@"^[1-9]\d{16}[\dXx]$");

        /// <summary>
        /// 是否中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fff]");
        }

        /// <summary>
        /// 是否为邮箱名
        /// </summary>
        public static bool IsEmail(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return EmailRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否为手机号
        /// </summary>
        public static bool IsMobile(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return MobileRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否为固话号
        /// </summary>
        public static bool IsPhone(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return PhoneRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否为IP
        /// </summary>
        public static bool IsIp(this string s)
        {
            return IpRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否是身份证号
        /// </summary>
        public static bool IsIdCard(this string idCard)
        {
            if (string.IsNullOrEmpty(idCard))
                return false;
            return IdRegex.IsMatch(idCard);
        }

        /// <summary>
        /// 是否为日期
        /// </summary>
        public static bool IsDate(this string s)
        {
            return DateRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsNumeric(this string numericStr)
        {
            return NumericRegex.IsMatch(numericStr);
        }

        /// <summary>
        /// 是否为邮政编码
        /// </summary>
        public static bool IsZipCode(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return ZipcodeRegex.IsMatch(s);
        }

        /// <summary>
        /// 是否是图片文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsImgFileName(this string fileName)
        {
            var suffix = new List<string>() { ".jpg", ".jpeg", ".png" };

            var fileSuffix = Path.GetExtension(fileName).ToLower();

            return suffix.Contains(fileSuffix);
        }

        #endregion

        /// <summary>
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            string pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 正整数判断
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumberR(this string str)
        {
            string pattern = @"^[0-9]*[1-9][0-9]*$";
            return Regex.IsMatch(str, pattern);
        }

        public static string Join(this IEnumerable<string> strs, string separate = ", ", bool removeEmptyEntry = false) => string.Join(separate, removeEmptyEntry ? strs.Where(s => !string.IsNullOrEmpty(s)) : strs);
    }
}
