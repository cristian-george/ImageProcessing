using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.Tools.Tools;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.Filters
{
    public class Filters
    {
        #region Low-pass filters

        #region Median filtering
        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> inputImage, int maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize - 1);
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
        public static Image<Gray, byte> FastMedianFiltering(Image<Gray, byte> inputImage, int maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize - 1);
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

        #region Vector median filtering
        public static Image<Bgr, byte> VectorMedianFiltering(Image<Bgr, byte> inputImage, int maskSize)
        {
            Image<Bgr, byte> borderedImage = BorderReplicate(inputImage, maskSize - 1);
            Image<Bgr, byte> result = new Image<Bgr, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;
            int size = maskSize * maskSize;

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    List<System.Tuple<int, int, int>> neighbourhood = new List<System.Tuple<int, int, int>>();

                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            int blue = borderedImage.Data[y + i, x + j, 0];
                            int green = borderedImage.Data[y + i, x + j, 1];
                            int red = borderedImage.Data[y + i, x + j, 2];

                            System.Tuple<int, int, int> color = new System.Tuple<int, int, int>(blue, green, red);
                            neighbourhood.Add(color);
                        }
                    }

                    double[,] distances = new double[size, size];

                    double minSumOnRows = double.MaxValue;
                    int pixel = 0;

                    for (int i = 0; i < size - 1; ++i)
                    {
                        for (int j = i + 1; j < size; ++j)
                        {
                            int blue = (int)System.Math.Pow(neighbourhood[i].Item1 - neighbourhood[j].Item1, 2);
                            int green = (int)System.Math.Pow(neighbourhood[i].Item2 - neighbourhood[j].Item2, 2);
                            int red = (int)System.Math.Pow(neighbourhood[i].Item3 - neighbourhood[j].Item3, 2);

                            distances[i, j] = distances[j, i] = System.Math.Sqrt(blue + green + red);
                        }
                    }

                    for (int i = 0; i < size; ++i)
                    {
                        double sumOnRow = 0;

                        for (int j = 0; j < size; ++j)
                        {
                            sumOnRow += distances[i, j];
                        }

                        if (sumOnRow < minSumOnRows)
                        {
                            minSumOnRows = sumOnRow;
                            pixel = i;
                        }
                    }

                    result.Data[y, x, 0] = (byte)neighbourhood[pixel].Item1;
                    result.Data[y, x, 1] = (byte)neighbourhood[pixel].Item2;
                    result.Data[y, x, 2] = (byte)neighbourhood[pixel].Item3;
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }
        #endregion

        #region Gaussian filtering
        private static double[,] GaussianMask(int maskRadius, double variance)
        {
            double[,] gaussianMask = new double[(2 * maskRadius) + 1, (2 * maskRadius) + 1];
            double maskCoefficient = 0;

            for (int i = -maskRadius; i <= maskRadius; ++i)
            {
                for (int j = -maskRadius; j <= maskRadius; ++j)
                {
                    gaussianMask[i + maskRadius, j + maskRadius] = System.Math.Exp(-(i * i + j * j) / (2 * variance * variance)) / (2 * System.Math.PI * variance * variance);
                    maskCoefficient += gaussianMask[i + maskRadius, j + maskRadius];
                }
            }

            for (int i = -maskRadius; i <= maskRadius; ++i)
            {
                for (int j = -maskRadius; j <= maskRadius; ++j)
                {
                    gaussianMask[i + maskRadius, j + maskRadius] /= maskCoefficient;
                }
            }

            return gaussianMask;
        }

        public static Image<Gray, byte> GaussianFiltering(Image<Gray, byte> inputImage, double variance)
        {
            int maskSize = (int)System.Math.Ceiling(4 * variance);
            if (maskSize % 2 == 0) maskSize++;

            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize - 1);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;
            double[,] mask = GaussianMask(maskRadius, variance);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    double sum = 0;
                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            sum += mask[i + maskRadius, j + maskRadius] * borderedImage.Data[y + i, x + j, 0];
                        }
                    }

                    if (sum > 255)
                        sum = 255;

                    result.Data[y, x, 0] = (byte)sum;
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }

        public static Image<Bgr, byte> GaussianFiltering(Image<Bgr, byte> inputImage, double variance)
        {
            int maskSize = (int)System.Math.Ceiling(4 * variance);
            if (maskSize % 2 == 0) maskSize++;

            Image<Bgr, byte> borderedImage = BorderReplicate(inputImage, maskSize);
            Image<Bgr, byte> result = new Image<Bgr, byte>(borderedImage.Size);

            int maskRadius = maskSize / 2;
            double[,] mask = GaussianMask(maskRadius, variance);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    double sumB = 0, sumG = 0, sumR = 0;
                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            sumB += mask[i + maskRadius, j + maskRadius] * borderedImage.Data[y + i, x + j, 0];
                            sumG += mask[i + maskRadius, j + maskRadius] * borderedImage.Data[y + i, x + j, 1];
                            sumR += mask[i + maskRadius, j + maskRadius] * borderedImage.Data[y + i, x + j, 2];
                        }
                    }

                    if (sumB > 255) sumB = 255;
                    result.Data[y, x, 0] = (byte)sumB;

                    if (sumG > 255) sumG = 255;
                    result.Data[y, x, 1] = (byte)sumG;

                    if (sumR > 255) sumR = 255;
                    result.Data[y, x, 2] = (byte)sumR;
                }
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

        #endregion

        #region High-pass filters

        #region Prewitt
        public static double[,] PrewittGradient(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, 2);
            double[,] gradient = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < borderedImage.Height - 1; ++y)
            {
                for (int x = 1; x < borderedImage.Width - 1; ++x)
                {
                    int fx = borderedImage.Data[y + 1, x - 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + borderedImage.Data[y + 1, x, 0] -
                             borderedImage.Data[y - 1, x, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y - 1, x + 1, 0];

                    int fy = borderedImage.Data[y - 1, x + 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + borderedImage.Data[y, x + 1, 0] -
                             borderedImage.Data[y, x - 1, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y + 1, x - 1, 0];

                    double grad = System.Math.Sqrt((fx * fx) + (fy * fy));
                    gradient[y - 1, x - 1] = grad;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> Prewitt(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = PrewittGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    double grad = gradient[y, x];
                    grad = grad < threshold ? 0 : 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }
        #endregion

        #region Sobel on grayscale
        public static double[,] SobelGradient(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, 2);
            double[,] gradient = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < borderedImage.Height - 1; ++y)
            {
                for (int x = 1; x < borderedImage.Width - 1; ++x)
                {
                    int fx = borderedImage.Data[y + 1, x - 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + 2 * borderedImage.Data[y + 1, x, 0] -
                               2 * borderedImage.Data[y - 1, x, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y - 1, x + 1, 0];

                    int fy = borderedImage.Data[y - 1, x + 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + 2 * borderedImage.Data[y, x + 1, 0] -
                               2 * borderedImage.Data[y, x - 1, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y + 1, x - 1, 0];

                    double grad = System.Math.Sqrt((fx * fx) + (fy * fy));
                    gradient[y - 1, x - 1] = grad;
                }
            }

            return gradient;
        }

        public static double[,] SobelDirection(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, 2);
            double[,] theta = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < borderedImage.Height - 1; ++y)
            {
                for (int x = 1; x < borderedImage.Width - 1; ++x)
                {
                    int fx = borderedImage.Data[y + 1, x - 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + 2 * borderedImage.Data[y + 1, x, 0] -
                               2 * borderedImage.Data[y - 1, x, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y - 1, x + 1, 0];

                    int fy = borderedImage.Data[y - 1, x + 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + 2 * borderedImage.Data[y, x + 1, 0] -
                               2 * borderedImage.Data[y, x - 1, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y + 1, x - 1, 0];

                    if (fx != 0)
                        theta[y - 1, x - 1] = System.Math.Atan(fy / fx);
                    else
                        theta[y - 1, x - 1] = System.Math.PI / 2;
                }
            }

            return theta;
        }

        public static Image<Gray, byte> Sobel(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = SobelGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    double grad = gradient[y, x];
                    grad = grad < threshold ? 0 : 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }

        #endregion

        #region Sobel on color using Euclidean distance

        private static double EuclideanDistance(byte red1, byte green1, byte blue1, byte red2, byte green2, byte blue2)
        {
            return System.Math.Sqrt(
                System.Math.Pow(red1 - red2, 2) +
                System.Math.Pow(green1 - green2, 2) +
                System.Math.Pow(blue1 - blue2, 2));
        }

        public static double[,] SobelGradient(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> borderedImage = BorderReplicate(inputImage, 2);
            double[,] gradient = new double[inputImage.Height, inputImage.Width];

            int[] dy1 = { 1, 1, 1, -1, 0, 0 };
            int[] dx1 = { -1, 0, 1, 1, 1, 1 };

            int[] dy2 = { -1, -1, -1, -1, 0, 1 };
            int[] dx2 = { -1, 0, 1, -1, -1, -1 };

            for (int y = 1; y < borderedImage.Height - 1; ++y)
            {
                for (int x = 1; x < borderedImage.Width - 1; ++x)
                {
                    double[] distances = new double[6];
                    for (int i = 0; i < 6; ++i)
                    {
                        byte blue1 = borderedImage.Data[y + dy1[i], x + dx1[i], 0];
                        byte green1 = borderedImage.Data[y + dy1[i], x + dx1[i], 1];
                        byte red1 = borderedImage.Data[y + dy1[i], x + dx1[i], 2];

                        byte blue2 = borderedImage.Data[y + dy2[i], x + dx2[i], 0];
                        byte green2 = borderedImage.Data[y + dy2[i], x + dx2[i], 1];
                        byte red2 = borderedImage.Data[y + dy2[i], x + dx2[i], 2];

                        distances[i] = EuclideanDistance(red1, green1, blue1, red2, green2, blue2);
                    };

                    gradient[y - 1, x - 1] = System.Linq.Enumerable.Max(distances);
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> Sobel(Image<Bgr, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = SobelGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    double grad = gradient[y, x];
                    grad = grad < threshold ? 0 : 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }
        #endregion

        #region Roberts
        public static double[,] RobertsGradient(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, 2);
            double[,] gradient = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < borderedImage.Height - 1; ++y)
            {
                for (int x = 1; x < borderedImage.Width - 1; ++x)
                {
                    int diagonal1 = -2 * borderedImage.Data[y - 1, x - 1, 0] - borderedImage.Data[y - 1, x, 0] - borderedImage.Data[y, x - 1, 0] +
                                     2 * borderedImage.Data[y + 1, x + 1, 0] + borderedImage.Data[y + 1, x, 0] + borderedImage.Data[y, x + 1, 0];

                    int diagonal2 = -2 * borderedImage.Data[y - 1, x + 1, 0] - borderedImage.Data[y - 1, x, 0] - borderedImage.Data[y, x + 1, 0] +
                                     2 * borderedImage.Data[y + 1, x - 1, 0] + borderedImage.Data[y + 1, x, 0] + borderedImage.Data[y, x - 1, 0];

                    double grad = System.Math.Sqrt((diagonal1 * diagonal1) + (diagonal2 * diagonal2));
                    gradient[y - 1, x - 1] = grad;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> Roberts(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = RobertsGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    double grad = gradient[y, x];
                    grad = grad < threshold ? 0 : 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }
        #endregion

        #region Canny

        public static Image<Gray, byte> CannyGradientImage(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> smoothImage = GaussianFiltering(inputImage, 1);

            double[,] gradient = SobelGradient(smoothImage);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int grad = (int)(gradient[y, x] + 0.5);
                    if (grad > 255) grad = 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }

        public static Image<Gray, byte> CannyAngleImage(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> smoothImage = GaussianFiltering(inputImage, 1);

            double[,] gradient = SobelGradient(smoothImage);
            double[,] direction = SobelDirection(smoothImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                    if (gradient[y, x] > 1)
                    {
                        // Direction 0 (horizontal)
                        if (-System.Math.PI / 8 <= direction[y, x] &&
                            direction[y, x] <= System.Math.PI / 8)
                        {
                            gradient[y, x] = 64;
                        }

                        // Direction 1 (-45 degrees)
                        if (-3 * System.Math.PI / 8 < direction[y, x] &&
                            direction[y, x] < -System.Math.PI / 8)
                        {
                            gradient[y, x] = 128;

                        }

                        // Direction 2 (vertical)
                        if ((-System.Math.PI / 2 <= direction[y, x] &&
                            direction[y, x] <= -3 * System.Math.PI / 8) ||
                            (3 * System.Math.PI / 8 <= direction[y, x] &&
                            direction[y, x] <= System.Math.PI / 2))
                        {
                            gradient[y, x] = 192;

                        }

                        // Direction 3 (45 degrees)
                        if (System.Math.PI / 8 < direction[y, x] &&
                            direction[y, x] < 3 * System.Math.PI / 8)
                        {
                            gradient[y, x] = 255;

                        }
                    }
                    else
                        gradient[y, x] = 0;
            }

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)gradient[y, x];
                }
            }

            return result;
        }

        public static Image<Gray, byte> CannyNonmaximaSuppression(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> smoothImage = GaussianFiltering(inputImage, 1);

            double[,] gradient = SobelGradient(smoothImage);
            double[,] direction = SobelDirection(smoothImage);

            double[,] nonmaximaSuppression = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < inputImage.Height - 1; ++y)
            {
                for (int x = 1; x < inputImage.Width - 1; ++x)
                {
                    // Direction 0 (horizontal)
                    if (-System.Math.PI / 8 <= direction[y, x] &&
                        direction[y, x] <= System.Math.PI / 8)
                    {
                        if (gradient[y, x] < gradient[y, x - 1] || gradient[y, x] < gradient[y, x + 1])
                            nonmaximaSuppression[y, x] = 0;
                        else
                            nonmaximaSuppression[y, x] = gradient[y, x];
                    }

                    // Direction 1 (-45 degrees)
                    if (-3 * System.Math.PI / 8 < direction[y, x] &&
                        direction[y, x] < -System.Math.PI / 8)
                    {
                        if (gradient[y, x] < gradient[y + 1, x - 1] || gradient[y, x] < gradient[y - 1, x + 1])
                            nonmaximaSuppression[y, x] = 0;
                        else
                            nonmaximaSuppression[y, x] = gradient[y, x];
                    }

                    // Direction 2 (vertical)
                    if ((-System.Math.PI / 2 <= direction[y, x] &&
                        direction[y, x] <= -3 * System.Math.PI / 8) ||
                        (3 * System.Math.PI / 8 <= direction[y, x] &&
                        direction[y, x] <= System.Math.PI / 2))
                    {
                        if (gradient[y, x] < gradient[y - 1, x] || gradient[y, x] < gradient[y + 1, x])
                            nonmaximaSuppression[y, x] = 0;
                        else
                            nonmaximaSuppression[y, x] = gradient[y, x];
                    }

                    // Direction 3 (45 degrees)
                    if (System.Math.PI / 8 < direction[y, x] &&
                        direction[y, x] < 3 * System.Math.PI / 8)
                    {
                        if (gradient[y, x] < gradient[y - 1, x - 1] || gradient[y, x] < gradient[y + 1, x + 1])
                            nonmaximaSuppression[y, x] = 0;
                        else
                            nonmaximaSuppression[y, x] = gradient[y, x];
                    }
                }
            }

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int nonmaxSup = (int)(nonmaximaSuppression[y, x] + 0.5);
                    if (nonmaxSup > 255) nonmaxSup = 255;

                    result.Data[y, x, 0] = (byte)nonmaxSup;
                }
            }

            return result;
        }

        public static Image<Gray, byte> CannyDoubleThresholding(Image<Gray, byte> inputImage, int T1, int T2)
        {
            Image<Gray, byte> nonmaxSupImage = CannyNonmaximaSuppression(inputImage);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < nonmaxSupImage.Width; ++x)
                {
                    if (nonmaxSupImage.Data[y, x, 0] <= T1)
                        result.Data[y, x, 0] = 0;
                    else if (nonmaxSupImage.Data[y, x, 0] > T2)
                        result.Data[y, x, 0] = 255;
                    else
                        result.Data[y, x, 0] = nonmaxSupImage.Data[y, x, 0];
                }
            }

            return result;
        }

        public static Image<Gray, byte> CannyLinkEdges(Image<Gray, byte> inputImage, int T1, int T2)
        {
            Image<Gray, byte> thresholdImage = CannyDoubleThresholding(inputImage, T1, T2);

            // 8-connectivity
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };

            for (int y = 1; y < inputImage.Height - 1; ++y)
            {
                for (int x = 1; x < inputImage.Width - 1; ++x)
                {
                    if (T1 < thresholdImage.Data[y, x, 0] && thresholdImage.Data[y, x, 0] <= T2)
                    {
                        bool isConnected = false;
                        for (int i = 0; i < 8 && isConnected == false; ++i)
                        {
                            int Y = y + dy[i];
                            int X = x + dx[i];

                            if (thresholdImage.Data[Y, X, 0] > T2)
                            {
                                thresholdImage.Data[y, x, 0] = 255;
                                isConnected = true;
                            }
                        }

                        if (isConnected == false)
                            thresholdImage.Data[y, x, 0] = 0;
                    }
                }
            }

            return thresholdImage;
        }

        public static Image<Gray, byte> Canny(Image<Gray, byte> inputImage, int T1, int T2)
        {
            return CannyLinkEdges(inputImage, T1, T2);
        }

        public static Image<Bgr, byte> Canny(Image<Bgr, byte> inputImage, int T1, int T2)
        {
            Image<Bgr, byte> result = GaussianFiltering(inputImage, 1);
            return result;
        }
        #endregion

        #endregion
    }
}
