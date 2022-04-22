using ImageProcessingFramework.ViewModel;
using System.Windows;
using static ImageProcessingFramework.Model.DataProvider;

namespace ImageProcessingFramework.View
{
    public partial class MagnifierWindow : Window
    {
        private readonly MagnifierCommands MagnifierCommands;
        private Point LastPoint { get; set; }

        public MagnifierWindow()
        {
            InitializeComponent();
            MagnifierCommands = new MagnifierCommands();

            DisplayGray();
            DisplayColor();

            LastPoint = LastPosition;
        }

        private void DisplayColor()
        {
            if (ColorInitialImage != null)
                imageBoxOriginal.Source = MagnifierCommands.GetImage(ColorInitialImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (ColorProcessedImage != null)
                imageBoxProcessed.Source = MagnifierCommands.GetImage(ColorProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (GrayProcessedImage != null)
                imageBoxProcessed.Source = MagnifierCommands.GetImageGray(GrayProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
        }

        private void DisplayGray()
        {
            if (GrayInitialImage != null)
                imageBoxOriginal.Source = MagnifierCommands.GetImageGray(GrayInitialImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (ColorProcessedImage != null)
                imageBoxProcessed.Source = MagnifierCommands.GetImage(ColorProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (GrayProcessedImage != null)
                imageBoxProcessed.Source = MagnifierCommands.GetImageGray(GrayProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
        }

        private void MagnifierClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MagnifierOn = false;
        }

        private void MagnifierUpdate(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (LastPoint != LastPosition)
            {
                DisplayGray();
                DisplayColor();

                LastPoint = LastPosition;
            }
        }
    }
}