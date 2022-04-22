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
    class RowDisplayCommands
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

            for (int x = 0; x < colorImage.Width; x++)
                channelValues.Add(colorImage.Data[(int)DataProvider.LastPosition.Y, x, channel]);

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
                    series.Points.Add(new DataPoint(index, channelValues[index]));

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
                    series.Points.Add(new DataPoint(index, channelValues[index]));

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
                    series.Points.Add(new DataPoint(index, channelValues[index]));

                return series;
            }

            return null;
        }

        private LineSeries GenerateSerieForGray(Image<Gray, byte> grayImage, int channel, string color)
        {
            List<int> channelValues = new List<int>();

            for (int x = 0; x < grayImage.Width; x++)
                channelValues.Add(grayImage.Data[(int)DataProvider.LastPosition.Y, x, channel]);

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
                    series.Points.Add(new DataPoint(index, channelValues[index]));

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
                Position = AxisPosition.Bottom,
                Maximum = colorImage.Width + 30,
                Minimum = -1,
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 300,
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
                Position = AxisPosition.Bottom,
                Maximum = grayImage.Width + 30,
                Minimum = -1,
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 300,
                Minimum = -1,
            });

            LineSeries seriesGray = GenerateSerieForGray(grayImage, 0, "Gray");

            PlotImage.Series.Add(seriesGray);

            return PlotImage;
        }
    }
}