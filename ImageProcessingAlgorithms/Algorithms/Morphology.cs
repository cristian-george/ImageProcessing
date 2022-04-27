using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingAlgorithms.AlgorithmsHelper;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.Algorithms
{
    public class Morphology
    {
        #region Dilation
        public static Image<Gray, byte> Dilation(Image<Gray, byte> inputImage, int maskDim)
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
        #endregion

        #region Erosion
        public static Image<Gray, byte> Erosion(Image<Gray, byte> inputImage, int maskDim)
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
        #endregion

        #region Opening
        public static Image<Gray, byte> Opening(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> erosion = Erosion(inputImage, maskDim);
            return Dilation(erosion, maskDim);
        }
        #endregion

        #region Closing
        public static Image<Gray, byte> Closing(Image<Gray, byte> inputImage, int maskDim)
        {
            Image<Gray, byte> dilation = Dilation(inputImage, maskDim);
            return Erosion(dilation, maskDim);
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
    }
}
