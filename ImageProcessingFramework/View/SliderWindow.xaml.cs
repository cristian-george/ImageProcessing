using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingFramework.ViewModel;
using System;
using System.Windows;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class SliderWindow : Window
    {
        private readonly SliderVM sliderVM;

        public SliderWindow(object mainContext, object description)
        {
            InitializeComponent();

            sliderVM = new SliderVM
            {
                MainCommands = mainContext as MainCommands,
                Description = description as string
            };

            DataContext = sliderVM;
        }

        public void SetAlgorithmToApply(Image<Gray, byte> image, Func<Image<Gray, byte>, double, Image<Gray, byte>> algorithm)
        {
            sliderVM.GrayImage = image;
            sliderVM.GrayToGrayAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Image<Gray, byte> image, Func<Image<Gray, byte>, double, Image<Bgr, byte>> algorithm)
        {
            sliderVM.GrayImage = image;
            sliderVM.GrayToColorAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Image<Bgr, byte> image, Func<Image<Bgr, byte>, double, Image<Gray, byte>> algorithm)
        {
            sliderVM.ColorImage = image;
            sliderVM.ColorToGrayAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Image<Bgr, byte> image, Func<Image<Bgr, byte>, double, Image<Bgr, byte>> algorithm)
        {
            sliderVM.ColorImage = image;
            sliderVM.ColorToColorAlgorithm = algorithm;
        }

        public void ConfigureSlider(double minimumValue = 0, double maximumValue = 255, double value = 0, double frequency = 5)
        {
            sliderVM.MinimumValue = minimumValue;
            sliderVM.MaximumValue = maximumValue;
            sliderVM.Value = value;
            sliderVM.Frequency = frequency;
        }

        private void SliderWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SliderOn = false;
        }
    }
}