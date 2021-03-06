using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.Tools.Tools;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;
using System.Collections.Generic;
using static System.Math;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class Filters
    {
        #region Low-pass filters

        #region Mean filtering
        public static Image<Gray, byte> MeanFiltering(Image<Gray, byte> inputImage, double maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, (int)(maskSize - 1));
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            Image<Gray, int> integralImage = IntegralImage(borderedImage);

            int maskRadius = (int)(maskSize / 2);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    result.Data[y, x, 0] =
                        (byte)MeanArea(integralImage, x - maskRadius, y - maskRadius, x + maskRadius, y + maskRadius);
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }


        public static Image<Bgr, byte> MeanFiltering(Image<Bgr, byte> inputImage, double maskSize)
        {
            Image<Bgr, byte> borderedImage = BorderReplicate(inputImage, (int)(maskSize - 1));
            Image<Bgr, byte> result = new Image<Bgr, byte>(borderedImage.Size);

            Image<Bgr, int> integralImage = IntegralImage(borderedImage);

            int maskRadius = (int)(maskSize / 2);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    result.Data[y, x, 0] =
                        (byte)MeanArea(integralImage, x - maskRadius, y - maskRadius, x + maskRadius, y + maskRadius, 0);
                    result.Data[y, x, 1] =
                        (byte)MeanArea(integralImage, x - maskRadius, y - maskRadius, x + maskRadius, y + maskRadius, 1);
                    result.Data[y, x, 2] =
                        (byte)MeanArea(integralImage, x - maskRadius, y - maskRadius, x + maskRadius, y + maskRadius, 2);
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }
        #endregion

        #region Median filtering
        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> inputImage, double maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, (int)(maskSize - 1));
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = (int)(maskSize / 2);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    result.Data[y, x, 0] = GetMedianValue(borderedImage, (int)maskSize, y, x);
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }
        #endregion

        #region Fast median filtering
        public static Image<Gray, byte> FastMedianFiltering(Image<Gray, byte> inputImage, double maskSize)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, (int)(maskSize - 1));
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            int maskRadius = (int)(maskSize / 2);
            int middle = (int)((maskSize * maskSize + 1) / 2);
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
        public static Image<Bgr, byte> VectorMedianFiltering(Image<Bgr, byte> inputImage, double maskSize)
        {
            Image<Bgr, byte> borderedImage = BorderReplicate(inputImage, (int)(maskSize - 1));
            Image<Bgr, byte> result = new Image<Bgr, byte>(borderedImage.Size);

            int maskRadius = (int)(maskSize / 2);
            int size = (int)(maskSize * maskSize);

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
                            int blue = (int)Pow(neighbourhood[i].Item1 - neighbourhood[j].Item1, 2);
                            int green = (int)Pow(neighbourhood[i].Item2 - neighbourhood[j].Item2, 2);
                            int red = (int)Pow(neighbourhood[i].Item3 - neighbourhood[j].Item3, 2);

                            distances[i, j] = distances[j, i] = Sqrt(blue + green + red);
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
                    gaussianMask[i + maskRadius, j + maskRadius] = Exp(-(i * i + j * j) / (2 * variance * variance));
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
            int maskSize = (int)Ceiling(4 * variance);
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
            int maskSize = (int)Ceiling(4 * variance);
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
            return Exp(-((i * i) + (j * j)) / (2 * variance_d * variance_d));
        }

        private static double RangeMask(Image<Gray, byte> image, int y, int x, int i, int j, double variance_r)
        {
            int value = image.Data[y + i, x + j, 0] - image.Data[y, x, 0];
            return Exp(-(value * value) / (2 * variance_r * variance_r));
        }

        public static Image<Gray, byte> GaussianBilateralFiltering(Image<Gray, byte> inputImage, double variance_d, double variance_r)
        {
            int maskSize = (int)Ceiling(4 * variance_d);
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

        #region Kuwahara filtering
        public static Image<Gray, byte> Kuwahara(Image<Gray, byte> inputImage)
        {
            int maskSize = 5;
            int maskRadius = maskSize / 2;

            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            double[,] meanAroundPixels = MeanAroundPixels(borderedImage, maskRadius + 1);
            double[,] varianceAroundPixels = VarianceAroundPixels(borderedImage, meanAroundPixels, maskRadius + 1);

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    List<(double, double)> variances = new List<(double, double)>
                    {
                        (varianceAroundPixels[y - 1, x - 1], meanAroundPixels[y - 1, x - 1]),
                        (varianceAroundPixels[y - 1, x + 1], meanAroundPixels[y - 1, x + 1]),
                        (varianceAroundPixels[y + 1, x + 1], meanAroundPixels[y + 1, x + 1]),
                        (varianceAroundPixels[y + 1, x - 1], meanAroundPixels[y + 1, x - 1])
                    };

                    double minVariance = double.MaxValue;
                    double mean = 0;
                    foreach (var pair in variances)
                    {
                        if (pair.Item1 < minVariance)
                        {
                            minVariance = pair.Item1;
                            mean = pair.Item2;
                        }
                    }

                    result.Data[y, x, 0] = (byte)Min(255, mean + 0.5);
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
                    int fx = borderedImage.Data[y - 1, x + 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + borderedImage.Data[y, x + 1, 0] -
                             borderedImage.Data[y, x - 1, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y + 1, x - 1, 0];

                    int fy = borderedImage.Data[y + 1, x - 1, 0] - borderedImage.Data[y - 1, x - 1, 0] + borderedImage.Data[y + 1, x, 0] -
                             borderedImage.Data[y - 1, x, 0] + borderedImage.Data[y + 1, x + 1, 0] - borderedImage.Data[y - 1, x + 1, 0];

                    double grad = Sqrt((fx * fx) + (fy * fy));
                    gradient[y - 1, x - 1] = grad;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> Prewitt(Image<Gray, byte> inputImage, double threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = PrewittGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (gradient[y, x] > threshold)
                        result.Data[y, x, 0] = 255;
                }
            }

            return result;
        }
        #endregion

        #region Sobel on grayscale
        public static double[,] SobelGradient(Image<Gray, byte> inputImage)
        {
            double[,] gradient = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < inputImage.Height - 1; ++y)
            {
                for (int x = 1; x < inputImage.Width - 1; ++x)
                {
                    int fx = inputImage.Data[y - 1, x + 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y, x + 1, 0] -
                             2 * inputImage.Data[y, x - 1, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y + 1, x - 1, 0];

                    int fy = inputImage.Data[y + 1, x - 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y + 1, x, 0] -
                             2 * inputImage.Data[y - 1, x, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y - 1, x + 1, 0];

                    gradient[y, x] = Sqrt((fx * fx) + (fy * fy));
                }
            }

            return gradient;
        }

        public static double[,] SobelDirection(Image<Gray, byte> inputImage)
        {
            double[,] angle = new double[inputImage.Height, inputImage.Width];

            for (int y = 1; y < inputImage.Height - 1; ++y)
            {
                for (int x = 1; x < inputImage.Width - 1; ++x)
                {
                    int fx = inputImage.Data[y - 1, x + 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y, x + 1, 0] -
                               2 * inputImage.Data[y, x - 1, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y + 1, x - 1, 0];

                    int fy = inputImage.Data[y + 1, x - 1, 0] - inputImage.Data[y - 1, x - 1, 0] + 2 * inputImage.Data[y + 1, x, 0] -
                               2 * inputImage.Data[y - 1, x, 0] + inputImage.Data[y + 1, x + 1, 0] - inputImage.Data[y - 1, x + 1, 0];

                    if (fx == 0)
                    {
                        if (fy < 0) angle[y, x] = -PI / 2;
                        if (fy > 0) angle[y, x] = PI / 2;
                    }
                    else
                        angle[y, x] = Atan(fy / fx);
                }
            }

            return angle;
        }

        public static Image<Gray, byte> Sobel(Image<Gray, byte> inputImage, double threshold)
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
            return Sqrt(
                Pow(red1 - red2, 2) +
                Pow(green1 - green2, 2) +
                Pow(blue1 - blue2, 2));
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

        public static Image<Gray, byte> Sobel(Image<Bgr, byte> inputImage, double threshold)
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

                    double grad = Sqrt((diagonal1 * diagonal1) + (diagonal2 * diagonal2));
                    gradient[y - 1, x - 1] = grad;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> Roberts(Image<Gray, byte> inputImage, double threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            double[,] gradient = RobertsGradient(inputImage);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (gradient[y, x] > threshold)
                        result.Data[y, x, 0] = 255;
                }
            }

            return result;
        }
        #endregion

        #region Canny

        #region Canny gradient magnitude

        #region On grayscale
        private static double[,] CannyGradient(Image<Gray, byte> smoothImage, double lowThreshold)
        {
            var gradient = SobelGradient(smoothImage);

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (gradient[y, x] <= lowThreshold)
                        gradient[y, x] = 0;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> CannyGradientForGray(Image<Gray, byte> inputImage, double lowThreshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyGradient = CannyGradient(smoothImage, lowThreshold);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int grad = (int)(cannyGradient[y, x] + 0.5);
                    if (grad > 255) grad = 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }
        #endregion

        #region On color
        private static double[,] CannyGradient(Image<Bgr, byte> smoothImage, double lowThreshold)
        {
            var gradient = MaxVariance(smoothImage);

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (gradient[y, x] <= lowThreshold)
                        gradient[y, x] = 0;
                }
            }

            return gradient;
        }

        public static Image<Gray, byte> CannyGradientForColor(Image<Bgr, byte> inputImage, double threshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyGradient = CannyGradient(smoothImage, threshold);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int grad = (int)(cannyGradient[y, x] + 0.5);
                    if (grad > 255) grad = 255;

                    result.Data[y, x, 0] = (byte)grad;
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Canny gradient direction

        #region On grayscale
        private static int[,] CannyGradientDirection(Image<Gray, byte> smoothImage, double lowThreshold)
        {
            var cannyGradient = CannyGradient(smoothImage, lowThreshold);
            var sobelDirection = SobelDirection(smoothImage);

            var cannyDirection = new int[smoothImage.Height, smoothImage.Width];

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (cannyGradient[y, x] > lowThreshold)
                    {
                        double dir = sobelDirection[y, x] * 180 / PI;

                        // Direction 0 (horizontal)
                        if (-22.5 <= dir && dir < 22.5)
                            cannyDirection[y, x] = 0;

                        // Direction 1 (-45 degrees)
                        if (-67.5 <= dir && dir < -22.5)
                            cannyDirection[y, x] = 1;

                        //Direction 2 (vertical)
                        if ((-90 <= dir && dir < -67.5) || (67.5 <= dir && dir <= 90))
                            cannyDirection[y, x] = 2;

                        //Direction 3 (45 degrees)
                        if (22.5 <= dir && dir < 67.5)
                            cannyDirection[y, x] = 3;
                    }
                    else
                        cannyDirection[y, x] = -1;
                }
            }

            return cannyDirection;
        }

        public static Image<Bgr, byte> CannyGradientDirectionForGray(Image<Gray, byte> inputImage, double lowThreshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyDirection = CannyGradientDirection(smoothImage, lowThreshold);

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (cannyDirection[y, x] == 0)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 255;
                        result.Data[y, x, 2] = 0;
                    }

                    if (cannyDirection[y, x] == 1)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 255;
                        result.Data[y, x, 2] = 255;
                    }

                    if (cannyDirection[y, x] == 2)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 0;
                        result.Data[y, x, 2] = 255;
                    }

                    if (cannyDirection[y, x] == 3)
                    {
                        result.Data[y, x, 0] = 255;
                        result.Data[y, x, 1] = 0;
                        result.Data[y, x, 2] = 0;
                    }
                }
            }

            return result;
        }
        #endregion

        #region On color
        private static int[,] CannyGradientDirection(Image<Bgr, byte> smoothImage, double lowThreshold)
        {
            var cannyGradient = CannyGradient(smoothImage, lowThreshold);
            var maxVarianceDirection = MaxVarianceDirection(smoothImage);

            var cannyDirection = new int[smoothImage.Height, smoothImage.Width];

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (cannyGradient[y, x] > lowThreshold)
                    {
                        double dir = maxVarianceDirection[y, x] * 180 / PI;

                        // Direction 0 (horizontal)
                        if (-22.5 <= dir && dir < 22.5)
                            cannyDirection[y, x] = 0;

                        // Direction 1 (-45 degrees)
                        if (-67.5 <= dir && dir < -22.5)
                            cannyDirection[y, x] = 1;

                        //Direction 2 (vertical)
                        if ((-90 <= dir && dir < -67.5) || (67.5 <= dir && dir <= 90))
                            cannyDirection[y, x] = 2;

                        //Direction 3 (45 degrees)
                        if (22.5 <= dir && dir < 67.5)
                            cannyDirection[y, x] = 3;
                    }
                    else
                        cannyDirection[y, x] = -1;
                }
            }

            return cannyDirection;
        }

        public static Image<Bgr, byte> CannyGradientDirectionForColor(Image<Bgr, byte> inputImage, double lowThreshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyDirection = CannyGradientDirection(smoothImage, lowThreshold);

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (cannyDirection[y, x] == 0)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 255;
                        result.Data[y, x, 2] = 0;
                    }

                    if (cannyDirection[y, x] == 1)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 255;
                        result.Data[y, x, 2] = 255;
                    }

                    if (cannyDirection[y, x] == 2)
                    {
                        result.Data[y, x, 0] = 0;
                        result.Data[y, x, 1] = 0;
                        result.Data[y, x, 2] = 255;
                    }

                    if (cannyDirection[y, x] == 3)
                    {
                        result.Data[y, x, 0] = 255;
                        result.Data[y, x, 1] = 0;
                        result.Data[y, x, 2] = 0;
                    }
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Canny Nonmaxima suppression

        #region On grayscale
        private static double[,] CannyNonmaxSuppression(Image<Gray, byte> smoothImage, double lowThreshold)
        {
            var cannyGradient = CannyGradient(smoothImage, lowThreshold);
            var cannyDirection = CannyGradientDirection(smoothImage, lowThreshold);
            var nonmaxSuppression = new double[smoothImage.Height, smoothImage.Width];

            for (int y = 1; y < smoothImage.Height - 1; ++y)
            {
                for (int x = 1; x < smoothImage.Width - 1; ++x)
                    if (cannyDirection[y, x] != -1)
                    {
                        double prev = 255, next = 255;
                        if (cannyDirection[y, x] == 0)
                        {
                            prev = cannyGradient[y, x - 1];
                            next = cannyGradient[y, x + 1];
                        }
                        else if (cannyDirection[y, x] == 1)
                        {
                            prev = cannyGradient[y - 1, x + 1];
                            next = cannyGradient[y + 1, x - 1];
                        }
                        else if (cannyDirection[y, x] == 2)
                        {
                            prev = cannyGradient[y - 1, x];
                            next = cannyGradient[y + 1, x];
                        }
                        else if (cannyDirection[y, x] == 3)
                        {
                            prev = cannyGradient[y - 1, x - 1];
                            next = cannyGradient[y + 1, x + 1];
                        }

                        if (cannyGradient[y, x] == next)
                            cannyGradient[y, x] = 0;

                        if (cannyGradient[y, x] >= prev && cannyGradient[y, x] > next)
                            nonmaxSuppression[y, x] = cannyGradient[y, x];
                    }
            }

            return nonmaxSuppression;
        }

        public static Image<Gray, byte> CannyNonmaxSuppressionForGray(Image<Gray, byte> inputImage, double lowThreshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyNonmaxSuppresion = CannyNonmaxSuppression(smoothImage, lowThreshold);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int value = (int)(cannyNonmaxSuppresion[y, x] + 0.5);
                    if (value > 255) value = 255;

                    result.Data[y, x, 0] = (byte)value;
                }
            }

            return result;
        }
        #endregion

        #region On color
        private static double[,] CannyNonmaxSuppression(Image<Bgr, byte> smoothImage, double lowThreshold)
        {
            var cannyGradient = CannyGradient(smoothImage, lowThreshold);
            var cannyDirection = CannyGradientDirection(smoothImage, lowThreshold);
            var nonmaxSuppression = new double[smoothImage.Height, smoothImage.Width];

            for (int y = 1; y < smoothImage.Height - 1; ++y)
            {
                for (int x = 1; x < smoothImage.Width - 1; ++x)
                    if (cannyDirection[y, x] != -1)
                    {
                        double prev = 255, next = 255;
                        if (cannyDirection[y, x] == 0)
                        {
                            prev = cannyGradient[y, x - 1];
                            next = cannyGradient[y, x + 1];
                        }
                        else if (cannyDirection[y, x] == 1)
                        {
                            prev = cannyGradient[y - 1, x + 1];
                            next = cannyGradient[y + 1, x - 1];
                        }
                        else if (cannyDirection[y, x] == 2)
                        {
                            prev = cannyGradient[y - 1, x];
                            next = cannyGradient[y + 1, x];
                        }
                        else if (cannyDirection[y, x] == 3)
                        {
                            prev = cannyGradient[y - 1, x - 1];
                            next = cannyGradient[y + 1, x + 1];
                        }

                        if (cannyGradient[y, x] == next)
                            cannyGradient[y, x] = 0;

                        if (cannyGradient[y, x] >= prev && cannyGradient[y, x] > next)
                            nonmaxSuppression[y, x] = cannyGradient[y, x];
                    }
            }

            return nonmaxSuppression;
        }

        public static Image<Gray, byte> CannyNonmaxSuppressionForColor(Image<Bgr, byte> inputImage, double lowThreshold)
        {
            var smoothImage = GaussianFiltering(inputImage, 1);
            var cannyNonmaxSuppresion = CannyNonmaxSuppression(smoothImage, lowThreshold);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int value = (int)(cannyNonmaxSuppresion[y, x] + 0.5);
                    if (value > 255) value = 255;

                    result.Data[y, x, 0] = (byte)value;
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Canny Hysteresis thresholding

        #region On grayscale
        private static double[,] CannyHysteresisThresholding(Image<Gray, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyNonmaxSuppression(smoothImage, lowThreshold);
            Queue<(int, int)> strongEdges = new Queue<(int, int)>();

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (thinEdges[y, x] <= lowThreshold)
                        thinEdges[y, x] = 0;

                    if (thinEdges[y, x] > highThreshold)
                    {
                        thinEdges[y, x] = 255;
                        strongEdges.Enqueue((y, x));
                    }
                }
            }

            // 8-connectivity
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };

            while (strongEdges.Count > 0)
            {
                var currentEdge = strongEdges.Dequeue();

                for (int i = 0; i < dy.Length; ++i)
                {
                    int Y = currentEdge.Item1 + dy[i];
                    int X = currentEdge.Item2 + dx[i];

                    if (Y >= 0 && Y < smoothImage.Height && X >= 0 && X < smoothImage.Width)
                    {
                        if (lowThreshold < thinEdges[Y, X] && thinEdges[Y, X] <= highThreshold)
                        {
                            thinEdges[Y, X] = 255;
                            strongEdges.Enqueue((Y, X));
                        }
                    }
                }
            }

            return thinEdges;
        }

        public static Image<Gray, byte> CannyHysteresisThresholdingImage(Image<Gray, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyHysteresisThresholding(smoothImage, lowThreshold, highThreshold);
            var result = new Image<Gray, byte>(smoothImage.Size);

            for (int y = 0; y < smoothImage.Height; y++)
            {
                for (int x = 0; x < smoothImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(thinEdges[y, x] + 0.5);
                }
            }

            return result;
        }
        #endregion

        #region On color
        private static double[,] CannyHysteresisThresholding(Image<Bgr, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyNonmaxSuppression(smoothImage, lowThreshold);
            Queue<(int, int)> strongEdges = new Queue<(int, int)>();

            for (int y = 0; y < smoothImage.Height; ++y)
            {
                for (int x = 0; x < smoothImage.Width; ++x)
                {
                    if (thinEdges[y, x] <= lowThreshold)
                        thinEdges[y, x] = 0;

                    if (thinEdges[y, x] > highThreshold)
                    {
                        thinEdges[y, x] = 255;
                        strongEdges.Enqueue((y, x));
                    }
                }
            }

            // 8-connectivity
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };

            while (strongEdges.Count > 0)
            {
                var currentEdge = strongEdges.Dequeue();

                for (int i = 0; i < dy.Length; ++i)
                {
                    int Y = currentEdge.Item1 + dy[i];
                    int X = currentEdge.Item2 + dx[i];

                    if (Y >= 0 && Y < smoothImage.Height && X >= 0 && X < smoothImage.Width)
                    {
                        if (lowThreshold < thinEdges[Y, X] && thinEdges[Y, X] <= highThreshold)
                        {
                            thinEdges[Y, X] = 255;
                            strongEdges.Enqueue((Y, X));
                        }
                    }
                }
            }

            return thinEdges;
        }

        public static Image<Gray, byte> CannyHysteresisThresholdingImage(Image<Bgr, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyHysteresisThresholding(smoothImage, lowThreshold, highThreshold);
            var result = new Image<Gray, byte>(smoothImage.Size);

            for (int y = 0; y < smoothImage.Height; y++)
            {
                for (int x = 0; x < smoothImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(thinEdges[y, x] + 0.5);
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Canny operator
        public static Image<Gray, byte> Canny(Image<Gray, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyHysteresisThresholding(smoothImage, lowThreshold, highThreshold);
            var result = new Image<Gray, byte>(smoothImage.Size);

            for (int y = 0; y < smoothImage.Height; y++)
            {
                for (int x = 0; x < smoothImage.Width; x++)
                {
                    if (lowThreshold < thinEdges[y, x] && thinEdges[y, x] <= highThreshold)
                        thinEdges[y, x] = 0;

                    result.Data[y, x, 0] = (byte)(thinEdges[y, x] + 0.5);
                }
            }

            return result;
        }

        public static Image<Gray, byte> Canny(Image<Bgr, byte> smoothImage, int lowThreshold, int highThreshold)
        {
            var thinEdges = CannyHysteresisThresholding(smoothImage, lowThreshold, highThreshold);
            var result = new Image<Gray, byte>(smoothImage.Size);

            for (int y = 0; y < smoothImage.Height; y++)
            {
                for (int x = 0; x < smoothImage.Width; x++)
                {
                    if (lowThreshold < thinEdges[y, x] && thinEdges[y, x] <= highThreshold)
                        thinEdges[y, x] = 0;

                    result.Data[y, x, 0] = (byte)(thinEdges[y, x] + 0.5);
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Emboss
        public static Image<Gray, byte> Emboss(Image<Gray, byte> inputImage, double maskSize)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int[,] mask = new int[(int)maskSize, (int)maskSize];
            for (int i = 0; i < maskSize; ++i)
            {
                for (int j = 0; j < maskSize; ++j)
                {
                    if (i + j < maskSize - 1)
                    {
                        mask[i, j] = -1;
                    }
                    else if (i + j > maskSize - 1)
                    {
                        mask[i, j] = 1;
                    }
                }
            }

            int maskRadius = (int)maskSize / 2;

            for (int y = maskRadius; y < inputImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < inputImage.Width - maskRadius; ++x)
                {
                    int value = 0;
                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            value += mask[i + maskRadius, j + maskRadius] * inputImage.Data[y + i, x + j, 0];
                        }
                    }

                    result.Data[y, x, 0] = (byte)Min(255, value + 128);
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Emboss(Image<Bgr, byte> inputImage, double maskSize)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            int[,] mask = new int[(int)maskSize, (int)maskSize];
            for (int i = 0; i < maskSize; ++i)
            {
                for (int j = 0; j < maskSize; ++j)
                {
                    if (i + j < maskSize - 1)
                    {
                        mask[i, j] = -1;
                    }
                    else if (i + j > maskSize - 1)
                    {
                        mask[i, j] = 1;
                    }
                }
            }

            int maskRadius = (int)maskSize / 2;

            for (int y = maskRadius; y < inputImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < inputImage.Width - maskRadius; ++x)
                {
                    int valueB = 0, valueG = 0, valueR = 0;
                    for (int i = -maskRadius; i <= maskRadius; ++i)
                    {
                        for (int j = -maskRadius; j <= maskRadius; ++j)
                        {
                            valueB += mask[i + maskRadius, j + maskRadius] * inputImage.Data[y + i, x + j, 0];
                            valueG += mask[i + maskRadius, j + maskRadius] * inputImage.Data[y + i, x + j, 1];
                            valueR += mask[i + maskRadius, j + maskRadius] * inputImage.Data[y + i, x + j, 2];
                        }
                    }

                    result.Data[y, x, 0] = (byte)Min(255, valueB + 128);
                    result.Data[y, x, 1] = (byte)Min(255, valueG + 128);
                    result.Data[y, x, 2] = (byte)Min(255, valueR + 128);
                }
            }

            return result;
        }
        #endregion

        #endregion
    }
}