using CZJ.Extension;

namespace CZJ.ImageExtensionTest
{
    [TestClass]
    public sealed class ImageSharpTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var img = ImageSharpUtil.Load("Images/input.jpg");
            img.Resize(800, 600);
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output_imagesharp.png");
            img.Save(outputPath);
        }
    }
}
