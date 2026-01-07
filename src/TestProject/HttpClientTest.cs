using System.Net.Http;
using System.Reflection.PortableExecutable;

namespace TestProject
{
    [TestClass]
    public sealed class HttpClientTest
    {

        [TestMethod]
        public void HttpSuccessTest()
        {
            var httpHelper = new HttpClientHelper();
            var content = httpHelper.Get<string>("https://api.github.com/repos/czjnoe/CZJ.Extension/contents", new CZJ.Extension.HttpRequestOption
            {
                Headers=new Dictionary<string, string>
                {
                    { "User-Agent", "CZJ.Extension" }
                }
            });
            Assert.IsTrue(content.Success);
        }

        [TestMethod]
        public void HttpFailTest()
        {
            var httpHelper = new HttpClientHelper();
            var content = httpHelper.Get<string>("https://api.github.com/repos/czjnoe/", new CZJ.Extension.HttpRequestOption
            {
                Headers = new Dictionary<string, string>
                {
                    { "User-Agent", "CZJ.Extension" }
                }
            });
            Assert.IsFalse(content.Success);
        }
    }
}
