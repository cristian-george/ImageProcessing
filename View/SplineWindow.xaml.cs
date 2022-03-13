using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ImageProcessingFramework.ViewModel;
using OxyPlot;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class SplineWindow : Window
    {
        private SplineCommands GraphViewCommands;
        private SplineCommands HermiteSplineCommands;

        public SplineWindow()
        {
            InitializeComponent();
            GraphViewCommands = new SplineCommands();
            graphView.Model = GraphViewCommands.InteractivePlot(0, 0, 255, 255);
        }

        private void WindowUpdate(object sender, System.Windows.Input.MouseEventArgs e)
        {
            xPos.Text = "X: " + ((int)GraphViewCommands.LastPoint.X).ToString();
            yPos.Text = "Y: " + ((int)GraphViewCommands.LastPoint.Y).ToString();
        }

        private void HermiteSplineClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HermiteSplineOn = false;
        }

        private void AddHermiteSpline(object sender, RoutedEventArgs e)
        {
            HermiteSplineCommands = new SplineCommands();
            splineView.Model = HermiteSplineCommands.CubicSplines(GraphViewCommands.Points);
        }

        private void RemoveHermiteSpline(object sender, RoutedEventArgs e)
        {
            if (splineView.Model != null)
            {
                splineView.Model.Series.Clear();
                splineView.Model.Axes.Clear();
                splineView.Model.InvalidatePlot(true);
            }

            HermiteSplineCommands = null;
        }

        private void ClearPlots(object sender, RoutedEventArgs e)
        {
            checkBox.IsChecked = false;
            RemoveHermiteSpline(sender, e);

            GraphViewCommands = new SplineCommands();
            graphView.Model = GraphViewCommands.InteractivePlot(0, 0, 255, 255);
        }

        private void ApplyEffect(object sender, RoutedEventArgs e)
        {
            if (HermiteSplineCommands != null)
            {
                List<int> frequenceOfKeys = new List<int>();
                List<double> sumOfValues = new List<double>();

                for (int key = 0; key < 256; ++key)
                {
                    frequenceOfKeys.Add(0);
                    sumOfValues.Add(0);
                }

                List<DataPoint> curvePoints = HermiteSplineCommands.GetCurvePoints();
                foreach (DataPoint point in curvePoints)
                {
                    ++frequenceOfKeys[(int)(point.X + 0.5)];
                    sumOfValues[(int)(point.X + 0.5)] += point.Y;
                }

                HermiteSplineLUT = new Collection<int>();
                for (int key = 0; key < 256; key++)
                {
                    int value = (int)sumOfValues[key] / frequenceOfKeys[key];
                    if (value > 255)
                        value = 255;

                    HermiteSplineLUT.Add(value);
                }
            }
        }
    }
}