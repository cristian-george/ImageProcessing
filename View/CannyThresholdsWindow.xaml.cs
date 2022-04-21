using ImageProcessingAlgorithms.Filters;
using ImageProcessingFramework.Model;
using ImageProcessingFramework.ViewModel;
using System.Windows;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class CannySliders : Window
    {
        readonly MainCommands Commands;

        public CannySliders(object dataContext)
        {
            InitializeComponent();
            Commands = dataContext as MainCommands;
        }

        private void ModifyProcessedImage(int lowThreshold, int highThreshold)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.Canny(GrayInitialImage, lowThreshold, highThreshold);
                Commands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.Canny(ColorInitialImage, lowThreshold, highThreshold);
                Commands.ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }

        private void SliderLowThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLowThreshold != null && sliderHighThreshold != null)
            {
                int lowThreshold = (int)sliderLowThreshold.Value;
                int highThreshold = (int)sliderHighThreshold.Value;

                if (Commands != null && lowThreshold < highThreshold)
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

                if (Commands != null && lowThreshold < highThreshold)
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
