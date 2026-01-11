namespace CZJ.Extension
{
    public class ImageUtil
    {
        public static Image ToImage(byte[] @this)
        {
            using (var ms = new MemoryStream(@this))
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// 根据base64获取图片尺码
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public static System.Drawing.Size GetImageDimensionsFromByte(byte[] imageBytes)
        {
            if (imageBytes.Length == 0)
                return new System.Drawing.Size(0, 0);
            using (MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                using (Image image = Image.FromStream(memoryStream))
                {
                    return new Size(image.Width, image.Height);
                }
            }
        }

        public static Bitmap CaptureRegion(byte[] ImageByte, Rectangle region)
        {
            // 加载原始图像
            using (var originalBitmap = ByteToBitmap(ImageByte))
            {
                // 创建一个新的Bitmap来存储区域截图
                Bitmap regionBitmap = originalBitmap.Clone(region, originalBitmap.PixelFormat);
                return regionBitmap;
            }
        }

        public static Bitmap ByteToBitmap(byte[] ImageByte)
        {
            Bitmap bitmap = null;
            using (MemoryStream stream = new MemoryStream(ImageByte))
            {
                bitmap = new Bitmap((Image)new Bitmap(stream));
            }
            return bitmap;
        }

        public static byte[] BitmapByte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        /// <summary>
        /// 转换图片格式(压缩质量)
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="format"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static MemoryStream ConvertImageFormatToStream(Stream imageStream, ImageFormat format, int quality = 75)
        {
            // 确保输入流位于开始位置
            imageStream.Seek(0, SeekOrigin.Begin);
            using (Image tiffImage = Image.FromStream(imageStream))
            {
                // 创建一个新的Bitmap对象，尺寸与原图相同
                using (Bitmap bitmap = new Bitmap(tiffImage.Width, tiffImage.Height))
                {
                    // 使用Graphics对象将TIFF图像绘制到Bitmap对象上
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(tiffImage, 0, 0, tiffImage.Width, tiffImage.Height);
                    }
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality); // 设置压缩质量%
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ImageCodecInfo jpegCodecInfo = GetEncoder(format);
                        bitmap.Save(ms, jpegCodecInfo, encoderParams);
                        return ms;
                    }
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
