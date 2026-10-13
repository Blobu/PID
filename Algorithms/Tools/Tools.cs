using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, byte threshold = 128)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] >= threshold)
                        result.Data[y, x, 0] = 255;
                }
            }
            return result;
        }

        #endregion

        #region Mirror Image
        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                    result.Data[y, x, 1] = inputImage.Data[y, inputImage.Width - 1 - x, 1];
                    result.Data[y, x, 2] = inputImage.Data[y, inputImage.Width - 1 - x, 2];
                }
            }
            return result;
        }
        #endregion

        #region Rotate
        public static Image<Gray, byte> Rotate(Image<Gray, byte> inputImage, byte direction = 0)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(new System.Drawing.Size(inputImage.Height, inputImage.Width));
            if (direction == 0) // Rotate 90 degrees clockwise
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    for (int x = 0; x < inputImage.Width; ++x)
                    {
                        result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    }
                }
            }
            else if (direction == 1) // Rotate 90 degrees counter-clockwise
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    for (int x = 0; x < inputImage.Width; ++x)
                    {
                        result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    }
                }
            }
            return result;
        }
        public static Image<Bgr, byte> Rotate(Image<Bgr, byte> inputImage, byte direction = 0)
        {
            Image<Bgr, byte> result  = new Image<Bgr, byte>(new System.Drawing.Size(inputImage.Height, inputImage.Width));
            if (direction == 0) // Rotate 90 degrees clockwise
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    for (int x = 0; x < inputImage.Width; ++x)
                    {
                        result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                        result.Data[x, inputImage.Height - 1 - y, 1] = inputImage.Data[y, x, 1];
                        result.Data[x, inputImage.Height - 1 - y, 2] = inputImage.Data[y, x, 2];
                    }
                }
            }
            else if (direction == 1) // Rotate 90 degrees counter-clockwise
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    for (int x = 0; x < inputImage.Width; ++x)
                    {
                        result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                        result.Data[inputImage.Width - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                        result.Data[inputImage.Width - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                    }
                }
            }
            
            return result;
        }
        #endregion

        #region Crop
        // Crop gray
        public static Image<Gray, byte> Crop(Image<Gray, byte> input, int x, int y, int width, int height)
        {
            // clamp
            x = Math.Max(0, Math.Min(x, input.Width - 1));
            y = Math.Max(0, Math.Min(y, input.Height - 1));
            width = Math.Max(0, Math.Min(width, input.Width - x));
            height = Math.Max(0, Math.Min(height, input.Height - y));

            var result = new Image<Gray, byte>(width, height);
            for (int ry = 0; ry < height; ++ry)
                for (int rx = 0; rx < width; ++rx)
                    result.Data[ry, rx, 0] = input.Data[y + ry, x + rx, 0];
            return result;
        }

        // Crop color
        public static Image<Bgr, byte> Crop(Image<Bgr, byte> input, int x, int y, int width, int height)
        {
            x = Math.Max(0, Math.Min(x, input.Width - 1));
            y = Math.Max(0, Math.Min(y, input.Height - 1));
            width = Math.Max(0, Math.Min(width, input.Width - x));
            height = Math.Max(0, Math.Min(height, input.Height - y));

            var result = new Image<Bgr, byte>(width, height);
            for (int ry = 0; ry < height; ++ry)
            {
                for (int rx = 0; rx < width; ++rx)
                {
                    result.Data[ry, rx, 0] = input.Data[y + ry, x + rx, 0];
                    result.Data[ry, rx, 1] = input.Data[y + ry, x + rx, 1];
                    result.Data[ry, rx, 2] = input.Data[y + ry, x + rx, 2];
                }
            }
            return result;
        }

        // Gray stats: mean + stdDev (RMS)
        public static (double mean, double stdDev) ComputeGrayStats(Image<Gray, byte> img)
        {
            long sum = 0;
            long sumSq = 0;
            int n = img.Width * img.Height;
            for (int y = 0; y < img.Height; ++y)
                for (int x = 0; x < img.Width; ++x)
                {
                    int v = img.Data[y, x, 0];
                    sum += v;
                    sumSq += v * v;
                }
            double mean = (double)sum / n;
            double variance = (double)sumSq / n - mean * mean;
            double stdDev = Math.Sqrt(Math.Max(0, variance));
            return (mean, stdDev);
        }

        // Color stats: per-channel mean + stdDev
        public static ((double bMean, double gMean, double rMean) mean, (double bStd, double gStd, double rStd) stdDev)
            ComputeColorStats(Image<Bgr, byte> img)
        {
            long sumB = 0, sumG = 0, sumR = 0;
            long sumB2 = 0, sumG2 = 0, sumR2 = 0;
            int n = img.Width * img.Height;
            for (int y = 0; y < img.Height; ++y)
                for (int x = 0; x < img.Width; ++x)
                {
                    int b = img.Data[y, x, 0];
                    int g = img.Data[y, x, 1];
                    int r = img.Data[y, x, 2];
                    sumB += b; sumG += g; sumR += r;
                    sumB2 += b * b; sumG2 += g * g; sumR2 += r * r;
                }
            double bMean = (double)sumB / n, gMean = (double)sumG / n, rMean = (double)sumR / n;
            double bVar = (double)sumB2 / n - bMean * bMean;
            double gVar = (double)sumG2 / n - gMean * gMean;
            double rVar = (double)sumR2 / n - rMean * rMean;
            return ((bMean, gMean, rMean), (Math.Sqrt(Math.Max(0, bVar)), Math.Sqrt(Math.Max(0, gVar)), Math.Sqrt(Math.Max(0, rVar))));
        }
        #endregion
    }
}