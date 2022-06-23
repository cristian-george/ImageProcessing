using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;
using static System.Math;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class PointwiseOperations
    {
        #region Linear operators with slope = 1

        #region Operator +
        public static int[] IncreaseBrightnessPlus(int intercept)
        {
            int[] lookupTable = new int[256];
            
            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (pixel <= 255 - intercept)
                    lookupTable[pixel] = pixel + intercept;
                else
                    lookupTable[pixel] = 255;
            }

            return lookupTable;
        }
        #endregion

        #region Operator -
        public static int[] DecreaseBrightnessMinus(int intercept)
        {
            int[] lookupTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (intercept <= pixel && pixel <= 255)
                    lookupTable[pixel] = pixel - intercept;
                else
                    lookupTable[pixel] = 0;
            }

            return lookupTable;
        }
        #endregion

        #endregion

        #region Operator * for increasing brightness and contrast
        public static int[] IncreaseBrightnessKeepBlack(double slope)
        {
            int[] lookupTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (pixel <= 255 / slope)
                    lookupTable[pixel] = (int)(slope * pixel + 0.5);
                else
                    lookupTable[pixel] = 255;
            }

            return lookupTable;
        }

        public static int[] IncreaseBrightnessKeepWhite(double slope)
        {
            int[] lookupTable = new int[256];
            int intercept = (int)(255 * (1 - slope));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                int value = (int)(slope * pixel + intercept + 0.5);
                if (value < 0) value = 0;
                else if (value > 255) value = 255;

                lookupTable[pixel] = value;
            }

            return lookupTable;
        }
        #endregion

        #region Operator * for decreasing brightness and contrast
        public static int[] DecreaseBrightnessKeepBlack(double slope)
        {
            int[] lookupTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(slope * pixel + 0.5);
            }

            return lookupTable;
        }

        public static int[] DecreaseBrightnessKeepWhite(double slope)
        {
            int[] lookupTable = new int[256];
            int intercept = (int)(255 * (slope - 1));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (intercept / slope <= pixel && pixel <= 255)
                {
                    int value = (int)(slope * pixel - intercept + 0.5);
                    if (value < 0) value = 0;
                    else if (value > 255) value = 255;

                    lookupTable[pixel] = value;
                }
                else
                    lookupTable[pixel] = 0;
            }

            return lookupTable;
        }
        #endregion

        #region Logarithmic operator for increasing brightness and contrast
        public static int[] LogarithmicOperator()
        {
            int[] lookupTable = new int[256];
            double c = 255 / Log(256);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(c * Log(pixel + 1) + 0.5);
            }

            return lookupTable;
        }

        #endregion

        #region Exponential operator for decreasing brightness and contrast
        public static int[] ExponentialOperator()
        {
            int[] lookupTable = new int[256];
            double c = 255 / Log(256);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(Exp(pixel / c) - 1 + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region Gamma operator for inc/decr brightness and contrast
        public static int[] GammaCorrection(double gamma)
        {
            int[] lookupTable = new int[256];
            double a = Pow(255, 1 - gamma);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(a * Pow(pixel, gamma) + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region Piecewise linear contrast enhancement
        public static int[] PiecewiseLinearContrast(int r1, int s1, int r2, int s2)
        {
            int[] lookupTable = new int[256];
            double alfa = (double)s1 / r1;
            double beta = (double)(s2 - s1) / (r2 - r1);
            double gamma = (double)(255 - s2) / (255 - r2);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (0 <= pixel && pixel < r1)
                {
                    lookupTable[pixel] = (int)(alfa * pixel + 0.5);
                }
                else if (r1 <= pixel && pixel < r2)
                {
                    lookupTable[pixel] = (int)(beta * (pixel - r1) + s1 + 0.5);
                }
                else if (r2 <= pixel && pixel <= 255)
                {
                    lookupTable[pixel] = (int)(gamma * (pixel - r2) + s2 + 0.5);
                }
            }

            return lookupTable;
        }
        #endregion

        #region Sinusoidal operator
        public static int[] SinusoidalOperator()
        {
            int[] lookupTable = new int[256];
            double alfa = 127.5;
            double beta = PI / 255;
            double gamma = -PI / 2;

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(alfa * (Sin(beta * pixel + gamma) + 1) + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region Polynomial operator
        public static int[] PolynomialOperator()
        {
            int[] lookupTable = new int[256];
            double a = -2 * Pow(255, -2);
            double b = Pow(85, -1);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] = (int)(Pow(pixel, 2) * (a * pixel + b) + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region EM - operator
        public static int[] EmOperator(double m, double E)
        {
            int[] lookupTable = new int[256];
            double c = 1 / 255 * Pow(m, E) * (Pow(255, E) + Pow(m, E));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] =
                (int)(255 * (Pow(pixel, E) / (Pow(pixel, E) + Pow(m, E)) + c * pixel) + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region Histogram equalization
        public static int[] HistogramEqualization(Image<Gray, byte> inputImage)
        {
            int[] lookupTable = new int[256];

            double[] cummulativeHist = CummulativeHistogram(inputImage);
            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] =
                    (int)((255 * (cummulativeHist[pixel] - cummulativeHist[0]) / (1 - cummulativeHist[0])) + 0.5);
            }

            return lookupTable;
        }
        #endregion

        #region Color histogram equalization
        public static Image<Bgr, byte> ColorHistogramEqualization(Image<Bgr, byte> inputImage)
        {
            Image<Hsv, byte> hsvImage = ConvertBgrToHsv(inputImage);

            int[] lookupTable = new int[256];
            double[] cummulativeHist = CummulativeHistogram(hsvImage);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookupTable[pixel] =
                    (int)((255 * (cummulativeHist[pixel] - cummulativeHist[0]) / (1 - cummulativeHist[0])) + 0.5);
            }

            for (int y = 0; y < hsvImage.Height; ++y)
            {
                for (int x = 0; x < hsvImage.Width; ++x)
                {
                    hsvImage.Data[y, x, 2] = (byte)lookupTable[hsvImage.Data[y, x, 2]];
                }
            }

            return ConvertHsvToBgr(hsvImage);
        }
        #endregion
    }
}
