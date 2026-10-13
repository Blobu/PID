using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Framework.Utilities
{
    internal static class FileHelper
    {
        private const string ImageFilter =
            "Image files (*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png";

        #region Show File Dialog for loading an image
        public static string LoadFileDialog(string title)
        {
            return ShowFileDialog(new OpenFileDialog
            {
                Title = title,
                Filter = ImageFilter
            });
        }
        #endregion

        #region Load an image
        public static Image<TColor, byte> LoadImage<TColor>(string imagePath)
            where TColor : struct, IColor
        {
            using (var decoded = new Mat())
            {
                CvInvoke.Imdecode(File.ReadAllBytes(imagePath), LoadImageType.AnyColor | LoadImageType.AnyDepth, decoded);
                if (decoded.IsEmpty)
                    return new Image<TColor, byte>(imagePath);

                return ApplyOrientation(ToImage<TColor>(decoded), ReadOrientation(imagePath));
            }
        }

        private static Image<TColor, byte> ToImage<TColor>(Mat decoded)
            where TColor : struct, IColor
        {
            var image = new Image<TColor, byte>(decoded.Width, decoded.Height);

            if (decoded.Depth != DepthType.Cv16U)
                image.ConvertFrom(decoded);
            else if (decoded.NumberOfChannels == 1)
                ConvertFrom16Bit<Gray, TColor>(decoded, image);
            else if (decoded.NumberOfChannels == 3)
                ConvertFrom16Bit<Bgr, TColor>(decoded, image);
            else
                ConvertFrom16Bit<Bgra, TColor>(decoded, image);

            return image;
        }

        private static void ConvertFrom16Bit<TSource, TColor>(Mat decoded, Image<TColor, byte> image)
            where TSource : struct, IColor
            where TColor : struct, IColor
        {
            using (var source = new Image<TSource, ushort>(decoded.Width, decoded.Height, decoded.Step, decoded.DataPointer))
                image.ConvertFrom(source);
        }

        private static int ReadOrientation(string imagePath)
        {
            const int OrientationTag = 0x0112;

            try
            {
                using (var stream = File.OpenRead(imagePath))
                using (var image = Image.FromStream(stream, false, false))
                {
                    if (!image.PropertyIdList.Contains(OrientationTag))
                        return 1;

                    return BitConverter.ToUInt16(image.GetPropertyItem(OrientationTag).Value, 0);
                }
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static Image<TColor, byte> ApplyOrientation<TColor>(Image<TColor, byte> image, int orientation)
            where TColor : struct, IColor
        {
            if (orientation >= 5 && orientation <= 8)
            {
                var transposed = new Image<TColor, byte>(image.Height, image.Width);
                CvInvoke.Transpose(image, transposed);
                image = transposed;
            }

            switch (orientation)
            {
                case 2:
                case 6:
                    return image.Flip(FlipType.Horizontal);
                case 3:
                case 7:
                    return image.Flip(FlipType.Horizontal | FlipType.Vertical);
                case 4:
                case 8:
                    return image.Flip(FlipType.Vertical);
                default:
                    return image;
            }
        }
        #endregion

        #region Show File Dialog for saving an image
        public static string SaveFileDialog(string imageName)
        {
            return ShowFileDialog(new SaveFileDialog
            {
                FileName = imageName,
                Filter = ImageFilter
            });
        }
        #endregion

        #region Save an image
        public static void SaveImage<TColor>(Image<TColor, byte> image, string imagePath)
            where TColor : struct, IColor
        {
            using (Bitmap bitmap = image.ToBitmap())
            {
                switch (Path.GetExtension(imagePath).ToLowerInvariant())
                {
                    case ".png":
                        bitmap.Save(imagePath, ImageFormat.Png);
                        break;
                    case ".bmp":
                        bitmap.Save(imagePath, ImageFormat.Bmp);
                        break;
                    default:
                        bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                        break;
                }
            }
        }

        public static EncoderParameters GetEncoderParameter(Encoder encoder, long value)
        {
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                encoder,
                value
            );

            return encoderParams;
        }

        public static ImageCodecInfo GetJpegCodec(string mimeType)
        {
            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == mimeType
                             select codec).Single();

            return jpegCodec;
        }
        #endregion

        #region Helpers
        private static string ShowFileDialog(FileDialog fileDialog)
        {
            if (fileDialog.ShowDialog() == false || fileDialog.FileName == "")
                return null;

            return fileDialog.FileName;
        }
        #endregion
    }
}