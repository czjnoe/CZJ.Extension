namespace CZJ.Extension
{
    public class UrlHelper
    {
        /// <summary>
        /// UrlEncode编码
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static string UrlEncode(string url)
        {
            return System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
        }
        /// <summary>
        ///  UrlEncode解码
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string UrlDecode(string data)
        {
            return System.Web.HttpUtility.UrlDecode(data, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 根据字段拼接get参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetPars(Dictionary<string, object> dic)
        {

            StringBuilder sb = new StringBuilder();
            string urlPars = null;
            bool isEnter = false;
            foreach (var item in dic)
            {
                sb.Append($"{(isEnter ? "&" : "")}{item.Key}={item.Value}");
                isEnter = true;
            }
            urlPars = sb.ToString();
            return urlPars;
        }
    }
}
