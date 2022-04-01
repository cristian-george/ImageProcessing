using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        #region Convert
        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            Image<Gray, byte> grayImage = coloredImage.Convert<Gray, byte>();

            return grayImage;
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

        #region Crop
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

        #region Cubic Hermite spline
        public static Image<Gray, byte> HermiteSplineInterpolation(Image<Gray, byte> inputImage, Collection<byte> lookUpTable)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = lookUpTable[inputImage.Data[y, x, 0]];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> HermiteSplineInterpolation(Image<Bgr, byte> inputImage, Collection<byte> lookUpTable)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = lookUpTable[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = lookUpTable[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = lookUpTable[inputImage.Data[y, x, 2]];
                }
            }

            return result;
        }
        #endregion

        #region Compute histogram
        private static double[] GetHistogram(Image<Gray, byte> inputImage)
        {
            int numberOfPixels = inputImage.Height * inputImage.Width;
            double[] histogram = new double[256];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    ++histogram[inputImage.Data[y, x, 0]];
                }
            }

            for (int color = 0; color < 256; color++)
            {
                histogram[color] = histogram[color] / numberOfPixels;
            }

            return histogram;
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

        #region Otsu two-threshold
        private static System.Tuple<int, int> GetThresholdValues(Image<Gray, byte> inputImage)
        {
            double[] histogram = GetHistogram(inputImage);
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

            return CropImage(result, maskSize / 2, maskSize / 2, inputImage.Width + maskSize / 2, inputImage.Height + maskSize / 2); ;
        }

        #endregion

        #region Fast median filtering

        private static byte GetMedianValue(int[] maskHistogram, int middle)
        {
            byte median;
            int sum = 0;
            for (median = 0; median <= 255; ++median)
            {
                sum += maskHistogram[median];
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

            return CropImage(result, maskSize / 2, maskSize / 2, inputImage.Width + maskSize / 2, inputImage.Height + maskSize / 2);
        }
        #endregion
    }
}