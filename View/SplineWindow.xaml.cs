using System.Windows;
using ImageProcessingFramework.ViewModel;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class SplineWindow : Window
    {
        private readonly SplineCommands GraphViewCommands;

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
            SplineCommands HermiteSplineCommands = new SplineCommands();
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
        }
    }
}
