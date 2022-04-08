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
    }
}
