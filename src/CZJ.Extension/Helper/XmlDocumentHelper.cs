namespace CZJ.Extension
{
    public static class XmlDocumentHelper
    {

        /// <summary>
        /// 根据xml文件路径加载xml对象
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static XmlDocument GetXDocument(string xmlPath)
        {
            // 加载XML文档
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            return xmlDocument;
        }

        /// <summary>
        /// 根据XPath路径获取元素单个内容
        /// </summary>
        /// <param name="xmlDocuemt"></param>
        /// <param name="xPath">格式根据xml内容自定义 
        /// 目前是//Parameter[Name='Pr WaferId']/Value
        /// 可换成站位符//Parameter[Name='{0}']/Value</param>
        /// <returns></returns>

        public static string GetSingleNodeValue(XmlDocument xmlDocuemt, string xPath)
        {
            var xmlNode = xmlDocuemt.SelectSingleNode(xPath);

            if (xmlNode == null) throw new Exception($"当前节点不存在,XPath表达式:{xPath}");

            return xmlNode.InnerText;
        }

        /// <summary>
        /// 根据XPath路径获取元素多个内容
        /// </summary>
        /// <param name="xmlDocuemt"></param>
        /// <param name="xPath">格式根据xml内容自定义 
        /// 目前是//Parameter[Name='Pr WaferId']/Value
        /// 可换成站位符//Parameter[Name='{0}']/Value</param>
        /// <returns></returns>

        public static List<string> GetNodeValueList(XmlDocument xmlDocuemt, string xPath)
        {
            var xmlNodes = xmlDocuemt.SelectNodes(xPath);

            if (xmlNodes == null || xmlNodes.Count == 0) throw new Exception($"当前节点不存在,XPath表达式:{xPath}");

            var valueList = new List<string>();
            foreach (XmlNode xmlNode in xmlNodes)
            {
                valueList.Add(xmlNode.InnerText);
            }
            return valueList;
        }

        /// <summary>
        /// 根据XPath路径获取元素
        /// </summary>
        /// <param name="xmlDocuemt"></param>
        /// <param name="xPath">格式根据xml内容自定义 
        /// 目前是//Parameter[Name='Pr WaferId']/Value
        /// 可换成站位符//Parameter[Name='{0}']/Value</param>
        /// <returns></returns>
        public static XmlNode GetSingleNode(XmlDocument xmlDocuemt, string xPath)
        {
            var xmlNode = xmlDocuemt.SelectSingleNode(xPath);
            return xmlNode;
        }

        /// <summary>
        /// 根据XPath路径获取元素
        /// </summary>
        /// <param name="xmlDocuemt"></param>
        /// <param name="xPath">格式根据xml内容自定义 
        /// 目前是//Parameter[Name='Pr WaferId']/Value
        /// 可换成站位符//Parameter[Name='{0}']/Value</param>
        /// <returns></returns>
        public static XmlNodeList GetNodes(XmlDocument xmlDocuemt, string xPath)
        {
            var xmlNode = xmlDocuemt.SelectNodes(xPath);

            if (xmlNode == null) throw new Exception($"当前节点不存在,XPath表达式:{xPath}");

            return xmlNode;
        }
    }
}
