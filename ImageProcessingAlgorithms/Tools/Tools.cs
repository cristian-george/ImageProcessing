using Emgu.CV;
using Emgu.CV.Structure;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;

namespace ImageProcessingAlgorithms.Tools
{
    public class Tools
    {
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

        #region Convert RGB to grayscale
        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            Image<Gray, byte> grayImage = coloredImage.Convert<Gray, byte>();
            return grayImage;
        }
        #endregion

        #region Crop image
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
    }
}