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

        public void SetAlgorithmToApply(Func<Image<Gray, byte>, int, Image<Gray, byte>> algorithm)
        {
            sliderVM.GrayToGrayAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Func<Image<Gray, byte>, int, Image<Bgr, byte>> algorithm)
        {
            sliderVM.GrayToColorAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Func<Image<Bgr, byte>, int, Image<Gray, byte>> algorithm)
        {
            sliderVM.ColorToGrayAlgorithm = algorithm;
        }

        public void SetAlgorithmToApply(Func<Image<Bgr, byte>, int, Image<Bgr, byte>> algorithm)
        {
            sliderVM.ColorToColorAlgorithm = algorithm;
        }

        public void ConfigureSlider(int minimumValue = 0, int maximumValue = 255, int defaultValue = 0, int frequency = 5)
        {
            slider.Minimum = minimumValue;
            slider.Maximum = maximumValue;
            slider.Value = defaultValue;
            slider.SmallChange = frequency;
            slider.TickFrequency = frequency;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)slider.Value;
            sliderVM.ModifyProcessedImage(value);
        }

        private void SliderWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SliderOn = false;
        }
    }
}