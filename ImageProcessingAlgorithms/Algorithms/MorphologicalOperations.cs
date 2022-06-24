using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingAlgorithms.AlgorithmsHelper;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class MorphologicalOperations
    {
        #region Dilation
        public static Image<Gray, byte> DilationOnBinary(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int maskRad = maskDim / 2;

            for (int y = maskRad; y < inputImage.Height - maskRad; y++)
            {
                for (int x = maskRad; x < inputImage.Width - maskRad; x++)
                {
                    if (inputImage.Data[y, x, 0] == 255)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                    else
                    {
                        bool isDilated = false;
                        for (int i = y - maskRad; i <= y + maskRad && isDilated == false; ++i)
                        {
                            for (int j = x - maskRad; j <= x + maskRad && isDilated == false; ++j)
                            {
                                if (inputImage.Data[i, j, 0] == 255)
                                {
                                    result.Data[y, x, 0] = 255;
                                    isDilated = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static Image<Gray, byte> DilationOnGrayscale(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int maskRad = maskDim / 2;

            for (int y = maskRad; y < inputImage.Height - maskRad; y++)
            {
                for (int x = maskRad; x < inputImage.Width - maskRad; x++)
                {
                    List<byte> neighPixels = new List<byte>();
                    for (int i = y - maskRad; i <= y + maskRad; ++i)
                    {
                        for (int j = x - maskRad; j <= x + maskRad; ++j)
                        {
                            neighPixels.Add(inputImage.Data[i, j, 0]);
                        }
                    }

                    byte maxNeigh = 0;
                    foreach (var neighPixel in neighPixels)
                    {
                        if (maxNeigh < neighPixel)
                            maxNeigh = neighPixel;
                    }

                    result.Data[y, x, 0] = maxNeigh;
                }
            }

            return result;
        }
        #endregion

        #region Erosion
        public static Image<Gray, byte> ErosionOnBinary(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int maskRad = maskDim / 2;

            for (int y = maskRad; y < inputImage.Height - maskRad; y++)
            {
                for (int x = maskRad; x < inputImage.Width - maskRad; x++)
                {
                    if (inputImage.Data[y, x, 0] == 255)
                    {
                        bool isEroded = false;
                        for (int i = y - maskRad; i <= y + maskRad && isEroded == false; ++i)
                        {
                            for (int j = x - maskRad; j <= x + maskRad && isEroded == false; ++j)
                            {
                                if (inputImage.Data[i, j, 0] == 0)
                                {
                                    isEroded = true;
                                }
                            }
                        }

                        if (isEroded == false)
                        {
                            result.Data[y, x, 0] = 255;
                        }
                    }
                }
            }

            return result;
        }

        public static Image<Gray, byte> ErosionOnGrayscale(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int maskRad = maskDim / 2;

            for (int y = maskRad; y < inputImage.Height - maskRad; y++)
            {
                for (int x = maskRad; x < inputImage.Width - maskRad; x++)
                {
                    List<byte> neighPixels = new List<byte>();
                    for (int i = y - maskRad; i <= y + maskRad; ++i)
                    {
                        for (int j = x - maskRad; j <= x + maskRad; ++j)
                        {
                            neighPixels.Add(inputImage.Data[i, j, 0]);
                        }
                    }

                    byte minNeigh = 255;
                    foreach (var neighPixel in neighPixels)
                    {
                        if (minNeigh > neighPixel)
                            minNeigh = neighPixel;
                    }

                    result.Data[y, x, 0] = minNeigh;
                }
            }

            return result;
        }
        #endregion

        #region Opening
        public static Image<Gray, byte> OpeningOnBinary(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> erosion = ErosionOnBinary(inputImage, maskDim);
            return DilationOnBinary(erosion, maskDim);
        }

        public static Image<Gray, byte> OpeningOnGrayscale(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> erosion = ErosionOnGrayscale(inputImage, maskDim);
            return DilationOnGrayscale(erosion, maskDim);
        }
        #endregion

        #region Closing
        public static Image<Gray, byte> ClosingOnBinary(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> dilation = DilationOnBinary(inputImage, maskDim);
            return ErosionOnBinary(dilation, maskDim);
        }

        public static Image<Gray, byte> ClosingOnGrayscale(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> dilation = DilationOnGrayscale(inputImage, maskDim);
            return ErosionOnGrayscale(dilation, maskDim);
        }
        #endregion

        #region Connected components using Disjoint Sets

        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage)
        {
            int[,] labels = new int[inputImage.Height, inputImage.Width];

            DisjointSet<int> components = new DisjointSet<int>();

            int[] dy = { 0, -1, -1, -1 };
            int[] dx = { -1, -1, 0, 1 };

            int regionCounter = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] != 0)
                    {
                        int mainLabel = int.MaxValue;
                        HashSet<int> neighbours = new HashSet<int>();

                        for (int i = 0; i < 4; ++i)
                        {
                            int Y = y + dy[i];
                            int X = x + dx[i];

                            if (Y >= 0 && Y < inputImage.Height && X >= 0 && X < inputImage.Width)
                            {
                                int secondaryLabel = labels[Y, X];
                                if (secondaryLabel != 0)
                                {
                                    neighbours.Add(secondaryLabel);

                                    if (secondaryLabel < mainLabel)
                                        mainLabel = secondaryLabel;
                                }
                            }
                        }

                        if (mainLabel == int.MaxValue)
                        {
                            ++regionCounter;
                            components.MakeSet(regionCounter);
                            labels[y, x] = regionCounter;
                        }
                        else
                        {
                            labels[y, x] = mainLabel;

                            if (neighbours.Count > 1)
                            {
                                foreach (var label in neighbours)
                                {
                                    components.UnionSets(mainLabel, label);
                                }
                            }
                        }
                    }
                }
            }

            HashSet<int> usedLabels = new HashSet<int>();

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int currentLabel = labels[y, x];
                    if (currentLabel != 0)
                    {
                        labels[y, x] = components.FindSet(currentLabel);
                        usedLabels.Add(currentLabel);
                    }
                }
            }

            Dictionary<int, (byte, byte, byte)> colors = new Dictionary<int, (byte, byte, byte)>();
            System.Random random = new System.Random();

            foreach (var label in usedLabels)
            {
                colors[label] = Helper.GenerateRandomBgr(random);
            }

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    int currentLabel = labels[y, x];
                    if (currentLabel != 0)
                    {
                        result.Data[y, x, 0] = colors[currentLabel].Item1;
                        result.Data[y, x, 1] = colors[currentLabel].Item2;
                        result.Data[y, x, 2] = colors[currentLabel].Item3;
                    }
                }
            }

            return result;
        }
        #endregion

        #region Edge detecting using XOR operator
        public static Image<Gray, byte> XOR(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> erosion = ErosionOnBinary(inputImage, 3);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    bool inputPixel = false, erodedPixel = false;

                    if (inputImage.Data[y, x, 0] == 255)
                        inputPixel = true;
                    if (erosion.Data[y, x, 0] == 255)
                        erodedPixel = true;

                    result.Data[y, x, 0] =
                        (byte)(inputPixel ^ erodedPixel == true ? 255 : 0);
                }
            }

            return result;
        }
        #endregion

        #region Skeletonization

        #region 8 Masks algorithm
        private static bool CheckRegion(Image<Gray, byte> inputImage, int xPos, int yPos, int[,] mask)
        {
            bool isEqual = true;

            for (int y = yPos - 1; y <= yPos + 1 && isEqual == true; ++y)
            {
                for (int x = xPos - 1; x <= xPos + 1 && isEqual == true; ++x)
                {
                    if (mask[y - yPos + 1, x - xPos + 1] != -1)
                    {
                        if (inputImage.Data[y, x, 0] == 0 &&
                            mask[y - yPos + 1, x - xPos + 1] != 0)
                            isEqual = false;

                        if (inputImage.Data[y, x, 0] == 255 &&
                            mask[y - yPos + 1, x - xPos + 1] != 1)
                            isEqual = false;
                    }
                }
            }

            return isEqual;
        }

        public static Image<Gray, byte> Masks8(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            inputImage.CopyTo(result);

            List<int[,]> masks = new List<int[,]>
            {
                new int[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                new int[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                new int[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                new int[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                new int[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
                new int[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                new int[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                new int[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } }
            };

            bool isMarked;
            do
            {
                isMarked = false;

                for (int maskIndex = 0; maskIndex < 8; ++maskIndex)
                {
                    List<(int, int)> markedPixels = new List<(int, int)>();

                    for (int y = 1; y < result.Height - 1; ++y)
                    {
                        for (int x = 1; x < result.Width - 1; ++x)
                        {
                            if (CheckRegion(result, x, y, masks[maskIndex]))
                            {
                                markedPixels.Add((y, x));
                                isMarked = true;
                            }
                        }
                    }

                    foreach (var pixel in markedPixels)
                    {
                        result.Data[pixel.Item1, pixel.Item2, 0] = 0;
                    }
                }

            } while (isMarked != false);

            return result;
        }
        #endregion

        #region Zhang-Suen algorithm

        static readonly int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };
        static readonly int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };

        private static int WhitePixelsCounter(Image<Gray, byte> inputImage, int xPos, int yPos)
        {
            int numberOfWhitePixels = 0;

            for (int index = 0; index < 8; ++index)
            {
                int X = xPos + dx[index];
                int Y = yPos + dy[index];

                if (inputImage.Data[Y, X, 0] == 255)
                    ++numberOfWhitePixels;
            }

            return numberOfWhitePixels;
        }

        private static int BlackToWhiteCounter(Image<Gray, byte> inputImage, int xPos, int yPos)
        {
            int numberOfTransitions = 0;

            for (int index = 0; index < 8; ++index)
            {
                int currentX = xPos + dx[index];
                int currentY = yPos + dy[index];

                int nextX = xPos + dx[(index + 1) % 8];
                int nextY = yPos + dy[(index + 1) % 8];

                if (inputImage.Data[currentY, currentX, 0] == 0 &&
                    inputImage.Data[nextY, nextX, 0] == 255)
                    ++numberOfTransitions;
            }

            return numberOfTransitions;
        }

        public static Image<Gray, byte> ZhangSuen(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            inputImage.CopyTo(result);

            bool isMarked;
            do
            {
                isMarked = false;

                #region First step
                List<(int, int)> markedPixels = new List<(int, int)>();

                for (int y = 1; y < result.Height - 1; y++)
                {
                    for (int x = 1; x < result.Width - 1; ++x)
                    {
                        if (result.Data[y, x, 0] == 255)
                        {
                            if (BlackToWhiteCounter(result, x, y) != 1)
                                continue;

                            int whitePixels = WhitePixelsCounter(result, x, y);
                            if (whitePixels < 2 || whitePixels > 6)
                                continue;

                            int pixel2 = result.Data[y + dy[0], x + dx[0], 0];
                            int pixel4 = result.Data[y + dy[2], x + dx[2], 0];
                            int pixel6 = result.Data[y + dy[4], x + dx[4], 0];
                            int pixel8 = result.Data[y + dy[6], x + dx[6], 0];

                            if (pixel2 * pixel4 * pixel6 != 0)
                                continue;

                            if (pixel4 * pixel6 * pixel8 != 0)
                                continue;

                            markedPixels.Add((y, x));
                        }
                    }
                }

                if (markedPixels.Count > 0)
                    isMarked = true;

                foreach (var pixel in markedPixels)
                {
                    result.Data[pixel.Item1, pixel.Item2, 0] = 0;
                }
                #endregion

                #region Second step
                markedPixels.Clear();

                for (int y = 1; y < result.Height - 1; y++)
                {
                    for (int x = 1; x < result.Width - 1; ++x)
                    {
                        if (result.Data[y, x, 0] == 255)
                        {
                            if (BlackToWhiteCounter(result, x, y) != 1)
                                continue;

                            int whitePixels = WhitePixelsCounter(result, x, y);
                            if (whitePixels < 2 || whitePixels > 6)
                                continue;

                            int pixel2 = result.Data[y + dy[0], x + dx[0], 0];
                            int pixel4 = result.Data[y + dy[2], x + dx[2], 0];
                            int pixel6 = result.Data[y + dy[4], x + dx[4], 0];
                            int pixel8 = result.Data[y + dy[6], x + dx[6], 0];

                            if (pixel2 * pixel4 * pixel8 != 0)
                                continue;

                            if (pixel2 * pixel6 * pixel8 != 0)
                                continue;

                            markedPixels.Add((y, x));
                        }
                    }
                }

                if (markedPixels.Count > 0)
                    isMarked = true;

                foreach (var pixel in markedPixels)
                {
                    result.Data[pixel.Item1, pixel.Item2, 0] = 0;
                }
                #endregion

            }
            while (isMarked != false);

            return result;
        }
        #endregion

        #endregion

        #region Smoothing
        public static Image<Gray, byte> MorfologicSmoothing(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> opening = OpeningOnGrayscale(inputImage, maskDim);
            return ClosingOnGrayscale(opening, maskDim);
        }
        #endregion

        #region Gradient
        public static Image<Gray, byte> MorfologicGradient(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> dilaton = DilationOnGrayscale(inputImage, maskDim);
            Image<Gray, byte> erosion = ErosionOnGrayscale(inputImage, maskDim);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(dilaton.Data[y, x, 0] - erosion.Data[y, x, 0]);
                }
            }

            return result;
        }
        #endregion
    }
}