using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZJ.ImageExtension
{
    public static class ImageSharpUtil
    {
        #region 加载和保存

        public static Image<Rgba32> Load(string filePath) => SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);

        public static Image<Rgba32> Load(Stream stream) => SixLabors.ImageSharp.Image.Load<Rgba32>(stream);

        public static Image<Rgba32> Load(byte[] bytes) => SixLabors.ImageSharp.Image.Load<Rgba32>(bytes);

        /// <summary>
        /// 保存 Image<Rgba32> 到文件，自动创建目录
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="format">保存格式，可选 png/jpeg</param>
        public static void Save(Image<Rgba32> img, string filePath, string format = "png")
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            format = format.ToLower();
            switch (format)
            {
                case "png":
                    img.Save(filePath, new PngEncoder());
                    break;
                case "jpeg":
                case "jpg":
                    img.Save(filePath, new JpegEncoder());
                    break;
                default:
                    throw new ArgumentException($"不支持的图片格式: {format}");
            }
        }

        public static void Save(Image<Rgba32> img, Stream stream, string format = "png")
        {
            format = format.ToLower();
            if (format == "png")
                img.SaveAsPng(stream);
            else if (format == "jpg" || format == "jpeg")
                img.SaveAsJpeg(stream);
            else if (format == "bmp")
                img.SaveAsBmp(stream);
            else
                img.SaveAsPng(stream);
        }

        /// <summary>
        /// 保存 Image 到文件
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="format">保存格式，可选：png/jpeg</param>
        /// <returns>返回完整文件路径</returns>
        public static string SaveToFile(SixLabors.ImageSharp.Image img, string filePath, string format = "png")
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            format = format.ToLower();
            switch (format)
            {
                case "png":
                    img.Save(filePath, new PngEncoder());
                    break;
                case "jpeg":
                case "jpg":
                    img.Save(filePath, new JpegEncoder());
                    break;
                default:
                    throw new ArgumentException($"不支持的图片格式: {format}");
            }

            return Path.GetFullPath(filePath);
        }

        /// <summary>
        /// 保存 Image 到 MemoryStream
        /// </summary>
        /// <param name="img">Image 对象</param>
        /// <param name="format">保存格式，可选：png/jpeg</param>
        /// <returns>返回 MemoryStream（Position 已重置）</returns>
        public static MemoryStream SaveToStream(SixLabors.ImageSharp.Image img, string format = "png")
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            var ms = new MemoryStream();
            format = format.ToLower();
            switch (format)
            {
                case "png":
                    img.Save(ms, new PngEncoder());
                    break;
                case "jpeg":
                case "jpg":
                    img.Save(ms, new JpegEncoder());
                    break;
                default:
                    throw new ArgumentException($"不支持的图片格式: {format}");
            }

            ms.Position = 0; // 重置流位置
            return ms;
        }

        public static byte[] ToByteArray(Image<Rgba32> img)
        {
            using var ms = new MemoryStream();
            img.SaveAsPng(ms);
            return ms.ToArray();
        }

        public static void Save(Image<Rgba32> img, Stream stream) => img.SaveAsPng(stream);

        #endregion

        #region 图像处理

        public static void Resize(Image<Rgba32> img, int width, int height) => img.Mutate(x => x.Resize(width, height));

        public static void Crop(Image<Rgba32> img, SixLabors.ImageSharp.Rectangle rect) => img.Mutate(x => x.Crop(rect));

        public static void Rotate(Image<Rgba32> img, float degrees) => img.Mutate(x => x.Rotate(degrees));

        public static void FlipHorizontal(Image<Rgba32> img) => img.Mutate(x => x.Flip(FlipMode.Horizontal));

        public static void FlipVertical(Image<Rgba32> img) => img.Mutate(x => x.Flip(FlipMode.Vertical));

        public static void AdjustBrightness(Image<Rgba32> img, float brightness) => img.Mutate(x => x.Brightness(brightness));

        public static void AdjustContrast(Image<Rgba32> img, float contrast) => img.Mutate(x => x.Contrast(contrast));

        #endregion

        #region 水印

        /// <summary>
        /// 添加图片水印
        /// </summary>
        public static void AddImageWatermark(Image<Rgba32> img, Image<Rgba32> watermark, float opacity = 0.5f, int margin = 10)
        {
            int x = img.Width - watermark.Width - margin;
            int y = img.Height - watermark.Height - margin;
            img.Mutate(ctx => ctx.DrawImage(watermark, new SixLabors.ImageSharp.Point(x, y), opacity));
        }

        #endregion

        #region 图片合并

        public static Image<Rgba32> MergeHorizontal(params Image<Rgba32>[] images)
        {
            if (images == null || images.Length == 0) throw new ArgumentException("images 不能为空");

            int totalWidth = 0;
            int maxHeight = 0;
            foreach (var img in images)
            {
                totalWidth += img.Width;
                maxHeight = Math.Max(maxHeight, img.Height);
            }

            var result = new Image<Rgba32>(totalWidth, maxHeight);
            int offsetX = 0;
            foreach (var img in images)
            {
                result.Mutate(x => x.DrawImage(img, new SixLabors.ImageSharp.Point(offsetX, 0), 1));
                offsetX += img.Width;
            }

            return result;
        }

        public static Image<Rgba32> MergeVertical(params Image<Rgba32>[] images)
        {
            if (images == null || images.Length == 0) throw new ArgumentException("images 不能为空");

            int totalHeight = 0;
            int maxWidth = 0;
            foreach (var img in images)
            {
                totalHeight += img.Height;
                maxWidth = Math.Max(maxWidth, img.Width);
            }

            var result = new Image<Rgba32>(maxWidth, totalHeight);
            int offsetY = 0;
            foreach (var img in images)
            {
                result.Mutate(x => x.DrawImage(img, new SixLabors.ImageSharp.Point(0, offsetY), 1));
                offsetY += img.Height;
            }

            return result;
        }

        #endregion

        public static Image<Rgba32> ToImageSharp(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            return SixLabors.ImageSharp.Image.Load<Rgba32>(memoryStream);
        }

        public static Bitmap ToImage(Image<Rgba32> image)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            memoryStream.Position = 0;
            return new Bitmap(memoryStream);
        }
    }
}
