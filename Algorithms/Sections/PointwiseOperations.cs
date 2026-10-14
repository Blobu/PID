using System.Drawing;
using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        #region Contrast & Brightness
        public static byte[] ContrastAndBrightness(double a, double b)
        {
            byte[] LUT = new byte[256];
            for (int r= 0; r < 256; r++)
            {
                double s = a * r + b;
                s = Math.Max(0, Math.Min(255, s));
                s = Math.Round(s);
                LUT[r] = (byte)s;
            }
            return LUT;
        }

        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, byte[] LUT)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    outputImage.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                }
            }
            return outputImage;
        }

        public static Image<Bgr, byte> ApplyLUT(Image<Bgr, byte> inputImage, byte[] LUT)
        {
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(inputImage.Width, inputImage.Height);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    outputImage.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]]; // Blue channel
                    outputImage.Data[y, x, 1] = LUT[inputImage.Data[y, x, 1]]; // Green channel
                    outputImage.Data[y, x, 2] = LUT[inputImage.Data[y, x, 2]]; // Red channel
                }
            }
            return outputImage;
        }

    
        #endregion
    }
}