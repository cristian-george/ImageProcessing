using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace ImageProcessingFramework.ViewModel
{
    internal class SplineCommands
    {
        private PlotModel Plot { get; set; }
        public DataPoint LastPoint { get; private set; }

        public List<DataPoint> Points;
        private readonly int MaximumNumberOfPoints = 9;

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
            LineSeries series = new LineSeries
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

        private PlotModel CreateOxyPlot(double xMin, double yMin, double xMax, double yMax)
        {
            PlotModel plotModel = new PlotModel();
            plotModel.Series.Clear();

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Maximum = xMax + 1,
                Minimum = xMin,
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = yMax + 1,
                Minimum = yMin
            });

            return plotModel;
        }

        public PlotModel InteractivePlot(double xMin, double yMin, double xMax, double yMax)
        {
            Plot = CreateOxyPlot(xMin, yMin, xMax, yMax);
            Plot.MouseDown += MouseClickPressed;

            Points = new List<DataPoint>
            {
                new DataPoint(0, 0),
                new DataPoint(255, 255)
            };

            DataPoint minPoint = new DataPoint(xMin, yMin);
            DataPoint maxPoint = new DataPoint(xMax, yMax);

            List<DataPoint> points = new List<DataPoint> { minPoint, maxPoint };
            Plot.Series.Add(GenerateSeries(points, 0.1, OxyColors.LightGray, LineStyle.Dash));

            AddPointToPlot(minPoint);
            AddPointToPlot(maxPoint);

            return Plot;
        }

        public PlotModel SplinePlot(double xMin, double yMin, double xMax, double yMax)
        {
            Plot = CreateOxyPlot(xMin, yMin, xMax, yMax);

            DataPoint minPoint = new DataPoint(xMin, yMin);
            DataPoint maxPoint = new DataPoint(xMax, yMax);

            List<DataPoint> points = new List<DataPoint> { minPoint, maxPoint };
            Plot.Series.Add(GenerateSeries(points, 0.1, OxyColors.LightGray, LineStyle.Dash));

            return Plot;
        }

        private void MouseClickPressed(object sender, OxyMouseDownEventArgs e)
        {
            if (Points.Count > MaximumNumberOfPoints) return;

            DataPoint point = Axis.InverseTransform(e.Position, Plot.Axes[0], Plot.Axes[1]);
            if (CheckDataPoint(point, LastPoint.X, 0, 255, 255))
            {
                Points.Add(point);

                LastPoint = point;
                AddPointToPlot(LastPoint);
            }
        }

        private bool CheckDataPoint(DataPoint point, double xMin, double yMin, double xMax, double yMax)
        {
            return point.X > xMin && point.Y > yMin && point.X < xMax && point.Y < yMax && Points.Count < MaximumNumberOfPoints;
        }

        private void AddPointToPlot(DataPoint point)
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
            Plot.Series.Add(series);
        }

        private DataPoint GetPseudoPoint(DataPoint point, DataPoint refPoint)
        {
            return new DataPoint((2 * refPoint.X) - point.X, (2 * refPoint.Y) - point.Y);
        }

        private double GetSlope(double tau, double inf, double sup)
        {
            return (double)tau * (sup - inf);
        }

        private double H00(double t)
        {
            return (2 * Math.Pow(t, 3)) - (3 * Math.Pow(t, 2)) + 1;
        }
        private double H10(double t)
        {
            return Math.Pow(t, 3) - (2 * Math.Pow(t, 2)) + t;
        }
        private double H01(double t)
        {
            return (-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2));
        }
        private double H11(double t)
        {
            return Math.Pow(t, 3) - Math.Pow(t, 2);
        }

        private double P(double t, double p1, double m1, double p2, double m2)
        {
            return (H00(t) * p1) + (H10(t) * m1) + (H01(t) * p2) + (H11(t) * m2);
        }

        private List<DataPoint> SplineDataPoints(List<double> slopesX, List<double> slopesY)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (int index = 1; index < Points.Count - 2; ++index)
            {
                for (double t = 0; t <= 1; t += 0.0001)
                {
                    dataPoints.Add(
                        new DataPoint(
                        P(t, Points[index].X, slopesX[index], Points[index + 1].X, slopesX[index + 1]),
                        P(t, Points[index].Y, slopesY[index], Points[index + 1].Y, slopesY[index + 1])
                        ));
                }
            }

            return dataPoints;
        }

        private void SetSlope(double tau, List<double> slopes)
        {
            for (int index = 1; index < Points.Count - 1; ++index)
            {
                double slopeX = GetSlope(tau, Points[index - 1].X, Points[index + 1].X);
                slopes.Add(slopeX);
            }
        }

        private void AddPseudoPoints()
        {
            Tuple<DataPoint, DataPoint> pseudoPoints = new Tuple<DataPoint, DataPoint>
                (
                    GetPseudoPoint(Points[1], Points[0]),
                    GetPseudoPoint(Points[Points.Count - 2], Points[Points.Count - 1])
                );

            Points.Add(pseudoPoints.Item1);
            Points.Add(pseudoPoints.Item2);
        }

        private void InitSplinePlot(List<DataPoint> GraphViewPoints)
        {
            Plot = SplinePlot(0, 0, 255, 255);
            Points = GraphViewPoints.OrderBy(point => point.X).ToList();

            AddPseudoPoints();
            Points = Points.OrderBy(point => point.X).ToList();
        }

        public PlotModel CubicSplines(List<DataPoint> GraphViewPoints)
        {
            InitSplinePlot(GraphViewPoints);

            List<double> slopesX = new List<double> { 0 };
            List<double> slopesY = new List<double> { 0 };
            SetSlope(0.5, slopesX);
            SetSlope(0.5, slopesY);

            List<DataPoint> seriesPoints = SplineDataPoints(slopesX, slopesY);
            Plot.Series.Add(GenerateSeries(seriesPoints, 5, OxyColors.Blue, LineStyle.Solid));

            return Plot;
        }
    }
}