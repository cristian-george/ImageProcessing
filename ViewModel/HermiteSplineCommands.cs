using OxyPlot;
using OxyPlot.Axes;
using System.Collections.Generic;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace ImageProcessingFramework.ViewModel
{
    class HermiteSplineCommands
    {
        public PlotModel PlotImage { get; private set; }
        public DataPoint LastPoint { get; private set; }

        public string Xpos
        {
            get
            {
                return "X: " + ((int)LastPoint.X).ToString();
            }
        }

        public string Ypos
        {
            get
            {
                return "Y: " + ((int)LastPoint.Y).ToString();
            }
        }

        private LineSeries GenerateSeries(List<DataPoint> points, double markerSize, OxyColor color, LineStyle lineStyle)
        {
            var series = new LineSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = markerSize,
                LineStyle = lineStyle,
                MarkerStroke = color,
                MarkerFill = color,
                Color = color
            };

            for (int index = 0; index < points.Count; ++index)
                series.Points.Add(points[index]);

            return series;
        }

        public PlotModel Plot(double xMin, double yMin, double xMax, double yMax, bool isGraphView)
        {
            PlotImage = new PlotModel();
            PlotImage.MouseDown += MouseClickPressed;

            PlotImage.Series.Clear();
            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Maximum = xMax + 1,
                Minimum = xMin,
                IsZoomEnabled = false
            });

            PlotImage.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = yMax + 1,
                Minimum = yMin,
                IsZoomEnabled = false
            });

            DataPoint minPoint = new DataPoint(xMin, yMin);
            DataPoint maxPoint = new DataPoint(xMax, yMax);

            List<DataPoint> points = new List<DataPoint> { minPoint, maxPoint };
            PlotImage.Series.Add(GenerateSeries(points, 0.1, OxyColors.LightGray, LineStyle.Dot));

            if (isGraphView == true)
            {
                AddPointToPlot(minPoint);
                AddPointToPlot(maxPoint);
            }
            else
            {
                var splinePoints = PlotImage.Series;
            }
            //LineSeries hermiteSplineSeries = GenerateHermiteSeries();

            //PlotImage.Series.Add(hermiteSplineSeries);

            return PlotImage;
        }

        private void MouseClickPressed(object sender, OxyMouseDownEventArgs e)
        {
            DataPoint point = Axis.InverseTransform(e.Position, PlotImage.Axes[0], PlotImage.Axes[1]);

            if (CheckDataPoint(point, LastPoint.X, 255, 255))
            {
                LastPoint = point;
                AddPointToPlot(LastPoint);
            }
        }

        private bool CheckDataPoint(DataPoint point, double xMin, double xMax, double yMax)
        {
            return point.X > xMin && point.X < xMax && point.Y < yMax;
        }

        public void AddPointToPlot(DataPoint point)
        {
            LineSeries series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Red,
                MarkerFill = OxyColors.Red,
                Color = OxyColors.Red
            };

            series.Points.Add(point);

            PlotImage.Series.Add(series);
        }
    }
}
