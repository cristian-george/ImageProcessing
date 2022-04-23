using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingAlgorithms.Filters;
using ImageProcessingFramework.Model;
using ImageProcessingFramework.ViewModel;
using System.Windows;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class CannySliders : Window
    {
        private readonly MainCommands Commands;
        private Image<Gray, byte> SmoothGrayImage;
        private Image<Bgr, byte> SmoothColorImage;

        public CannySliders(object dataContext)
        {
            InitializeComponent();
            Commands = dataContext as MainCommands;

            if (GrayInitialImage != null)
            {
                SmoothGrayImage = Filters.GaussianFiltering(GrayInitialImage, 1);
            }
            else if (ColorInitialImage != null)
            {
                SmoothColorImage = Filters.GaussianFiltering(ColorInitialImage, 1);
            }
        }

        private void ModifyProcessedImage(int lowThreshold, int highThreshold)
        {
            if (SmoothGrayImage != null)
            {
                GrayProcessedImage = Filters.Canny(SmoothGrayImage, lowThreshold, highThreshold);
                Commands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (SmoothColorImage != null)
            {
                GrayProcessedImage = Filters.Canny(SmoothColorImage, lowThreshold, highThreshold);
                Commands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }

        private void SliderLowThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLowThreshold != null && sliderHighThreshold != null)
            {
                int lowThreshold = (int)sliderLowThreshold.Value;
                int highThreshold = (int)sliderHighThreshold.Value;

                if (Commands != null && lowThreshold < highThreshold && highThreshold != 255)
                {
                    ModifyProcessedImage(lowThreshold, highThreshold);
                }
            }
        }

        private void SliderHighThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLowThreshold != null && sliderHighThreshold != null)
            {
                int lowThreshold = (int)sliderLowThreshold.Value;
                int highThreshold = (int)sliderHighThreshold.Value;

                if (Commands != null && lowThreshold < highThreshold && highThreshold != 255)
                {
                    ModifyProcessedImage(lowThreshold, highThreshold);
                }
            }
        }

        private void CannyWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CannyWindowOn = false;
        }
    }
}
