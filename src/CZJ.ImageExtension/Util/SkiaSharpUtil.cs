using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZJ.ImageExtension
{
    public static class SkiaSharpUtil
    {
        #region 加载和保存

        public static SKBitmap Load(string path) => SKBitmap.Decode(path);

        public static SKBitmap Load(Stream stream) => SKBitmap.Decode(stream);

        public static void Save(SKBitmap bitmap, string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            using var img = SKImage.FromBitmap(bitmap);
            using var data = img.Encode(format, quality);
            using var fs = File.OpenWrite(path);
            data.SaveTo(fs);
        }

        #endregion

        #region 图像处理

        public static SKBitmap Resize(SKBitmap bitmap, int width, int height)
        {
            var resized = new SKBitmap(width, height);
            bitmap.ScalePixels(resized, SKFilterQuality.High);
            return resized;
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="bitmap">原图</param>
        /// <param name="rect">裁剪区域</param>
        /// <returns>裁剪后的新图</returns>
        public static SKBitmap Crop(SKBitmap bitmap, SKRectI rect)
        {
            // 确保裁剪区域不超出原图
            var safeRect = new SKRectI(
                Math.Max(0, rect.Left),
                Math.Max(0, rect.Top),
                Math.Min(bitmap.Width, rect.Right),
                Math.Min(bitmap.Height, rect.Bottom)
            );

            var cropped = new SKBitmap(safeRect.Width, safeRect.Height, bitmap.ColorType, bitmap.AlphaType);
            bitmap.ExtractSubset(cropped, safeRect);
            return cropped;
        }

        /// <summary>
        /// 裁剪并保存为文件
        /// </summary>
        public static void CropAndSave(string inputFile, string outputFile, SKRectI rect)
        {
            using var bitmap = SKBitmap.Decode(inputFile);
            using var cropped = bitmap.Crop(rect);
            using var image = SKImage.FromBitmap(cropped);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(outputFile);
            data.SaveTo(stream);
        }

        public static SKBitmap Rotate(SKBitmap bitmap, float degrees)
        {
            var image = SKImage.FromBitmap(bitmap);
            var rotated = new SKBitmap(bitmap.Width, bitmap.Height);
            using var canvas = new SKCanvas(rotated);
            canvas.Translate(rotated.Width / 2, rotated.Height / 2);
            canvas.RotateDegrees(degrees);
            canvas.Translate(-bitmap.Width / 2, -bitmap.Height / 2);
            canvas.DrawBitmap(bitmap, 0, 0);
            return rotated;
        }

        public static SKBitmap FlipHorizontal(SKBitmap bitmap)
        {
            var flipped = new SKBitmap(bitmap.Width, bitmap.Height);
            using var canvas = new SKCanvas(flipped);
            canvas.Scale(-1, 1, bitmap.Width / 2f, bitmap.Height / 2f);
            canvas.DrawBitmap(bitmap, 0, 0);
            return flipped;
        }

        public static SKBitmap FlipVertical(SKBitmap bitmap)
        {
            var flipped = new SKBitmap(bitmap.Width, bitmap.Height);
            using var canvas = new SKCanvas(flipped);
            canvas.Scale(1, -1, bitmap.Width / 2f, bitmap.Height / 2f);
            canvas.DrawBitmap(bitmap, 0, 0);
            return flipped;
        }

        #endregion

        #region 水印

        public static void AddTextWatermark(SKBitmap bitmap, string text, SKColor color, float textSize = 24, int margin = 10)
        {
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint
            {
                Color = color,
                TextSize = textSize,
                IsAntialias = true
            };
            canvas.DrawText(text, margin, textSize + margin, paint);
        }

        public static void AddImageWatermark(SKBitmap bitmap, SKBitmap watermark, float opacity = 0.5f, int margin = 10)
        {
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(opacity * 255)) };
            canvas.DrawBitmap(watermark, margin, margin, paint);
        }

        #endregion

        #region Create

        public static SKBitmap CreateSolidColor(int width, int height, SKColor color)
        {
            var bitmap = new SKBitmap(width, height);
            bitmap.Erase(color);
            return bitmap;
        }

        #endregion

        public static SKImage ToSKImage(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            return SKImage.FromEncodedData(memoryStream);
        }
        public static Bitmap ToImage(SKImage image)
        {
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var memoryStream = new MemoryStream();
            data.SaveTo(memoryStream);
            memoryStream.Position = 0;
            return new Bitmap(memoryStream);
        }
    }
}
