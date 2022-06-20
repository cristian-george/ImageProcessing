using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ImageProcessingFramework.View
{
    public partial class GraphWindow : Window
    {
        public GraphWindow(List<double> firstList, List<double> secondList)
        {
            InitializeComponent();
            graphView.Model = PlotGraph(firstList, secondList);
        }

        public PlotModel PlotImage { get; private set; }

        public PlotModel PlotGraph(List<double> firstList, List<double> secondList)
        {
            double firstMax = firstList.Max();
            double secondMax = secondList.Max();

            double firstMin = firstList.Min();
            double secondMin = firstList.Min();

            double maxim = firstMax > secondMax ? firstMax : secondMax;
            double minim = firstMin < secondMin ? firstMin : secondMin;

            PlotImage = new PlotModel();
            PlotImage.Series.Clear();
            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Maximum = maxim,
                Minimum = 0
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = maxim,
                Minimum = minim
            });

            var seriesRed = GenerateSeries(firstList, OxyColors.Red);
            var seriesBlue = GenerateSeries(secondList, OxyColors.Blue);

            PlotImage.Series.Add(seriesRed);
            PlotImage.Series.Add(seriesBlue);

            return PlotImage;
        }

        private LineSeries GenerateSeries(List<double> values, OxyColor color)
        {
            var series = new LineSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = 1,
                MarkerStroke = color,
                MarkerFill = color,
                Color = color
            };

            for (int index = 0; index < values.Count; ++index)
                series.Points.Add(new DataPoint(index, values[index]));

            return series;
        }
    }
}