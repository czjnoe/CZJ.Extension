namespace CZJ.ImageExtension
{
    public static class SkiaSharpExtension
    {
        #region 加载和保存

        public static void Save(this SKBitmap bitmap, string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            SkiaSharpUtil.Save(bitmap, path, format, quality);
        }

        #endregion

        #region 图像处理

        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height)
        {
            return SkiaSharpUtil.Resize(bitmap, width, height);
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="bitmap">原图</param>
        /// <param name="rect">裁剪区域</param>
        /// <returns>裁剪后的新图</returns>
        public static SKBitmap Crop(this SKBitmap bitmap, SKRectI rect)
        {
            return SkiaSharpUtil.Crop(bitmap, rect);
        }

        public static SKBitmap Rotate(this SKBitmap bitmap, float degrees)
        {
            return SkiaSharpUtil.Rotate(bitmap, degrees);
        }

        public static SKBitmap FlipHorizontal(this SKBitmap bitmap)
        {
            return SkiaSharpUtil.FlipHorizontal(bitmap);
        }

        public static SKBitmap FlipVertical(this SKBitmap bitmap)
        {
            return SkiaSharpUtil.FlipVertical(bitmap);
        }

        #endregion

        #region 水印

        public static void AddTextWatermark(this SKBitmap bitmap, string text, SKColor color, float textSize = 24, int margin = 10)
        {
            SkiaSharpUtil.AddTextWatermark(bitmap, text, color, textSize, margin);
        }

        public static void AddImageWatermark(this SKBitmap bitmap, SKBitmap watermark, float opacity = 0.5f, int margin = 10)
        {
            SkiaSharpUtil.AddImageWatermark(bitmap, watermark, opacity, margin);
        }

        #endregion
    }
}
