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

        public Image<Gray, byte> GrayImage;
        public Image<Bgr, byte> ColorImage;

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

        private double m_minimumValue;
        public double MinimumValue
        {
            get
            {
                return m_minimumValue;
            }
            set
            {
                m_minimumValue = value;
                NotifyPropertyChanged("MinimumValue");
            }
        }

        private double m_maximumValue;
        public double MaximumValue
        {
            get
            {
                return m_maximumValue;
            }
            set
            {
                m_maximumValue = value;
                NotifyPropertyChanged("MaximumValue");
            }
        }

        private double m_value;
        public double Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
                NotifyPropertyChanged("Value");

                ModifyProcessedImage();
            }
        }

        private double m_frequency;
        public double Frequency
        {
            get
            {
                return m_frequency;
            }
            set
            {
                m_frequency = value;
                NotifyPropertyChanged("Frequency");
            }
        }

        public void ModifyProcessedImage()
        {
            GrayProcessedImage = null;
            ColorProcessedImage = null;

            if (GrayImage != null)
            {
                if (GrayToGrayAlgorithm != null)
                {
                    GrayProcessedImage = GrayToGrayAlgorithm(GrayImage, (int)Value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else
                {
                    ColorProcessedImage = GrayToColorAlgorithm(GrayImage, (int)Value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else if (ColorImage != null)
            {
                if (ColorToGrayAlgorithm != null)
                {
                    GrayProcessedImage = ColorToGrayAlgorithm(ColorImage, (int)Value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else
                {
                    ColorProcessedImage = ColorToColorAlgorithm(ColorImage, (int)Value);
                    MainCommands.ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
        }
    }
}