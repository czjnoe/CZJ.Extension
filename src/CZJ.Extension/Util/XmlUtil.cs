namespace CZJ.Extension
{
    public class XmlUtil
    {
        /// <summary>
        /// 初始化Xml操作
        /// </summary>
        /// <param name="xml">Xml字符串</param>
        public XmlUtil(string xml = null)
        {
            Document = new XmlDocument();
            Document.LoadXml(GetXml(xml));
            Root = Document.DocumentElement;
            if (Root == null)
                throw new ArgumentException(nameof(xml));
        }

        /// <summary>
        /// 将Xml字符串转换为XDocument
        /// </summary>
        /// <param name="xml">Xml字符串</param>
        public static XDocument ToDocument(string xml)
        {
            return XDocument.Parse(xml);
        }

        /// <summary>
        /// 将Xml字符串转换为XElement列表
        /// </summary>
        /// <param name="xml">Xml字符串</param>
        public static List<XElement> ToElements(string xml)
        {
            var document = ToDocument(xml);
            if (document?.Root == null)
                return new List<XElement>();
            return document.Root.Elements().ToList();
        }

        /// <summary>
        /// 加载Xml文件到XDocument
        /// </summary>
        /// <param name="filePath">Xml文件绝对路径</param>
        public static async Task<XDocument> LoadFileToDocumentAsync(string filePath)
        {
            return await LoadFileToDocumentAsync(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 加载Xml文件到XDocument
        /// </summary>
        /// <param name="filePath">Xml文件绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static async Task<XDocument> LoadFileToDocumentAsync(string filePath, Encoding encoding)
        {
            var xml = await FileUtil.ReadToStringAsync(filePath, encoding);
            return ToDocument(xml);
        }

        /// <summary>
        /// 加载Xml文件到XElement列表
        /// </summary>
        /// <param name="filePath">Xml文件绝对路径</param>
        public static async Task<List<XElement>> LoadFileToElementsAsync(string filePath)
        {
            return await LoadFileToElementsAsync(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 加载Xml文件到XElement列表
        /// </summary>
        /// <param name="filePath">Xml文件绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static async Task<List<XElement>> LoadFileToElementsAsync(string filePath, Encoding encoding)
        {
            var xml = await FileUtil.ReadToStringAsync(filePath, encoding);
            return ToElements(xml);
        }

        /// <summary>
        /// 获取Xml字符串
        /// </summary>
        private string GetXml(string xml)
        {
            return string.IsNullOrWhiteSpace(xml) ? "<xml></xml>" : xml;
        }

        /// <summary>
        /// Xml文档
        /// </summary>
        public XmlDocument Document { get; }

        /// <summary>
        /// Xml根节点
        /// </summary>
        public XmlElement Root { get; }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="value">值</param>
        /// <param name="parent">父节点</param>
        public XmlNode AddNode(string name, object value = null, XmlNode parent = null)
        {
            var node = CreateNode(name, value, XmlNodeType.Element);
            GetParent(parent).AppendChild(node);
            return node;
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        private XmlNode CreateNode(string name, object value, XmlNodeType type)
        {
            var node = Document.CreateNode(type, name, string.Empty);
            if (string.IsNullOrWhiteSpace(value.ToString()) == false)
                node.InnerText = value.ToString();
            return node;
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        private XmlNode GetParent(XmlNode parent)
        {
            if (parent == null)
                return Root;
            return parent;
        }

        /// <summary>
        /// 添加CDATA节点
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="parent">父节点</param>
        public XmlNode AddCDataNode(object value, XmlNode parent = null)
        {
            var node = CreateNode(CreateId(), value, XmlNodeType.CDATA);
            GetParent(parent).AppendChild(node);
            return node;
        }

        /// <summary>
        /// 创建标识
        /// </summary>
        private string CreateId()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 添加CDATA节点
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="parentName">父节点名称</param>
        public XmlNode AddCDataNode(object value, string parentName)
        {
            var parent = CreateNode(parentName, null, XmlNodeType.Element);
            Root.AppendChild(parent);
            return AddCDataNode(value, parent);
        }

        /// <summary>
        /// 输出Xml
        /// </summary>
        public override string ToString()
        {
            return Document.OuterXml;
        }

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
