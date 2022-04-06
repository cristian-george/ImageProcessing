using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.Tools
{
    public class Tools
    {
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

        #region Convert RGB to grayscale
        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            Image<Gray, byte> grayImage = coloredImage.Convert<Gray, byte>();
            return grayImage;
        }
        #endregion

        #region Convert RGB to HSV
        public static Image<Hsv, byte> ConvertBgrToHsv(Image<Bgr, byte> coloredImage)
        {
            Image<Hsv, byte> hsvImage = coloredImage.Convert<Hsv, byte>();
            return hsvImage;
        }
        #endregion

        #region Convert HSV to RGB
        public static Image<Bgr, byte> ConvertHsvToBgr(Image<Hsv, byte> hsvImage)
        {
            Image<Bgr, byte> coloredImage = hsvImage.Convert<Bgr, byte>();
            return coloredImage;
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

        #region Calculate mean
        public static double Mean(Image<Gray, byte> image)
        {
            double sumOfPixels = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    sumOfPixels += image.Data[y, x, 0];
                }
            }

            return sumOfPixels / (image.Height * image.Width);
        }

        public static double Mean(Image<Bgr, byte> image)
        {
            double sumOfPixels = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    sumOfPixels += (image.Data[y, x, 0] + image.Data[y, x, 1] + image.Data[y, x, 2]) / 3;
                }
            }

            return sumOfPixels / (image.Height * image.Width);
        }
        #endregion

        #region Calculate standard deviation
        public static double StandardDeviation(Image<Gray, byte> image)
        {
            double sumOfSquares = 0;
            double mean = Mean(image);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    sumOfSquares += System.Math.Pow(image.Data[y, x, 0] - mean, 2);
                }
            }

            return System.Math.Sqrt(sumOfSquares / (image.Height * image.Width));
        }

        public static double StandardDeviation(Image<Bgr, byte> image)
        {
            double sumOfSquares = 0;
            double mean = Mean(image);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    sumOfSquares += System.Math.Pow((image.Data[y, x, 0] + image.Data[y, x, 1] + image.Data[y, x, 2]) / 3 - mean, 2);
                }
            }

            return System.Math.Sqrt(sumOfSquares / (image.Height * image.Width));
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

        #region Transpose
        private static Image<Gray, byte> Transpose(Image<Gray, byte> image)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(image.Height, image.Width);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[x, y, 0] = image.Data[y, x, 0];
                }
            }

            return result;
        }

        private static Image<Bgr, byte> Transpose(Image<Bgr, byte> image)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(image.Height, image.Width);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[x, y, 0] = image.Data[y, x, 0];
                    result.Data[x, y, 1] = image.Data[y, x, 1];
                    result.Data[x, y, 2] = image.Data[y, x, 2];
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

        #region Compute histograms

        private static int[] Histogram(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    ++histogram[inputImage.Data[y, x, 0]];
                }
            }

            return histogram;
        }

        private static double[] RelativeHistogram(Image<Gray, byte> inputImage)
        {
            int resolution = inputImage.Height * inputImage.Width;
            int[] histogram = Histogram(inputImage);

            double[] relativeHistogram = new double[256];
            for (int color = 0; color < 256; ++color)
            {
                relativeHistogram[color] = (double)histogram[color] / resolution;
            }

            return relativeHistogram;
        }

        private static double[] CummulativeHistogram(Image<Gray, byte> inputImage)
        {
            double[] relativeHist = RelativeHistogram(inputImage);
            double[] cummulativeHist = new double[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                for (int j = 0; j <= pixel; ++j)
                {
                    cummulativeHist[pixel] += relativeHist[j];
                }
            }

            return cummulativeHist;
        }

        private static double GetHistogramProbability(int inf, int sup, double[] histogram)
        {
            double probability = 0;
            for (int k = inf; k <= sup; ++k)
            {
                probability += histogram[k];
            }

            return probability;
        }

        private static double GetHistogramSum(int inf, int sup, double[] histogram)
        {
            double sum = 0;
            for (int k = inf; k <= sup; ++k)
            {
                sum += k * histogram[k];
            }

            return sum;
        }
        #endregion

        #region Adjust brightness and contrast

        public static Image<Gray, byte> AdjustBrightnessAndContrast(Image<Gray, byte> inputImage, int[] lookUpTable)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)lookUpTable[inputImage.Data[y, x, 0]];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> AdjustBrightnessAndContrast(Image<Bgr, byte> inputImage, int[] lookUpTable)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)lookUpTable[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = (byte)lookUpTable[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = (byte)lookUpTable[inputImage.Data[y, x, 2]];
                }
            }

            return result;
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
        private static double[] RelativeHistogram(Image<Hsv, byte> inputImage)
        {
            int numberOfPixels = inputImage.Height * inputImage.Width;
            double[] histogram = new double[256];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    ++histogram[inputImage.Data[y, x, 2]];
                }
            }

            for (int color = 0; color < 256; ++color)
            {
                histogram[color] = histogram[color] / numberOfPixels;
            }

            return histogram;
        }

        private static double[] CummulativeHistogram(Image<Hsv, byte> inputImage)
        {
            double[] relativeHist = RelativeHistogram(inputImage);
            double[] cummulativeHist = new double[256];

            for (int pixel = 0; pixel <= 255; ++pixel)
            {
                for (int j = 0; j <= pixel; ++j)
                {
                    cummulativeHist[pixel] += relativeHist[j];
                }
            }

            return cummulativeHist;
        }

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

        #region Median filtering
        private static byte GetMedianValue(Image<Gray, byte> inputImage, int maskSize, int y, int x)
        {
            int maskRadius = maskSize / 2;
            List<byte> maskValues = new List<byte>();

            for (int row = y - maskRadius; row <= y + maskRadius; ++row)
            {
                for (int column = x - maskRadius; column <= x + maskRadius; ++column)
                {
                    maskValues.Add(inputImage.Data[row, column, 0]);
                }
            }

            maskValues.Sort();
            return maskValues[maskSize * maskSize / 2];
        }

        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> inputImage, int maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    result.Data[y, x, 0] = GetMedianValue(borderedImage, maskSize, y, x);
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }

        #endregion

        #region Fast median filtering
        private static byte GetMedianValue(int[] histogram, int middle)
        {
            byte median;
            int sum = 0;
            for (median = 0; median <= 255; ++median)
            {
                sum += histogram[median];
                if (sum >= middle)
                    break;
            }

            return median;
        }

        public static Image<Gray, byte> FastMedianFiltering(Image<Gray, byte> inputImage, int maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;
            int middle = (maskSize * maskSize + 1) / 2;
            int x;

            // Initialize mask histogram
            int[] maskHistogram = new int[256];
            for (int y = -maskRadius; y <= maskRadius; ++y)
            {
                for (x = -maskRadius; x <= maskRadius; ++x)
                {
                    byte pixel = borderedImage.Data[y + maskRadius, x + maskRadius, 0];
                    ++maskHistogram[pixel];
                }
            }

            // Initialize column histograms
            int[,] columnHistograms = new int[borderedImage.Width, 256];
            for (int col = 0; col < borderedImage.Width; ++col)
            {
                for (int row = -maskRadius; row <= maskRadius; ++row)
                {
                    byte pixel = borderedImage.Data[row + maskRadius, col, 0];
                    ++columnHistograms[col, pixel];
                }
            }

            // Initialize first row of result image
            for (x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
            {
                result.Data[maskRadius, x, 0] = GetMedianValue(maskHistogram, middle);

                if (x < borderedImage.Width - maskRadius - 1)
                {
                    for (int pixel = 0; pixel <= 255; ++pixel)
                    {
                        maskHistogram[pixel] += columnHistograms[x + maskRadius + 1, pixel];
                        maskHistogram[pixel] -= columnHistograms[x - maskRadius, pixel];
                    }
                }
            }

            int direction = -1;
            x = borderedImage.Width - maskRadius - 1;

            for (int y = maskRadius + 1; y < borderedImage.Height - maskRadius; y++)
            {
                for (int k = -maskRadius; k <= maskRadius; ++k)
                {
                    byte pixelToRemove = borderedImage.Data[y - maskRadius - 1, x + k, 0];
                    byte pixelToAdd = borderedImage.Data[y + maskRadius, x + k, 0];

                    --columnHistograms[x + k, pixelToRemove];
                    --maskHistogram[pixelToRemove];

                    ++columnHistograms[x + k, pixelToAdd];
                    ++maskHistogram[pixelToAdd];
                }

                // Right to Left
                if (direction == -1)
                {
                    while (x >= maskRadius)
                    {
                        result.Data[y, x, 0] = GetMedianValue(maskHistogram, middle);

                        if (x > maskRadius)
                        {
                            byte pixelToRemove = borderedImage.Data[y - maskRadius - 1, x - maskRadius - 1, 0];
                            byte pixelToAdd = borderedImage.Data[y + maskRadius, x - maskRadius - 1, 0];

                            --columnHistograms[x - maskRadius - 1, pixelToRemove];
                            ++columnHistograms[x - maskRadius - 1, pixelToAdd];

                            for (int pixel = 0; pixel <= 255; ++pixel)
                            {
                                maskHistogram[pixel] += columnHistograms[x - maskRadius - 1, pixel];
                                maskHistogram[pixel] -= columnHistograms[x + maskRadius, pixel];
                            }
                        }
                        x += direction;
                    }

                    ++x;
                }
                // Left to Right
                else if (direction == 1)
                {
                    while (x < borderedImage.Width - maskRadius)
                    {
                        result.Data[y, x, 0] = GetMedianValue(maskHistogram, middle);

                        if (x < borderedImage.Width - maskRadius - 1)
                        {
                            byte pixelToRemove = borderedImage.Data[y - maskRadius - 1, x + maskRadius + 1, 0];
                            byte pixelToAdd = borderedImage.Data[y + maskRadius, x + maskRadius + 1, 0];

                            --columnHistograms[x + maskRadius + 1, pixelToRemove];
                            ++columnHistograms[x + maskRadius + 1, pixelToAdd];

                            for (int pixel = 0; pixel <= 255; ++pixel)
                            {
                                maskHistogram[pixel] += columnHistograms[x + maskRadius + 1, pixel];
                                maskHistogram[pixel] -= columnHistograms[x - maskRadius, pixel];
                            }
                        }

                        x += direction;
                    }

                    --x;
                }

                direction *= -1;
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }
        #endregion

        #region Bilateral gaussian filtering
        private static double MedianMask(int i, int j, double variance_d)
        {
            return System.Math.Exp(-((i * i) + (j * j)) / (2 * variance_d * variance_d));
        }

        private static double RangeMask(Image<Gray, byte> image, int y, int x, int i, int j, double variance_r)
        {
            int value = image.Data[y + i, x + j, 0] - image.Data[y, x, 0];
            return System.Math.Exp(-(value * value) / (2 * variance_r * variance_r));
        }

        public static Image<Gray, byte> GaussianBilateralFiltering(Image<Gray, byte> inputImage, double variance_d, double variance_r)
        {
            int maskSize = (int)System.Math.Ceiling(4 * variance_d);
            if (maskSize % 2 == 0) maskSize++;

            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;
            double[,] medianMask = new double[maskSize, maskSize];

            for (int i = -maskRadius; i <= maskRadius; ++i)
            {
                for (int j = -maskRadius; j <= maskRadius; ++j)
                {
                    medianMask[i + maskRadius, j + maskRadius] = MedianMask(i, j, variance_d);
                }
            }

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    double numerator = 0, denominator = 0;
                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            double median = medianMask[i + maskRadius, j + maskRadius];
                            double range = RangeMask(borderedImage, y, x, i, j, variance_r);
                            double filter = median * range;

                            numerator += borderedImage.Data[y + i, x + j, 0] * filter;
                            denominator += filter;
                        }
                    }

                    int pixel = (int)(numerator / denominator);
                    if (pixel < 0) pixel = 0;
                    else if (pixel > 255) pixel = 255;

                    result.Data[y, x, 0] = (byte)pixel;
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }

        #endregion

        #region Sobel on grayscale
        public static Image<Gray, byte> SobelGradient(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> gradient = new Image<Gray, byte>(inputImage.Size);

            for (int y = 1; y < inputImage.Height - 1; ++y)
            {
                for (int x = 1; x < inputImage.Width - 1; ++x)
                {
                    int fx = inputImage.Data[y + 1, x - 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y + 1, x, 0] -
                               2 * inputImage.Data[y - 1, x, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y - 1, x + 1, 0];

                    int fy = inputImage.Data[y - 1, x + 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y, x + 1, 0] -
                               2 * inputImage.Data[y, x - 1, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y + 1, x - 1, 0];

                    double grad = System.Math.Sqrt((fx * fx) + (fy * fy));

                    if (grad > 255) grad = 255;
                    gradient.Data[y, x, 0] = (byte)grad;
                }
            }

            return gradient;
        }

        #endregion
    }
}