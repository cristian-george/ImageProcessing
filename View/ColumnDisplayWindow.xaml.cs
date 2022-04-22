using System.Windows;
using ImageProcessingFramework.ViewModel;
using OxyPlot;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework
{
    public partial class ColumnDisplayWindow : Window
    {
        private readonly ColumnDisplayCommands ColumnDisplayCommands;

        private Point LastPoint { get; set; }

        public ColumnDisplayWindow()
        {
            InitializeComponent();
            ColumnDisplayCommands = new ColumnDisplayCommands();

            DisplayGray();
            DisplayColor();

            LastPoint = LastPosition;
        }

        private void ColumnDisplayUpdate(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (LastPoint != LastPosition)
            {
                xPos.Text = "X: " + ((int)LastPosition.X).ToString();
                yPos.Text = "Y: " + ((int)LastPosition.Y).ToString();

                DisplayGray();
                DisplayColor();

                checkBoxBlue.IsChecked = true;
                checkBoxGreen.IsChecked = true;
                checkBoxRed.IsChecked = true;

                LastPoint = LastPosition;
            }
        }

        private void DisplayGray()
        {
            if (GrayInitialImage != null)
                originalImageView.Model = ColumnDisplayCommands.PlotGrayImage(GrayInitialImage);
            if (GrayProcessedImage != null)
                processedImageView.Model = ColumnDisplayCommands.PlotGrayImage(GrayProcessedImage);
        }

        private void DisplayColor()
        {
            if (ColorInitialImage != null)
                originalImageView.Model = ColumnDisplayCommands.PlotColorImage(ColorInitialImage);
            if (ColorProcessedImage != null)
                processedImageView.Model = ColumnDisplayCommands.PlotColorImage(ColorProcessedImage);

            checkBoxBlue.Visibility = Visibility.Visible;
            checkBoxGreen.Visibility = Visibility.Visible;
            checkBoxRed.Visibility = Visibility.Visible;

            DisplayGray();
        }

        private void AddBlueSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 0, true);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 0, true);
        }

        private void RemoveBlueSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 0, false);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 0, false);
        }

        private void AddGreenSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 1, true);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 1, true);
        }

        private void RemoveGreenSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 1, false);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 1, false);
        }

        private void AddRedSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 2, true);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 2, true);
        }

        private void RemoveRedSeries(object sender, RoutedEventArgs e)
        {
            if (ColorInitialImage != null)
                SetVisibility(originalImageView.Model, 2, false);
            if (ColorProcessedImage != null)
                SetVisibility(processedImageView.Model, 2, false);
        }

        private void SetVisibility(PlotModel model, int indexSeries, bool isVisible)
        {
            if (model != null)
            {
                model.Series[indexSeries].IsVisible = isVisible;
                model.InvalidatePlot(true);
            }
        }

        private void ColumnDisplayClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GLevelsColumnOn = false;
        }
    }
}