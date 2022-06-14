using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.Tools.Tools;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class Thresholding
    {
        #region Thresholding
        public static Image<Gray, byte> InputThresholding(Image<Gray, byte> inputImage, double threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] >= threshold)
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
            int[] histogram = Histogram(inputImage);

            int lowLevel = 255, highLevel = 0;
            for (int pixel = 0; pixel < 255 && lowLevel == 255; ++pixel)
                if (histogram[pixel] != 0)
                    lowLevel = pixel;

            for (int pixel = 255; pixel >= 0 && highLevel == 0; --pixel)
                if (histogram[pixel] != 0)
                    highLevel = pixel;

            int currentThreshold = (int)(lowLevel + highLevel) / 2;
            int threshold;

            double[] relativeHistogram = RelativeHistogram(inputImage);
            do
            {
                threshold = currentThreshold;

                double P1 = GetHistogramProbability(0, currentThreshold, relativeHistogram);
                double mu1 = GetHistogramSum(0, currentThreshold, relativeHistogram);
                if (P1 != 0)
                    mu1 /= P1;

                double P2 = GetHistogramProbability(currentThreshold + 1, 255, relativeHistogram);
                double mu2 = GetHistogramSum(currentThreshold + 1, 255, relativeHistogram);
                if (P2 != 0)
                    mu2 /= P2;

                currentThreshold = (int)(mu1 + mu2) / 2;

            }
            while (threshold != currentThreshold);

            return threshold;
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

        #region Adaptive thresholding

        public static Image<Gray, byte> AdaptiveThresholding(Image<Gray, byte> inputImage, int maskSize, double b)
        {
            Image<Gray, byte> borderedImage = BorderReplicate(inputImage, maskSize - 1);
            Image<Gray, byte> result = new Image<Gray, byte>(borderedImage.Size);

            Image<Gray, int> integralImage = IntegralImage(borderedImage);

            int maskRadius = maskSize / 2;

            for (int y = maskRadius; y < borderedImage.Height - maskRadius; ++y)
            {
                for (int x = maskRadius; x < borderedImage.Width - maskRadius; ++x)
                {
                    double thresh = b * MeanArea(integralImage, x - maskRadius, y - maskRadius, x + maskRadius, y + maskRadius);

                    if (borderedImage.Data[y, x, 0] > thresh)
                        result.Data[y, x, 0] = 255;
                }
            }

            return CropImage(result, maskRadius, maskRadius, inputImage.Width + maskRadius, inputImage.Height + maskRadius);
        }

        #endregion

        #region 3D Color Thresholding
        public static Image<Gray, byte> Thresholding3D(Image<Bgr, byte> inputImage, double Xpos, double Ypos, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            byte B0 = inputImage.Data[(int)Ypos, (int)Xpos, 0];
            byte G0 = inputImage.Data[(int)Ypos, (int)Xpos, 1];
            byte R0 = inputImage.Data[(int)Ypos, (int)Xpos, 2];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte B = inputImage.Data[y, x, 0];
                    byte G = inputImage.Data[y, x, 1];
                    byte R = inputImage.Data[y, x, 2];

                    double distance = System.Math.Sqrt(
                        System.Math.Pow(R - R0, 2) +
                        System.Math.Pow(G - G0, 2) +
                        System.Math.Pow(B - B0, 2));

                    if (distance <= threshold)
                        result.Data[y, x, 0] = 255;
                }
            }

            return result;
        }
        #endregion

        #region 2D Color Thresholding
        public static Image<Gray, byte> Thresholding2D(Image<Bgr, byte> inputImage, double Xpos, double Ypos, double threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            byte B0 = inputImage.Data[(int)Ypos, (int)Xpos, 0];
            byte G0 = inputImage.Data[(int)Ypos, (int)Xpos, 1];
            byte R0 = inputImage.Data[(int)Ypos, (int)Xpos, 2];

            double r0 = (double)R0 / (R0 + G0 + B0);
            double g0 = (double)G0 / (R0 + G0 + B0);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte B = inputImage.Data[y, x, 0];
                    byte G = inputImage.Data[y, x, 1];
                    byte R = inputImage.Data[y, x, 2];

                    double r = (double)R / (R + G + B);
                    double g = (double)G / (R + G + B);

                    double distance = System.Math.Sqrt(
                        System.Math.Pow(r - r0, 2) +
                        System.Math.Pow(g - g0, 2));

                    if (distance <= threshold)
                        result.Data[y, x, 0] = 255;
                }
            }

            return result;
        }
        #endregion
    }
}
