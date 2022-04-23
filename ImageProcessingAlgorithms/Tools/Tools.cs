using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingAlgorithms.AlgorithmsHelper;
using System.Collections.Generic;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;

namespace ImageProcessingAlgorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Bgr, byte> Copy(Image<Bgr, byte> image)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(image.Size);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[y, x, 0] = image.Data[y, x, 0];
                }
            }
            return image;
        }

        public static Image<Gray, byte> Copy(Image<Gray, byte> image)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(image.Size);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[y, x, 0] = image.Data[y, x, 0];
                }
            }
            return image;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert RGB to grayscale
        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            Image<Gray, byte> grayImage = coloredImage.Convert<Gray, byte>();
            return grayImage;
        }
        #endregion

        #region Crop image
        public static Image<Gray, byte> CropImage(Image<Gray, byte> inputImage, int leftTopX, int leftTopY, int rightBottomX, int rightBottomY)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(rightBottomX - leftTopX, rightBottomY - leftTopY);

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[y + leftTopY, x + leftTopX, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> CropImage(Image<Bgr, byte> inputImage, int leftTopX, int leftTopY, int rightBottomX, int rightBottomY)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(rightBottomX - leftTopX, rightBottomY - leftTopY);

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[y + leftTopY, x + leftTopX, 0];
                    result.Data[y, x, 1] = inputImage.Data[y + leftTopY, x + leftTopX, 1];
                    result.Data[y, x, 2] = inputImage.Data[y + leftTopY, x + leftTopX, 2];
                }
            }

            return result;
        }
        #endregion

        #region Mirroring
        public static Image<Gray, byte> MirrorVertically(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - x - 1, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> MirrorVertically(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - x - 1, 0];
                    result.Data[y, x, 1] = inputImage.Data[y, inputImage.Width - x - 1, 1];
                    result.Data[y, x, 2] = inputImage.Data[y, inputImage.Width - x - 1, 2];
                }
            }

            return result;
        }

        public static Image<Gray, byte> MirrorHorizontally(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[inputImage.Height - y - 1, x, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> MirrorHorizontally(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[inputImage.Height - y - 1, x, 0];
                    result.Data[y, x, 1] = inputImage.Data[inputImage.Height - y - 1, x, 1];
                    result.Data[y, x, 2] = inputImage.Data[inputImage.Height - y - 1, x, 2];
                }
            }

            return result;
        }
        #endregion

        #region Rotate
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> transpose = Transpose(inputImage);
            return MirrorVertically(transpose);
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> transpose = Transpose(inputImage);
            return MirrorVertically(transpose);
        }

        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> transpose = Transpose(inputImage);
            return MirrorHorizontally(transpose);
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> transpose = Transpose(inputImage);
            return MirrorHorizontally(transpose);
        }
        #endregion

        #region Replicate padding
        public static Image<Gray, byte> BorderReplicate(Image<Gray, byte> inputImage, int thickness)
        {
            Image<Gray, byte> borderedImage = new Image<Gray, byte>(inputImage.Width + thickness, inputImage.Height + thickness);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    borderedImage.Data[y + thickness / 2, x + thickness / 2, 0] = inputImage.Data[y, x, 0];
                }
            }

            for (int y = 0; y < thickness / 2 + 1; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    borderedImage.Data[y, x + thickness / 2, 0] = inputImage.Data[0, x, 0];
                    borderedImage.Data[borderedImage.Height - y - 1, x + thickness / 2, 0] = inputImage.Data[inputImage.Height - 1, x, 0];
                }
            }

            for (int x = 0; x < thickness / 2 + 1; ++x)
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    borderedImage.Data[y + thickness / 2, x, 0] = inputImage.Data[y, 0, 0];
                    borderedImage.Data[y + thickness / 2, borderedImage.Width - x - 1, 0] = inputImage.Data[y, inputImage.Width - 1, 0];
                }
            }

            byte topLeft = inputImage.Data[0, 0, 0];

            for (int y = 0; y < thickness / 2; ++y)
            {
                for (int x = 0; x < thickness / 2; ++x)
                {
                    borderedImage.Data[y, x, 0] = topLeft;
                }
            }

            byte topRight = inputImage.Data[0, inputImage.Width - 1, 0];

            for (int y = 0; y < thickness / 2; ++y)
            {
                for (int x = inputImage.Width + thickness / 2; x < borderedImage.Width; ++x)
                {
                    borderedImage.Data[y, x, 0] = topRight;
                }
            }

            byte bottomLeft = inputImage.Data[inputImage.Height - 1, 0, 0];

            for (int y = inputImage.Height + thickness / 2; y < borderedImage.Height; ++y)
            {
                for (int x = 0; x < thickness / 2; ++x)
                {
                    borderedImage.Data[y, x, 0] = bottomLeft;
                }
            }

            byte bottomRight = inputImage.Data[inputImage.Height - 1, inputImage.Width - 1, 0];

            for (int y = inputImage.Height + thickness / 2; y < borderedImage.Height; ++y)
            {
                for (int x = inputImage.Width + thickness / 2; x < borderedImage.Width; ++x)
                {
                    borderedImage.Data[y, x, 0] = bottomRight;
                }
            }

            return borderedImage;
        }

        public static Image<Bgr, byte> BorderReplicate(Image<Bgr, byte> inputImage, int thickness)
        {
            Image<Bgr, byte> borderedImage = new Image<Bgr, byte>(inputImage.Width + thickness, inputImage.Height + thickness);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    borderedImage.Data[y + thickness / 2, x + thickness / 2, 0] = inputImage.Data[y, x, 0];
                    borderedImage.Data[y + thickness / 2, x + thickness / 2, 1] = inputImage.Data[y, x, 1];
                    borderedImage.Data[y + thickness / 2, x + thickness / 2, 2] = inputImage.Data[y, x, 2];
                }
            }

            for (int y = 0; y < thickness / 2 + 1; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    borderedImage.Data[y, x + thickness / 2, 0] = inputImage.Data[0, x, 0];
                    borderedImage.Data[y, x + thickness / 2, 1] = inputImage.Data[0, x, 1];
                    borderedImage.Data[y, x + thickness / 2, 2] = inputImage.Data[0, x, 2];

                    borderedImage.Data[borderedImage.Height - y - 1, x + thickness / 2, 0] = inputImage.Data[inputImage.Height - 1, x, 0];
                    borderedImage.Data[borderedImage.Height - y - 1, x + thickness / 2, 1] = inputImage.Data[inputImage.Height - 1, x, 1];
                    borderedImage.Data[borderedImage.Height - y - 1, x + thickness / 2, 2] = inputImage.Data[inputImage.Height - 1, x, 2];
                }
            }

            for (int x = 0; x < thickness / 2 + 1; ++x)
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    borderedImage.Data[y + thickness / 2, x, 0] = inputImage.Data[y, 0, 0];
                    borderedImage.Data[y + thickness / 2, x, 1] = inputImage.Data[y, 0, 1];
                    borderedImage.Data[y + thickness / 2, x, 2] = inputImage.Data[y, 0, 2];

                    borderedImage.Data[y + thickness / 2, borderedImage.Width - x - 1, 0] = inputImage.Data[y, inputImage.Width - 1, 0];
                    borderedImage.Data[y + thickness / 2, borderedImage.Width - x - 1, 1] = inputImage.Data[y, inputImage.Width - 1, 1];
                    borderedImage.Data[y + thickness / 2, borderedImage.Width - x - 1, 2] = inputImage.Data[y, inputImage.Width - 1, 2];
                }
            }

            for (int y = 0; y < thickness / 2; ++y)
            {
                for (int x = 0; x < thickness / 2; ++x)
                {
                    borderedImage.Data[y, x, 0] = inputImage.Data[0, 0, 0];
                    borderedImage.Data[y, x, 1] = inputImage.Data[0, 0, 1];
                    borderedImage.Data[y, x, 2] = inputImage.Data[0, 0, 2];
                }
            }

            for (int y = 0; y < thickness / 2; ++y)
            {
                for (int x = inputImage.Width + thickness / 2; x < borderedImage.Width; ++x)
                {
                    borderedImage.Data[y, x, 0] = inputImage.Data[0, inputImage.Width - 1, 0];
                    borderedImage.Data[y, x, 1] = inputImage.Data[0, inputImage.Width - 1, 1];
                    borderedImage.Data[y, x, 2] = inputImage.Data[0, inputImage.Width - 1, 2];
                }
            }

            for (int y = inputImage.Height + thickness / 2; y < borderedImage.Height; ++y)
            {
                for (int x = 0; x < thickness / 2; ++x)
                {
                    borderedImage.Data[y, x, 0] = inputImage.Data[inputImage.Height - 1, 0, 0];
                    borderedImage.Data[y, x, 1] = inputImage.Data[inputImage.Height - 1, 0, 1];
                    borderedImage.Data[y, x, 2] = inputImage.Data[inputImage.Height - 1, 0, 2];
                }
            }

            for (int y = inputImage.Height + thickness / 2; y < borderedImage.Height; ++y)
            {
                for (int x = inputImage.Width + thickness / 2; x < borderedImage.Width; ++x)
                {
                    borderedImage.Data[y, x, 0] = inputImage.Data[inputImage.Height - 1, inputImage.Width - 1, 0];
                    borderedImage.Data[y, x, 1] = inputImage.Data[inputImage.Height - 1, inputImage.Width - 1, 1];
                    borderedImage.Data[y, x, 2] = inputImage.Data[inputImage.Height - 1, inputImage.Width - 1, 2];
                }
            }

            return borderedImage;
        }

        #endregion

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

        #region Thresholding
        public static Image<Gray, byte> Thresholding(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (threshold <= inputImage.Data[y, x, 0])
                        result.Data[y, x, 0] = 255;
                }
            }
            return result;
        }
        #endregion

        #region Quantile threshold
        public static int QuantileThreshold(Image<Gray, byte> inputImage, double percent)
        {
            int resolution = inputImage.Height * inputImage.Width;
            int[] histogram = Histogram(inputImage);

            int threshold = 0;
            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                int sum = 0;
                for (int k = 0; k <= pixel; ++k)
                {
                    sum += histogram[k];
                }

                if (sum >= resolution * percent)
                {
                    threshold = pixel;
                    break;
                }
            }

            return threshold;
        }
        #endregion

        #region Median threshold
        public static int MedianThreshold(Image<Gray, byte> inputImage)
        {
            // return QuantileThreshold(inputImage, 0.5);

            int[] histogram = Histogram(inputImage);
            int resolution = inputImage.Height * inputImage.Width;

            int threshold = GetMedianValue(histogram, resolution / 2);
            return threshold;
        }
        #endregion

        #region Intermeans threshold
        public static int IntermeansThreshold(Image<Gray, byte> inputImage)
        {
            return 0;
        }
        #endregion

        #region Otsu two-threshold
        private static System.Tuple<int, int> GetThresholdValues(Image<Gray, byte> inputImage)
        {
            double[] histogram = RelativeHistogram(inputImage);
            double mu = Mean(inputImage);

            double maxInterclassVariance = 0;
            int maxInterclassVarianceFrequence = 0;

            int threshold1 = -1, threshold2 = -1;
            for (int k1 = 1; k1 < 254; ++k1)
            {
                double P1 = GetHistogramProbability(0, k1, histogram);
                double mu1 = GetHistogramSum(0, k1, histogram);
                if (P1 != 0)
                    mu1 /= P1;

                for (int k2 = k1 + 1; k2 < 255; ++k2)
                {
                    double P2 = GetHistogramProbability(k1 + 1, k2, histogram);
                    double mu2 = GetHistogramSum(k1 + 1, k2, histogram);
                    if (P2 != 0)
                        mu2 /= P2;

                    double P3 = 1 - P1 - P2;
                    double mu3 = GetHistogramSum(k2 + 1, 255, histogram);
                    if (P3 != 0)
                        mu3 /= P3;

                    double interclassVariance =
                        (P1 * System.Math.Pow(mu1 - mu, 2)) +
                        (P2 * System.Math.Pow(mu2 - mu, 2)) +
                        (P3 * System.Math.Pow(mu3 - mu, 2));

                    if (interclassVariance > maxInterclassVariance)
                    {
                        maxInterclassVariance = interclassVariance;
                        threshold1 = k1;
                        threshold2 = k2;
                        maxInterclassVarianceFrequence = 1;
                    }
                    else if (interclassVariance == maxInterclassVariance)
                    {
                        threshold1 += k1;
                        threshold2 += k2;
                        ++maxInterclassVarianceFrequence;
                    }
                }
            }

            threshold1 /= maxInterclassVarianceFrequence;
            threshold2 /= maxInterclassVarianceFrequence;

            return new System.Tuple<int, int>(threshold1, threshold2);
        }

        public static Image<Gray, byte> OtsuTwoThreshold(Image<Gray, byte> inputImage)
        {
            System.Tuple<int, int> thresholdValues = GetThresholdValues(inputImage);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] > thresholdValues.Item1 &&
                        inputImage.Data[y, x, 0] <= thresholdValues.Item2)
                    {
                        result.Data[y, x, 0] = 128;
                    }
                    else if (inputImage.Data[y, x, 0] > thresholdValues.Item2)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                }
            }

            return result;
        }
        #endregion

        #region Dilation
        public static Image<Gray, byte> Dilation(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int maskRad = maskDim / 2;

            for (int y = maskRad; y < inputImage.Height - maskRad; y++)
            {
                for (int x = maskRad; x < inputImage.Width - maskRad; x++)
                {
                    if (inputImage.Data[y, x, 0] == 255)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                    else
                    {
                        bool isDilated = false;
                        for (int i = y - maskRad; i <= y + maskRad && isDilated == false; ++i)
                        {
                            for (int j = x - maskRad; j <= x + maskRad && isDilated == false; ++j)
                            {
                                if (inputImage.Data[i, j, 0] == 255)
                                {
                                    result.Data[y, x, 0] = 255;
                                    isDilated = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Connected components using Disjoint Sets

        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage)
        {
            int[,] labels = new int[inputImage.Height, inputImage.Width];

            DisjointSet<int> components = new DisjointSet<int>();

            int[] dy = { 0, -1, -1, -1 };
            int[] dx = { -1, -1, 0, 1 };

            int regionCounter = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] != 0)
                    {
                        int mainLabel = int.MaxValue;
                        HashSet<int> neighbours = new HashSet<int>();

                        for (int i = 0; i < 4; ++i)
                        {
                            int Y = y + dy[i];
                            int X = x + dx[i];

                            if (Y >= 0 && Y < inputImage.Height && X >= 0 && X < inputImage.Width)
                            {
                                int secondaryLabel = labels[Y, X];
                                if (secondaryLabel != 0)
                                {
                                    neighbours.Add(secondaryLabel);

                                    if (secondaryLabel < mainLabel)
                                        mainLabel = secondaryLabel;
                                }
                            }
                        }

                        if (mainLabel == int.MaxValue)
                        {
                            ++regionCounter;
                            components.MakeSet(regionCounter);
                            labels[y, x] = regionCounter;
                        }
                        else
                        {
                            labels[y, x] = mainLabel;

                            if (neighbours.Count > 1)
                            {
                                foreach (var label in neighbours)
                                {
                                    components.UnionSets(mainLabel, label);
                                }
                            }
                        }
                    }
                }
            }

            HashSet<int> usedLabels = new HashSet<int>();

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int currentLabel = labels[y, x];
                    if (currentLabel != 0)
                    {
                        labels[y, x] = components.FindSet(currentLabel);
                        usedLabels.Add(currentLabel);
                    }
                }
            }

            Dictionary<int, (byte, byte, byte)> colors = new Dictionary<int, (byte, byte, byte)>();
            System.Random random = new System.Random();

            foreach (var label in usedLabels)
            {
                colors[label] = Helper.GenerateRandomBgr(random);
            }

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int currentLabel = labels[y, x];
                    if (currentLabel != 0)
                    {
                        result.Data[y, x, 0] = colors[currentLabel].Item1;
                        result.Data[y, x, 1] = colors[currentLabel].Item2;
                        result.Data[y, x, 2] = colors[currentLabel].Item3;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}