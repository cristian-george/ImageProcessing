using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class Segmentation
    {
        #region Detecting lines

        #region Hough transformation (3 quadrants)

        private static int[,] GetHoughMatrixThreeQuadrants(Image<Gray, byte> binaryImage)
        {
            double diagonalLength = System.Math.Sqrt(System.Math.Pow(binaryImage.Height, 2) +
                                                     System.Math.Pow(binaryImage.Width, 2));
            int[,] houghMatrix = new int[(int)diagonalLength, 271];

            for (int y = 0; y < binaryImage.Height; ++y)
            {
                for (int x = 0; x < binaryImage.Width; ++x)
                {
                    if (binaryImage.Data[y, x, 0] == 255)
                    {
                        for (int angle = -90; angle <= 180; ++angle)
                        {
                            double radians = angle * System.Math.PI / 180;

                            double radius = y * System.Math.Cos(radians) +
                                            x * System.Math.Sin(radians);

                            if (radius >= 0)
                            {
                                ++houghMatrix[(int)radius, angle + 90];
                            }
                        }
                    }
                }
            }

            return houghMatrix;
        }

        public static Image<Gray, byte> HoughTransformThreeQuadrants(Image<Gray, byte> binaryImage)
        {
            var houghMatrix = GetHoughMatrixThreeQuadrants(binaryImage);

            int height = houghMatrix.GetLength(0);
            int width = houghMatrix.GetLength(1);

            var result = new Image<Gray, byte>(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    result.Data[y, x, 0] = (byte)System.Math.Min(255, houghMatrix[y, x]);
                }
            }

            return result;
        }
        #endregion

        #region Hough transformation (2 quadrants)
        private static int[,] GetHoughMatrixTwoQuadrants(Image<Gray, byte> binaryImage)
        {
            double diagonalLength = System.Math.Sqrt(System.Math.Pow(binaryImage.Height, 2) +
                                                     System.Math.Pow(binaryImage.Width, 2));
            int[,] houghMatrix = new int[2 * (int)diagonalLength, 181];

            for (int y = 0; y < binaryImage.Height; ++y)
            {
                for (int x = 0; x < binaryImage.Width; ++x)
                {
                    if (binaryImage.Data[y, x, 0] == 255)
                    {
                        for (int angle = -90; angle <= 90; ++angle)
                        {
                            double radians = angle * System.Math.PI / 180;

                            double radius = y * System.Math.Cos(radians) +
                                            x * System.Math.Sin(radians);

                            ++houghMatrix[(int)(radius + diagonalLength) / 2, angle + 90];
                        }
                    }
                }
            }

            return houghMatrix;
        }

        public static Image<Gray, byte> HoughTransformTwoQuadrants(Image<Gray, byte> binaryImage)
        {
            var houghMatrix = GetHoughMatrixTwoQuadrants(binaryImage);

            int height = houghMatrix.GetLength(0);
            int width = houghMatrix.GetLength(1);

            var result = new Image<Gray, byte>(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    result.Data[y, x, 0] = (byte)System.Math.Min(255, houghMatrix[y, x]);
                }
            }

            return result;
        }
        #endregion

        #endregion
    }
}
