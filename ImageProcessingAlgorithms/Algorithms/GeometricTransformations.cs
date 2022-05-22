using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingAlgorithms.AlgorithmsHelper;
using System;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class GeometricTransformations
    {
        #region Bilinear interpolation
        private static int BilinearInterpolation(Image<Gray, byte> f, double xc, double yc)
        {
            int x0 = (int)xc, y0 = (int)yc;
            int x1 = x0 + 1, y1 = y0 + 1;

            // calculate f(xc, y0)
            double f1 = (f.Data[y0, x1, 0] - f.Data[y0, x0, 0]) * (xc - x0) + f.Data[y0, x0, 0];

            // calculate f(xc, y0 + 1)
            double f2 = (f.Data[y1, x1, 0] - f.Data[y1, x0, 0]) * (xc - x0) + f.Data[y1, x0, 0];

            return (int)((f2 - f1) * (yc - y0) + f1);
        }

        private static int BilinearInterpolation(Image<Bgr, byte> f, double xc, double yc, int channel)
        {
            int x0 = (int)xc, y0 = (int)yc;
            int x1 = x0 + 1, y1 = y0 + 1;

            // calculate f(xc, y0)
            double f1 = (f.Data[y0, x1, channel] - f.Data[y0, x0, channel]) * (xc - x0) + f.Data[y0, x0, channel];

            // calculate f(xc, y0 + 1)
            double f2 = (f.Data[y1, x1, channel] - f.Data[y1, x0, channel]) * (xc - x0) + f.Data[y1, x0, channel];

            return (int)((f2 - f1) * (yc - y0) + f1);
        }
        #endregion

        #region Projective transformation

        private static double[,] ProjectiveTransformationMatrix(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            int height, int width)
        {
            // source P4'
            double[,] srcP4 = new double[3, 1]
                {
                    {x4},
                    {y4},
                    {1}
                };

            // determinant of P'
            double srcP_det = x1 * y2 + x2 * y3 + x3 * y1 - (x2 * y1 + x3 * y2 + x1 * y3);

            // inverse of P'
            double[,] srcP_inverse = new double[3, 3]
                {
                    {(y2 - y3) / srcP_det, (x3 - x2) / srcP_det, (x2 * y3 - x3 * y2) / srcP_det},
                    {(y3 - y1) / srcP_det, (x1 - x3) / srcP_det, (x3 * y1 - x1 * y3) / srcP_det},
                    {(y1 - y2) / srcP_det, (x2 - x1) / srcP_det, (x1 * y2 - x2 * y1) / srcP_det}
                };

            // result P4
            double[,] resP4 = new double[3, 1]
                {
                    {height - 1},
                    {0},
                    {1}
                };

            // inverse of P
            double[,] resP_inverse = new double[3, 3]
                {
                    {0, -1f / (width - 1), 1},
                    {-1f / (height - 1), 1f / (width - 1), 0},
                    {1f / (height - 1), 0, 0}
                };

            // calculate b'
            var srcB = Helper.Multiply(srcP_inverse, srcP4);

            // calculate b
            var resB = Helper.Multiply(resP_inverse, resP4);

            double h1 = (double)(srcB[0, 0] / resB[0, 0]);
            double h2 = (double)(srcB[1, 0] / resB[1, 0]);
            double h3 = (double)(srcB[2, 0] / resB[2, 0]);

            // calculate A
            var _ = new double[3, 3]
                    {
                        {h1 * x1, h2 * x2, h3 * x3},
                        {h1 * y1, h2 * y2, h3 * y3},
                        {h1, h2, h3}
                    };

            return Helper.Multiply(_, resP_inverse);
        }

        public static Image<Gray, byte> ProjectiveTransformation(Image<Gray, byte> inputImage,
            double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            Image<Gray, byte> borderedImage = Tools.Tools.BorderReplicate(inputImage, 2);

            int height = (int)Math.Max(
                Math.Sqrt(Math.Pow(x1 - x4, 2) + Math.Pow(y1 - y4, 2)),
                Math.Sqrt(Math.Pow(x2 - x3, 2) + Math.Pow(y2 - y3, 2))
                );

            int width = (int)Math.Max(
                Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)),
                Math.Sqrt(Math.Pow(x3 - x4, 2) + Math.Pow(y3 - y4, 2))
                );

            var transformationMatrix =
                ProjectiveTransformationMatrix(x1, y1, x2, y2, x3, y3, x4, y4, height, width);

            Image<Gray, byte> result = new Image<Gray, byte>(width, height);

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    var currentPoint = new double[3, 1]
                        {
                            {y},
                            {x},
                            {1}
                        };

                    var projectedPoint = Helper.Multiply(transformationMatrix, currentPoint);

                    double h = projectedPoint[2, 0];
                    int Y = (int)(projectedPoint[1, 0] / h + 0.5);
                    int X = (int)(projectedPoint[0, 0] / h + 0.5);

                    if (Y >= 0 && X >= 0 && Y < inputImage.Height && X < inputImage.Width)
                    {
                        result.Data[y, x, 0] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y) + 0.5);
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ProjectiveTransformation(Image<Bgr, byte> inputImage,
            double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            Image<Bgr, byte> borderedImage = Tools.Tools.BorderReplicate(inputImage, 2);

            int height = (int)Math.Max(
                Math.Sqrt(Math.Pow(x1 - x4, 2) + Math.Pow(y1 - y4, 2)),
                Math.Sqrt(Math.Pow(x2 - x3, 2) + Math.Pow(y2 - y3, 2))
                );

            int width = (int)Math.Max(
                Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)),
                Math.Sqrt(Math.Pow(x3 - x4, 2) + Math.Pow(y3 - y4, 2))
                );

            var transformationMatrix =
                ProjectiveTransformationMatrix(x1, y1, x2, y2, x3, y3, x4, y4, height, width);

            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    var currentPoint = new double[3, 1]
                        {
                            {y},
                            {x},
                            {1}
                        };

                    var projectedPoint = Helper.Multiply(transformationMatrix, currentPoint);

                    double h = projectedPoint[2, 0];
                    int Y = (int)(projectedPoint[1, 0] / h + 0.5);
                    int X = (int)(projectedPoint[0, 0] / h + 0.5);

                    if (Y >= 0 && X >= 0 && Y < inputImage.Height && X < inputImage.Width)
                    {
                        result.Data[y, x, 0] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 0) + 0.5);
                        result.Data[y, x, 1] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 1) + 0.5);
                        result.Data[y, x, 2] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 2) + 0.5);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}