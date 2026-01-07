using System.Net.Http;
using System.Reflection.PortableExecutable;

namespace TestProject
{
    [TestClass]
    public sealed class HttpClientTest
    {

        [TestMethod]
        public void TestHttpClient()
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
    }
}
