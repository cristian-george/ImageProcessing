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
                int[] frequenceOfKeys = new int[256];
                double[] sumOfValues = new double[256];

                List<DataPoint> curvePoints = HermiteSplineCommands.GetCurvePoints();
                foreach (DataPoint point in curvePoints)
                {
                    int pos = (int)(point.X + 0.5);
                    if (pos < 0) pos = 0;
                    else if (pos > 255) pos = 255;

                    ++frequenceOfKeys[pos];
                    sumOfValues[pos] += point.Y;
                }

                HermiteSplineLookUpTable = new int[256];
                for (int key = 0; key < 256; key++)
                {
                    int value = (int)((sumOfValues[key] / frequenceOfKeys[key]) + 0.5);
                    if (value < 0) value = 0;
                    else if (value > 255) value = 255;

                    HermiteSplineLookUpTable[key] = (byte)value;
                }
            }
        }
    }
}