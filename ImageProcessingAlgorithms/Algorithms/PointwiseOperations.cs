using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class PointwiseOperations
    {
        #region Operator +
        public static int[] IncreaseBrightnessPlus(int b)
        {
            int[] lookUpTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (pixel <= 255 - b)
                    lookUpTable[pixel] = pixel + b;
                else
                    lookUpTable[pixel] = 255;
            }

            return lookUpTable;
        }
        #endregion

        #region Operator -
        public static int[] DecreaseBrightnessMinus(int b)
        {
            int[] lookUpTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (b <= pixel && pixel <= 255)
                    lookUpTable[pixel] = pixel - b;
                else
                    lookUpTable[pixel] = 0;
            }

            return lookUpTable;
        }
        #endregion

        #region Operator * for increasing brightness and contrast
        public static int[] IncreaseBrightnessKeepBlack(double a)
        {
            int[] lookUpTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (pixel <= 255 / a)
                    lookUpTable[pixel] = (int)(a * pixel + 0.5);
                else
                    lookUpTable[pixel] = 255;
            }

            return lookUpTable;
        }

        public static int[] IncreaseBrightnessKeepWhite(double a)
        {
            int[] lookUpTable = new int[256];
            int b = (int)(255 * (1 - a));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                int value = (int)(a * pixel + b + 0.5);
                if (value < 0) value = 0;
                else if (value > 255) value = 255;

                lookUpTable[pixel] = value;
            }

            return lookUpTable;
        }
        #endregion

        #region Operator * for decreasing brightness and contrast
        public static int[] DecreaseBrightnessKeepBlack(double a)
        {
            int[] lookUpTable = new int[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(a * pixel + 0.5);
            }

            return lookUpTable;
        }

        public static int[] DecreaseBrightnessKeepWhite(double a)
        {
            int[] lookUpTable = new int[256];
            int b = (int)(255 * (a - 1));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (b / a <= pixel && pixel <= 255)
                {
                    int value = (int)(a * pixel - b + 0.5);
                    if (value < 0) value = 0;
                    else if (value > 255) value = 255;

                    lookUpTable[pixel] = value;
                }
                else
                    lookUpTable[pixel] = 0;
            }

            return lookUpTable;
        }
        #endregion

        #region Log operator for increasing brightness and contrast
        public static int[] LogarithmicOperator()
        {
            int[] lookUpTable = new int[256];
            double c = 255 / System.Math.Log(256);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(c * System.Math.Log(pixel + 1) + 0.5);
            }

            return lookUpTable;
        }

        #endregion

        #region InverseLog operator for decreasing brightness and contrast
        public static int[] ExponentialOperator()
        {
            int[] lookUpTable = new int[256];
            double c = 255 / System.Math.Log(256);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(System.Math.Exp(pixel / c) - 1 + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region Gamma operator for inc/decr brightness and contrast
        public static int[] GammaCorrection(double gamma)
        {
            int[] lookUpTable = new int[256];
            double a = System.Math.Pow(255, 1 - gamma);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(a * System.Math.Pow(pixel, gamma) + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region Piecewise linear contrast enhancement
        public static int[] PiecewiseLinearContrast(int r1, int s1, int r2, int s2)
        {
            int[] lookUpTable = new int[256];
            double alfa = (double)s1 / r1;
            double beta = (double)(s2 - s1) / (r2 - r1);
            double gamma = (double)(255 - s2) / (255 - r2);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                if (0 <= pixel && pixel < r1)
                {
                    lookUpTable[pixel] = (int)(alfa * pixel + 0.5);
                }
                else if (r1 <= pixel && pixel < r2)
                {
                    lookUpTable[pixel] = (int)(beta * (pixel - r1) + s1 + 0.5);
                }
                else if (r2 <= pixel && pixel <= 255)
                {
                    lookUpTable[pixel] = (int)(gamma * (pixel - r2) + s2 + 0.5);
                }
            }

            return lookUpTable;
        }
        #endregion

        #region Sinusoidal operator
        public static int[] SinusoidalOperator()
        {
            int[] lookUpTable = new int[256];
            double alfa = 127.5;
            double beta = System.Math.PI / 255;
            double gamma = -System.Math.PI / 2;

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(alfa * (System.Math.Sin(beta * pixel + gamma) + 1) + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region Polynomial operator
        public static int[] PolynomialOperator()
        {
            int[] lookUpTable = new int[256];
            double a = -2 * System.Math.Pow(255, -2);
            double b = System.Math.Pow(85, -1);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] = (int)(System.Math.Pow(pixel, 2) * (a * pixel + b) + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region EM - operator
        public static int[] EmOperator(double m, double E)
        {
            int[] lookUpTable = new int[256];
            double c = 1 / 255 * System.Math.Pow(m, E) * (System.Math.Pow(255, E) + System.Math.Pow(m, E));

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] =
                (int)(255 * (System.Math.Pow(pixel, E) / (System.Math.Pow(pixel, E) + System.Math.Pow(m, E)) + c * pixel) + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region Histogram equalization
        public static int[] HistogramEqualization(Image<Gray, byte> inputImage)
        {
            int[] lookUpTable = new int[256];

            double[] cummulativeHist = CummulativeHistogram(inputImage);
            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] =
                    (int)((255 * (cummulativeHist[pixel] - cummulativeHist[0]) / (1 - cummulativeHist[0])) + 0.5);
            }

            return lookUpTable;
        }
        #endregion

        #region Color histogram equalization
        public static Image<Bgr, byte> ColorHistogramEqualization(Image<Bgr, byte> inputImage)
        {
            Image<Hsv, byte> hsvImage = ConvertBgrToHsv(inputImage);

            int[] lookUpTable = new int[256];
            double[] cummulativeHist = CummulativeHistogram(hsvImage);

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                lookUpTable[pixel] =
                    (int)((255 * (cummulativeHist[pixel] - cummulativeHist[0]) / (1 - cummulativeHist[0])) + 0.5);
            }

            for (int y = 0; y < hsvImage.Height; ++y)
            {
                for (int x = 0; x < hsvImage.Width; ++x)
                {
                    hsvImage.Data[y, x, 2] = (byte)lookUpTable[hsvImage.Data[y, x, 2]];
                }
            }

            return ConvertHsvToBgr(hsvImage);
        }
        #endregion
    }
}
