namespace CZJ.ImageExtension
{
    public static class ImageSharpExtension
    {
        #region 加载和保存

        /// <summary>
        /// 保存 Image<Rgba32> 到文件，自动创建目录
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="format">保存格式，可选 png/jpeg</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Save(this Image<Rgba32> img, string filePath, string format = "png")
        {
            ImageSharpUtil.Save(img, filePath, format);
        }

        public static void Save(this Image<Rgba32> img, Stream stream, string format = "png")
        {
            ImageSharpUtil.Save(img, stream, format);
        }

        /// <summary>
        /// 保存 Image 到文件
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="format">保存格式，可选：png/jpeg</param>
        /// <returns>返回完整文件路径</returns>
        public static string SaveToFile(this SixLabors.ImageSharp.Image img, string filePath, string format = "png")
        {
            return ImageSharpUtil.SaveToFile(img, filePath, format);
        }

        /// <summary>
        /// 保存 Image 到 MemoryStream
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="format">保存格式，可选：png/jpeg</param>
        /// <returns>返回 MemoryStream（Position 已重置）</returns>
        public static MemoryStream SaveToStream(this SixLabors.ImageSharp.Image img, string format = "png")
        {
            return ImageSharpUtil.SaveToStream(img, format);
        }

        public static byte[] ToByteArray(this Image<Rgba32> img)
        {
            return ImageSharpUtil.ToByteArray(img);
        }

        public static void Save(this Image<Rgba32> img, Stream stream) => ImageSharpUtil.Save(img, stream);

        #endregion

        #region 图像处理

        public static void Resize(this Image<Rgba32> img, int width, int height) => ImageSharpUtil.Resize(img, width, height);

        public static void Crop(this Image<Rgba32> img, SixLabors.ImageSharp.Rectangle rect) => ImageSharpUtil.Crop(img, rect);

        public static void Rotate(this Image<Rgba32> img, float degrees) => ImageSharpUtil.Rotate(img, degrees);

        public static void FlipHorizontal(this Image<Rgba32> img) => ImageSharpUtil.FlipHorizontal(img);

        public static void FlipVertical(this Image<Rgba32> img) => ImageSharpUtil.FlipVertical(img);

        public static void AdjustBrightness(this Image<Rgba32> img, float brightness) => ImageSharpUtil.AdjustBrightness(img, brightness);

        public static void AdjustContrast(this Image<Rgba32> img, float contrast) => ImageSharpUtil.AdjustContrast(img, contrast);

        #endregion

        #region 水印

        /// <summary>
        /// 添加图片水印
        /// </summary>
        public static void AddImageWatermark(this Image<Rgba32> img, Image<Rgba32> watermark, float opacity = 0.5f, int margin = 10)
        {
            ImageSharpUtil.AddImageWatermark(img, watermark, opacity, margin);
        }

        #endregion
    }
}
