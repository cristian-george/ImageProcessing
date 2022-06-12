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

        #region Scale
        public static Image<Gray, byte> Scale(Image<Gray, byte> inputImage, double sy, double sx)
        {
            Image<Gray, byte> result = new Image<Gray, byte>((int)(sx * inputImage.Width), (int)(sy * inputImage.Height));

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = y / sy;
                    double X = x / sx;

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y);
                    }
                    else
                    {
                        result.Data[y, x, 0] = inputImage.Data[(int)Y, (int)X, 0];
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Scale(Image<Bgr, byte> inputImage, double sy, double sx)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>((int)(sx * inputImage.Width), (int)(sy * inputImage.Height));

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = y / sy;
                    double X = x / sx;

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y, 0);
                        result.Data[y, x, 1] = (byte)BilinearInterpolation(inputImage, X, Y, 1);
                        result.Data[y, x, 2] = (byte)BilinearInterpolation(inputImage, X, Y, 2);
                    }
                    else
                    {
                        result.Data[y, x, 0] = inputImage.Data[(int)Y, (int)X, 0];
                        result.Data[y, x, 1] = inputImage.Data[(int)Y, (int)X, 1];
                        result.Data[y, x, 2] = inputImage.Data[(int)Y, (int)X, 2];
                    }
                }
            }

            return result;
        }
        #endregion

        #region Rotate
        public static Image<Gray, byte> Rotate(Image<Gray, byte> inputImage, double rotationAngle)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            double rotAnglRadians = rotationAngle * Math.PI / 180;

            double xCenter = inputImage.Width / 2;
            double yCenter = inputImage.Height / 2;

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = -(x - xCenter) * Math.Sin(rotAnglRadians) +
                                (y - yCenter) * Math.Cos(rotAnglRadians) + yCenter;

                    double X = (x - xCenter) * Math.Cos(rotAnglRadians) +
                                (y - yCenter) * Math.Sin(rotAnglRadians) + xCenter;

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y);
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Rotate(Image<Bgr, byte> inputImage, double rotationAngle)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            double rotAnglRadians = rotationAngle * Math.PI / 180;

            double xCenter = inputImage.Width / 2;
            double yCenter = inputImage.Height / 2;

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = -(x - xCenter) * Math.Sin(rotAnglRadians) +
                                (y - yCenter) * Math.Cos(rotAnglRadians) + yCenter;

                    double X = (x - xCenter) * Math.Cos(rotAnglRadians) +
                                (y - yCenter) * Math.Sin(rotAnglRadians) + xCenter;

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y, 0);
                        result.Data[y, x, 1] = (byte)BilinearInterpolation(inputImage, X, Y, 1);
                        result.Data[y, x, 2] = (byte)BilinearInterpolation(inputImage, X, Y, 2);
                    }
                }
            }

            return result;
        }
        #endregion

        #region Twirl transformation
        public static Image<Gray, byte> TwirlTransformation(Image<Gray, byte> inputImage,
            double rotationAngle, double maxRadius)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int y0 = inputImage.Height / 2;
            int x0 = inputImage.Width / 2;

            double rotAnglRadians = rotationAngle * Math.PI / 180;

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    int dy = y - y0;
                    int dx = x - x0;
                    double radius = Math.Sqrt(dy * dy + dx * dx);
                    double angle = Math.Atan2(dx, dy) + rotAnglRadians * ((maxRadius - radius) / maxRadius);

                    if (radius <= maxRadius)
                    {
                        double Y = y0 + radius * Math.Sin(angle);
                        double X = x0 + radius * Math.Cos(angle);

                        if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                        {
                            result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y);
                        }
                    }
                    else
                    {
                        result.Data[y, x, 0] = inputImage.Data[y, x, 0];
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> TwirlTransformation(Image<Bgr, byte> inputImage,
            double rotationAngle, double maxRadius)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            int y0 = inputImage.Height / 2;
            int x0 = inputImage.Width / 2;

            double rotAnglRadians = rotationAngle * Math.PI / 180;

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    int dy = y - y0;
                    int dx = x - x0;
                    double radius = Math.Sqrt(dy * dy + dx * dx);
                    double angle = Math.Atan2(dx, dy) + rotAnglRadians * ((maxRadius - radius) / maxRadius);

                    if (radius <= maxRadius)
                    {
                        double Y = y0 + radius * Math.Sin(angle);
                        double X = x0 + radius * Math.Cos(angle);

                        if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                        {
                            result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y, 0);
                            result.Data[y, x, 1] = (byte)BilinearInterpolation(inputImage, X, Y, 1);
                            result.Data[y, x, 2] = (byte)BilinearInterpolation(inputImage, X, Y, 2);
                        }
                    }
                    else
                    {
                        result.Data[y, x, 0] = inputImage.Data[y, x, 0];
                        result.Data[y, x, 1] = inputImage.Data[y, x, 1];
                        result.Data[y, x, 2] = inputImage.Data[y, x, 2];
                    }
                }
            }

            return result;
        }
        #endregion

        #region Ripple transformation
        public static Image<Gray, byte> RippleTransformation(Image<Gray, byte> inputImage,
            double tx, double ty, double ax, double ay)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = y + ay * Math.Sin(2 * Math.PI * x / ty);
                    double X = x + ax * Math.Sin(2 * Math.PI * y / tx);

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y);
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> RippleTransformation(Image<Bgr, byte> inputImage,
    double tx, double ty, double ax, double ay)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; ++x)
                {
                    double Y = y + ay * Math.Sin(2 * Math.PI * x / ty);
                    double X = x + ax * Math.Sin(2 * Math.PI * y / tx);

                    if (Y > 0 && Y < inputImage.Height - 1 && X > 0 && X < inputImage.Width - 1)
                    {
                        result.Data[y, x, 0] = (byte)BilinearInterpolation(inputImage, X, Y, 0);
                        result.Data[y, x, 1] = (byte)BilinearInterpolation(inputImage, X, Y, 1);
                        result.Data[y, x, 2] = (byte)BilinearInterpolation(inputImage, X, Y, 2);
                    }
                }
            }

            return result;
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
                    {0},
                    {height - 1},
                    {1}
                };

            // inverse of P
            double[,] resP_inverse = new double[3, 3]
                {
                    {-1f / (width - 1), 0, 1},
                    {1f / (width - 1), -1f / (height - 1), 0},
                    {0, 1f / (height - 1), 0}
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
                            {x},
                            {y},
                            {1}
                        };

                    var projectedPoint = Helper.Multiply(transformationMatrix, currentPoint);

                    double h = projectedPoint[2, 0];
                    double Y = projectedPoint[1, 0] / h + 0.5;
                    double X = projectedPoint[0, 0] / h + 0.5;

                    if (Y >= 0 && X >= 0 && Y < inputImage.Height && X < inputImage.Width)
                    {
                        result.Data[y, x, 0] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y));
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
                            {x},
                            {y},
                            {1}
                        };

                    var projectedPoint = Helper.Multiply(transformationMatrix, currentPoint);

                    double h = projectedPoint[2, 0];
                    double Y = projectedPoint[1, 0] / h + 0.5;
                    double X = projectedPoint[0, 0] / h + 0.5;

                    if (Y >= 0 && X >= 0 && Y < inputImage.Height && X < inputImage.Width)
                    {
                        result.Data[y, x, 0] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 0));
                        result.Data[y, x, 1] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 1));
                        result.Data[y, x, 2] =
                            (byte)Math.Min(255, BilinearInterpolation(borderedImage, X, Y, 2));
                    }
                }
            }

            return result;
        }

        #endregion
    }
}