using System.Windows;
using ImageProcessingFramework.ViewModel;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class HermiteSplineWindow : Window
    {
        private readonly HermiteSplineCommands GraphViewCommands;
        private HermiteSplineCommands HermiteSplineCommands;

        public HermiteSplineWindow()
        {
            InitializeComponent();
            GraphViewCommands = new HermiteSplineCommands();

            graphView.Model = GraphViewCommands.Plot(0, 0, 255, 255, true);
        }

        private void WindowUpdate(object sender, System.Windows.Input.MouseEventArgs e)
        {
            xPos.Text = "X: " + ((int)GraphViewCommands.LastPoint.X).ToString();
            yPos.Text = "Y: " + ((int)GraphViewCommands.LastPoint.Y).ToString();
        }

        //private void DisplayGray()
        //{
        //    if (GrayInitialImage != null)
        //        originalImageView.Model = RowDisplayCommands.PlotGrayImage(GrayInitialImage);
        //    if (GrayProcessedImage != null)
        //        processedImageView.Model = RowDisplayCommands.PlotGrayImage(GrayProcessedImage);
        //}

        //private void DisplayColor()
        //{
        //    originalImageView.Model = RowDisplayCommands.PlotColorImage(ColorInitialImage);
        //    if (ColorProcessedImage != null)
        //        processedImageView.Model = RowDisplayCommands.PlotColorImage(ColorProcessedImage);
        //    checkBoxBlue.Visibility = Visibility.Visible;
        //    checkBoxGreen.Visibility = Visibility.Visible;
        //    checkBoxRed.Visibility = Visibility.Visible;

        //    DisplayGray();
        //}

        //private void AddBlueSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 0, true);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 0, true);
        //}

        //private void RemoveBlueSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 0, false);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 0, false);
        //}

        //private void AddGreenSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 1, true);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 1, true);
        //}

        //private void RemoveGreenSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 1, false);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 1, false);
        //}

        //private void AddRedSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 2, true);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 2, true);
        //}

        //private void RemoveRedSeries(object sender, RoutedEventArgs e)
        //{
        //    if (ColorInitialImage != null)
        //        SetVisibility(originalImageView.Model, 2, false);
        //    if (ColorProcessedImage != null)
        //        SetVisibility(processedImageView.Model, 2, false);
        //}

        //private void SetVisibility(PlotModel model, int indexSeries, bool isVisible)
        //{
        //    if (model != null)
        //    {
        //        model.Series[indexSeries].IsVisible = isVisible;
        //        model.InvalidatePlot(true);
        //    }
        //}

        private void HermiteSplineClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HermiteSplineOn = false;
        }

        private void AddHermiteSpline(object sender, RoutedEventArgs e)
        {
            HermiteSplineCommands = new HermiteSplineCommands();
            hermiteSplineView.Model = HermiteSplineCommands.Plot(0, 0, 255, 255, false);
        }

        private void RemoveHermiteSpline(object sender, RoutedEventArgs e)
        {
            hermiteSplineView.Model.Series.Clear();
            hermiteSplineView.Model.InvalidatePlot(true);
        }
    }
}
