using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.ObjectModel;

namespace ImageProcessingAlgorithms.Tools
{
    public class Tools
    {
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            var result = new Image<Gray, byte>(inputImage.Size);
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
            var result = new Image<Bgr, byte>(inputImage.Size);
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

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> image)
        {
            var result = new Image<Bgr, byte>(image.Size);
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
            var result = new Image<Gray, byte>(image.Size);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[y, x, 0] = image.Data[y, x, 0];
                }
            }
            return image;
        }

        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            var grayImage = coloredImage.Convert<Gray, byte>();

            return grayImage;
        }

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

        public static Image<Gray, byte> HermiteSplineInterpolation(Image<Gray, byte> inputImage, Collection<int> LUT)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)LUT[inputImage.Data[y, x, 0]];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> HermiteSplineInterpolation(Image<Bgr, byte> inputImage, Collection<int> lookUpTable)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)lookUpTable[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = (byte)lookUpTable[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = (byte)lookUpTable[inputImage.Data[y, x, 2]];
                }
            }

            return result;
        }

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

        private static System.Tuple<int, int> GetThresholdValues(Image<Gray, byte> inputImage)
        {
            double[] histogram = GetHistogram(inputImage);
            double mu = Mean(inputImage);

            double maxInterclassVariance = 0;
            int threshold1 = -1, threshold2 = -1;

            for (int k1 = 0; k1 < 254; ++k1)
            {
                double P1 = 0, mu1 = 0;
                for (int k = 0; k <= k1; ++k)
                {
                    P1 += histogram[k];
                    mu1 += k * histogram[k];
                }
                mu1 /= P1;

                for (int k2 = k1 + 1; k1 < 254 && k2 < 255; ++k2)
                {
                    double P2 = 0, mu2 = 0;
                    for (int k = k1 + 1; k <= k2; ++k)
                    {
                        P2 += histogram[k];
                        mu2 += k * histogram[k];
                    }
                    mu2 /= P2;

                    double P3 = 1 - P1 - P2, mu3 = 0;
                    for (int k = k2 + 1; k <= 255; ++k)
                    {
                        mu3 += k * histogram[k];
                    }
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
                    }
                }
            }
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
    }
}