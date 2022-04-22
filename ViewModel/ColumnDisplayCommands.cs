using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingFramework.Model;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.Generic;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace ImageProcessingFramework.ViewModel
{
    class ColumnDisplayCommands
    {
        public PlotModel PlotImage { get; private set; }

        public string Xpos
        {
            get
            {
                return "X: " + ((int)DataProvider.LastPosition.X).ToString();
            }
        }

        public string Ypos
        {
            get
            {
                return "Y: " + ((int)DataProvider.LastPosition.Y).ToString();
            }
        }

        private LineSeries GenerateSeriesForColor(Image<Bgr, byte> colorImage, int channel, string color)
        {
            List<int> channelValues = new List<int>();

            for (int y = 0; y < colorImage.Height; y++)
                channelValues.Add(colorImage.Data[y, (int)DataProvider.LastPosition.X, channel]);

            if (color.Equals("Blue"))
            {
                LineSeries series = new LineSeries
                {
                    MarkerType = MarkerType.None,
                    MarkerSize = 1,
                    MarkerStroke = OxyColors.Blue,
                    MarkerFill = OxyColors.Blue,
                    Color = OxyColors.Blue
                };

                for (int index = 0; index < channelValues.Count; ++index)
                    series.Points.Add(new DataPoint(channelValues[index], index));

                return series;
            }

            if (color.Equals("Green"))
            {
                LineSeries series = new LineSeries
                {
                    MarkerType = MarkerType.None,
                    MarkerSize = 1,
                    MarkerStroke = OxyColors.Green,
                    MarkerFill = OxyColors.Green,
                    Color = OxyColors.Green
                };

                for (int index = 0; index < channelValues.Count; ++index)
                    series.Points.Add(new DataPoint(channelValues[index], index));

                return series;
            }

            if (color.Equals("Red"))
            {
                LineSeries series = new LineSeries
                {
                    MarkerType = MarkerType.None,
                    MarkerSize = 1,
                    MarkerStroke = OxyColors.Red,
                    MarkerFill = OxyColors.Red,
                    Color = OxyColors.Red
                };

                for (int index = 0; index < channelValues.Count; ++index)
                    series.Points.Add(new DataPoint(channelValues[index], index));

                return series;
            }

            return null;
        }

        private LineSeries GenerateSerieForGray(Image<Gray, byte> grayImage, int channel, string color)
        {
            List<int> channelValues = new List<int>();

            for (int y = 0; y < grayImage.Height; y++)
                channelValues.Add(grayImage.Data[y, (int)DataProvider.LastPosition.X, channel]);

            if (color.Equals("Gray"))
            {
                LineSeries series = new LineSeries
                {
                    MarkerType = MarkerType.None,
                    MarkerSize = 1,
                    MarkerStroke = OxyColors.Red,
                    MarkerFill = OxyColors.Red,
                    Color = OxyColors.Red
                };

                for (int index = 0; index < channelValues.Count; ++index)
                    series.Points.Add(new DataPoint(channelValues[index], index));

                return series;
            }

            return null;
        }

        public PlotModel PlotColorImage(Image<Bgr, byte> colorImage)
        {
            PlotImage = new PlotModel();
            PlotImage.Series.Clear();

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Top,
                Maximum = 300,
                Minimum = -1,
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                StartPosition = 1,
                EndPosition = 0,
                Maximum = colorImage.Height + 30,
                Minimum = -1,
            });

            LineSeries seriesBlue = GenerateSeriesForColor(colorImage, 0, "Blue");
            LineSeries seriesGreen = GenerateSeriesForColor(colorImage, 1, "Green");
            LineSeries seriesRed = GenerateSeriesForColor(colorImage, 2, "Red");

            PlotImage.Series.Add(seriesBlue);
            PlotImage.Series.Add(seriesGreen);
            PlotImage.Series.Add(seriesRed);

            return PlotImage;
        }

        public PlotModel PlotGrayImage(Image<Gray, byte> grayImage)
        {
            PlotImage = new PlotModel();
            PlotImage.Series.Clear();

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Top,
                Maximum = 300,
                Minimum = -1,
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                StartPosition = 1,
                EndPosition = 0,
                Maximum = grayImage.Height + 30,
                Minimum = -1,
            });

            LineSeries seriesGray = GenerateSerieForGray(grayImage, 0, "Gray");

            PlotImage.Series.Add(seriesGray);

            return PlotImage;
        }
    }
}