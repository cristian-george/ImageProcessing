using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingFramework.Model;
using System;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.ViewModel
{
    class SliderVM : BaseVM
    {
        public MainCommands MainCommands;

        public Func<Image<Gray, byte>, int, Image<Gray, byte>> GrayToGrayAlgorithm = null;
        public Func<Image<Gray, byte>, int, Image<Bgr, byte>> GrayToColorAlgorithm = null;

        public Func<Image<Bgr, byte>, int, Image<Gray, byte>> ColorToGrayAlgorithm = null;
        public Func<Image<Bgr, byte>, int, Image<Bgr, byte>> ColorToColorAlgorithm = null;


        private string m_description;
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public void ModifyProcessedImage(int value)
        {
            GrayProcessedImage = null;
            ColorProcessedImage = null;

            if (GrayInitialImage != null)
            {
                if (GrayToGrayAlgorithm != null)
                {
                    GrayProcessedImage = GrayToGrayAlgorithm(GrayInitialImage, value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else
                {
                    ColorProcessedImage = GrayToColorAlgorithm(GrayInitialImage, value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else if (ColorInitialImage != null)
            {
                if (ColorToGrayAlgorithm != null)
                {
                    GrayProcessedImage = ColorToGrayAlgorithm(ColorInitialImage, value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else
                {
                    ColorProcessedImage = ColorToColorAlgorithm(ColorInitialImage, value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
        }
    }
}
