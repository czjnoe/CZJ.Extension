using SkiaSharp;

namespace CZJ.ImageExtensionTest
{
    [TestClass]
    public sealed class SkiaSharpTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var skImg = SkiaSharpUtil.Load("Images/input.jpg");
            skImg = skImg.Resize(800, 600);
            skImg.AddTextWatermark("Hello SkiaSharp", SKColors.Red, 36);
            skImg.Save("output_skiasharp.png", SkiaSharp.SKEncodedImageFormat.Png);
        }
    }
}
