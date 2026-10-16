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

        public static byte[] ContrastAndBrightnessGammaOperator(double gamma)
        {
            byte[] LUT = new byte[256];
            double c = Math.Pow(255, 1 - gamma);
            for (int r = 0; r < 256; r++)
            {
                double s = c * Math.Pow(r, gamma);
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

        #region Normalize Histogram
         public static double[] NormalizeHistogram(Image<Gray, byte> inputImage)
        {
            double N = inputImage.Width * inputImage.Height;
            double[] histogram = new double[256];
            double[] p = new double[256];
            
            for(int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    histogram[inputImage.Data[y, x, 0]]++;
                }
            }

            for(int r = 0; r < 256; ++r)
            {
                p[r] = histogram[r] / N;
            }

            return p;
        }

        public static double[][] NormalizeHistogram(Image<Bgr, byte> inputImage)
        {
            double N = inputImage.Width * inputImage.Height;
            double[] histogramB = new double[256];
            double[] histogramG = new double[256];
            double[] histogramR = new double[256];
            double[] pB = new double[256];
            double[] pG = new double[256];
            double[] pR = new double[256];
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    histogramB[inputImage.Data[y, x, 0]]++;
                    histogramG[inputImage.Data[y, x, 1]]++;
                    histogramR[inputImage.Data[y, x, 2]]++;
                }
            }
            for (int r = 0; r < 256; ++r)
            {
                pB[r] = histogramB[r] / N;
                pG[r] = histogramG[r] / N;
                pR[r] = histogramR[r] / N;
            }
            return new double[][] { pB, pG, pR };
        }

        #endregion
    }
}