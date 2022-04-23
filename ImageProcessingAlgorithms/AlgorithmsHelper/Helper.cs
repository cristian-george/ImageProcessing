using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.AlgorithmsHelper
{
    public class Helper
    {
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

        #region Transpose
        public static Image<Gray, byte> Transpose(Image<Gray, byte> image)
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

        public static Image<Bgr, byte> Transpose(Image<Bgr, byte> image)
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

        #region Compute histograms
        public static int[] Histogram(Image<Gray, byte> inputImage)
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

        public static double[] RelativeHistogram(Image<Gray, byte> inputImage)
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

        public static double[] CummulativeHistogram(Image<Gray, byte> inputImage)
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

        public static double GetHistogramProbability(int inf, int sup, double[] histogram)
        {
            double probability = 0;
            for (int k = inf; k <= sup; ++k)
            {
                probability += histogram[k];
            }

            return probability;
        }

        public static double GetHistogramSum(int inf, int sup, double[] histogram)
        {
            double sum = 0;
            for (int k = inf; k <= sup; ++k)
            {
                sum += k * histogram[k];
            }

            return sum;
        }

        public static byte GetMedianValue(Image<Gray, byte> inputImage, int maskSize, int y, int x)
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

        public static byte GetMedianValue(int[] histogram, int middle)
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

        public static double[] RelativeHistogram(Image<Hsv, byte> inputImage)
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

        public static double[] CummulativeHistogram(Image<Hsv, byte> inputImage)
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
        #endregion

        #region Integral image
        public static int[,] IntegralImage(Image<Gray, byte> inputImage)
        {
            int[,] integralImage = new int[inputImage.Height, inputImage.Width];

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (y == 0 && x == 0)
                    {
                        integralImage[y, x] = inputImage.Data[0, 0, 0];
                    }
                    else if (y == 0 && x != 0)
                    {
                        integralImage[y, x] = integralImage[0, x - 1] + inputImage.Data[0, x, 0];
                    }
                    else if (y != 0 && x == 0)
                    {
                        integralImage[y, x] = integralImage[y - 1, 0] + inputImage.Data[y, 0, 0];
                    }
                    else
                    {
                        integralImage[y, x] = integralImage[y - 1, x] + integralImage[y, x - 1] - integralImage[y - 1, x - 1] + inputImage.Data[y, x, 0];
                    }
                }
            }

            return integralImage;
        }

        #endregion

        #region Calculate sum of values in a rectangular subset of a grid from the image
        public static int SumArea(Image<Gray, byte> inputImage, int y0, int x0, int y1, int x1)
        {
            int[,] integralImage = IntegralImage(inputImage);

            if (x0 == 0 && y0 == 0)
            {
                return integralImage[y1, x1];
            }
            else if (y0 == 0)
            {
                return integralImage[y1, x1] - integralImage[y0 - 1, x1];
            }
            else if (x0 == 0)
            {
                return integralImage[y1, x1] - integralImage[y1, x0 - 1];
            }

            return integralImage[y1, x1] + integralImage[y0 - 1, x0 - 1] - integralImage[y0 - 1, x1] - integralImage[y1, x0 - 1];
        }
        #endregion

        #region Calculate mean using integral image
        public static int MeanArea(Image<Gray, byte> inputImage, int y0, int x0, int y1, int x1)
        {
            int resolution = (y1 - y0) * (x1 - x0);
            int sumArea = SumArea(inputImage, y0, x0, y1, x1);
            return sumArea / resolution;
        }
        #endregion

        #region Verify if an image is binary
        public static bool IsBinaryImage(Image<Gray, byte> inputImage)
        {
            bool isBinary = true;

            for (int y = 0; y < inputImage.Height && isBinary == true; y++)
            {
                for (int x = 0; x < inputImage.Width && isBinary == true; x++)
                {
                    if (inputImage.Data[y, x, 0] != 0 && inputImage.Data[y, x, 0] != 255)
                        isBinary = false;
                }
            }

            return isBinary;
        }
        #endregion

        #region Swap method
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            (rhs, lhs) = (lhs, rhs);
        }

        #endregion

        #region Generate random Bgr color

        public static (byte, byte, byte) GenerateRandomBgr(System.Random random)
        {
            int max = byte.MaxValue + 1;
            int r = random.Next(max);
            int g = random.Next(max);
            int b = random.Next(max);

            return ((byte)b, (byte)g, (byte)r);
        }

        #endregion
    }
}
