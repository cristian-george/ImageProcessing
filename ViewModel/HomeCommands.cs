using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessingFramework.ViewModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ImageProcessingAlgorithms.Tools;
using ImageConverter = ImageProcessingFramework.Model.ImageConverter;
using static ImageProcessingFramework.Model.DataProvider;
using ImageProcessingFramework.View;
using ImageProcessingFramework.Model;

namespace ImageProcessingFramework.ViewModel
{
    class HomeCommands : INotifyPropertyChanged
    {
        public ImageSource InitialImage { get; set; }

        public ImageSource ProcessedImage { get; set; }

        private ICommand m_rowDisplay;
        private ICommand m_loadColorImage;
        private ICommand m_loadGrayImage;
        private ICommand m_exitWindow;
        private ICommand m_resetButton;
        private ICommand m_saveAsOriginalImage;
        private ICommand m_invertImage;
        private ICommand m_saveImage;
        private ICommand m_copyImage;
        private ICommand m_convertToGrayImage;
        private ICommand m_magnifier;
        private ICommand m_thresholding;
        private ICommand m_removeAllElements;
        private ICommand m_clearUi;
        private ICommand m_cropImage;
        private ICommand m_mirrorVertically;
        private ICommand m_mirrorHorizontally;
        private ICommand m_rotateClockwise;
        private ICommand m_rotateAntiClockwise;
        private ICommand m_cubicHermiteSpline;

        private bool m_isColorImage;
        private bool m_isPressedConvertButton;

        private void ResetInitialCanvas()
        {
            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
            OnPropertyChanged("InitialImage");
        }

        private void ResetProcessedCanvas()
        {
            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
            OnPropertyChanged("ProcessedImage");
        }

        private void ResetUiToInitial(object parameter)
        {
            RemoveAllDrawnElements(parameter);
            ResetInitialCanvas();
            ResetProcessedCanvas();

            m_isPressedConvertButton = false;
            MagnifierOn = false;
            GLevelsrowOn = false;

            ResetZoom(parameter);

            CloseAllWindows();
        }

        private static void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();
        }

        public void LoadColorImage(object parameter)
        {
            ResetUiToInitial(parameter);

            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "Image files(*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            ColorInitialImage = new Image<Bgr, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(ColorInitialImage);
            OnPropertyChanged("InitialImage");
            m_isColorImage = true;
        }

        public void LoadGrayImage(object parameter)
        {
            ResetUiToInitial(parameter);

            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "Image files(*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            GrayInitialImage = new Image<Gray, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(GrayInitialImage);
            OnPropertyChanged("InitialImage");
            m_isColorImage = false;
        }

        public void InvertImage(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else
                MessageBox.Show("Please add an image!");
        }

        public void SaveImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, please add and process an image first!");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                FileName = "image.jpg",
                Filter = "Image files(*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };

            saveFile.ShowDialog();

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                Encoder.Quality,
                (long)100
            );

            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == "image/jpeg"
                             select codec).Single();

            if (GrayProcessedImage != null)
                GrayProcessedImage.Bitmap.Save(saveFile.FileName, jpegCodec, encoderParams);

            if (ColorProcessedImage != null)
                ColorProcessedImage.Bitmap.Save(saveFile.FileName, jpegCodec, encoderParams);
        }

        public void ExitWindow(object parameter)
        {
            System.Environment.Exit(0);
        }

        public void ResetZoom(object parameter)
        {
            if (parameter is Slider slider)
                slider.Value = 1;
            OnPropertyChanged("buttonResetZoom");
        }

        public void SaveAsOriginal(object parameter)
        {
            RemoveAllDrawnElements(parameter);
            switch (m_isColorImage)
            {
                case true:
                    {
                        if (ColorProcessedImage == null && GrayProcessedImage == null)
                        {
                            MessageBox.Show("Doesn't exist processed image.");
                            return;
                        }

                        if (m_isPressedConvertButton == true)
                        {
                            GrayInitialImage = GrayProcessedImage;
                            InitialImage = ImageConverter.Convert(GrayInitialImage);
                            GrayProcessedImage = null;
                        }
                        else
                        {
                            ColorInitialImage = ColorProcessedImage;
                            InitialImage = ImageConverter.Convert(ColorInitialImage);
                            ColorProcessedImage = null;
                        }

                        ProcessedImage = null;
                        OnPropertyChanged("InitialImage");
                        OnPropertyChanged("ProcessedImage");
                        break;
                    }

                case false:
                    {
                        if (GrayProcessedImage == null)
                        {
                            MessageBox.Show("Doesn't exist processed image.");
                            return;
                        }

                        GrayInitialImage = GrayProcessedImage;
                        InitialImage = ImageConverter.Convert(GrayInitialImage);
                        GrayProcessedImage = null;
                        ProcessedImage = null;
                        OnPropertyChanged("InitialImage");
                        OnPropertyChanged("ProcessedImage");
                        break;
                    }
            }
        }

        public void CopyImage(object parameter)
        {
            if (m_isColorImage == true)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
                return;
            }
            else
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Tools.Copy(GrayInitialImage);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                    return;
                }
            }
            MessageBox.Show("Please add an image.");
        }

        public void ConvertToGray(object parameter)
        {
            if (m_isColorImage == true)
            {
                GrayInitialImage = Tools.Convert(ColorInitialImage);
                InitialImage = ImageConverter.Convert(GrayInitialImage);
                OnPropertyChanged("InitialImage");

                ColorInitialImage = null;
                ResetProcessedCanvas();
                m_isPressedConvertButton = true;
                m_isColorImage = false;
                return;
            }

            MessageBox.Show(ColorInitialImage != null
               ? "It is possible to convert only colored images."
               : "Please add a colored image first.");
        }

        public void GrayLevelsRow(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (GLevelsrowOn == true) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            var rowDisplayWindow = new RowDisplayWindow();
            rowDisplayWindow.Show();
            GLevelsrowOn = true;
        }

        public void MagnifierShow(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (MagnifierOn == true) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            var magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
            MagnifierOn = true;
        }

        private void HermiteSplineShow(object parameter)
        {
            //if (GrayInitialImage == null && ColorInitialImage == null)
            //{
            //    MessageBox.Show("Please add an image!");
            //    return;
            //}

            if (HermiteSplineOn == true) return;
            var splineShow = new SplineWindow();
            splineShow.Show();
            HermiteSplineOn = true;
        }

        public void ThresholdingImage(object parameter)
        {
            if (GrayInitialImage != null)
            {
                DialogBox dialogBox = new DialogBox();
                System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Threshold value"
                };

                dialogBox.CreateDialogBox(prop);
                dialogBox.ShowDialog();

                System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
                if (response != null)
                {
                    int threshold = (int)response[0];
                    if (threshold != 0)
                    {
                        GrayProcessedImage = Tools.Thresholding(GrayInitialImage, threshold);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else MessageBox.Show("Please add a threshold value first.");
                }
            }
            else MessageBox.Show("No grayscale image!");
        }

        public void CropImage(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (VectorOfMousePosition.Count <= 1)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            System.Windows.Point firstPosition = VectorOfMousePosition[VectorOfMousePosition.Count - 2];

            int leftTopX = (int)System.Math.Min(firstPosition.X, LastPosition.X);
            int leftTopY = (int)System.Math.Min(firstPosition.Y, LastPosition.Y);
            int rightBottomX = (int)System.Math.Max(firstPosition.X, LastPosition.X);
            int rightBottomY = (int)System.Math.Max(firstPosition.Y, LastPosition.Y);

            RemoveAllDrawnElements(parameter);
            VectorOfRectangles.Add(DrawHelper.DrawRectangle(InitialCanvas, leftTopX, leftTopY, rightBottomX, rightBottomY, 3, Brushes.Red));
            SliderZoom.Value += 0.01; SliderZoom.Value -= 0.01;

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.CropImage(GrayInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");

                MessageBox.Show("Mean: " + Tools.Mean(GrayProcessedImage) + "\n" +
                                "Standard deviation: " + Tools.StandardDeviation(GrayProcessedImage));
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.CropImage(ColorInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");

                MessageBox.Show("Mean: " + Tools.Mean(ColorProcessedImage) + "\n" +
                                "Standard deviation: " + Tools.StandardDeviation(ColorProcessedImage));
            }
        }

        public void MirrorImageVertically(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.MirrorVertically(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.MirrorVertically(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else MessageBox.Show("Please add an image!");
        }

        public void MirrorImageHorizontally(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.MirrorHorizontally(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.MirrorHorizontally(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else MessageBox.Show("Please add an image!");
        }

        public void RotateImageClockwise(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayInitialImage = Tools.RotateClockwise(GrayInitialImage);
                InitialImage = ImageConverter.Convert(GrayInitialImage);
                OnPropertyChanged("InitialImage");

                ResetProcessedCanvas();
            }
            else if (ColorInitialImage != null)
            {
                ColorInitialImage = Tools.RotateClockwise(ColorInitialImage);
                InitialImage = ImageConverter.Convert(ColorInitialImage);
                OnPropertyChanged("InitialImage");

                ResetProcessedCanvas();
            }
            else MessageBox.Show("Please add an image!");
        }

        public void RotateImageAntiClockwise(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayInitialImage = Tools.RotateAntiClockwise(GrayInitialImage);
                InitialImage = ImageConverter.Convert(GrayInitialImage);
                OnPropertyChanged("InitialImage");

                ResetProcessedCanvas();
            }
            else if (ColorInitialImage != null)
            {
                ColorInitialImage = Tools.RotateAntiClockwise(ColorInitialImage);
                InitialImage = ImageConverter.Convert(ColorInitialImage);
                OnPropertyChanged("InitialImage");

                ResetProcessedCanvas();
            }
            else MessageBox.Show("Please add an image!");
        }

        public void RemoveAllDrawnElements(object parameter)
        {
            UiHelper.RemoveAllDrawnLines(InitialCanvas, ProcessedCanvas, VectorOfLines);
            UiHelper.RemoveAllDrawnRectangles(InitialCanvas, ProcessedCanvas, VectorOfRectangles);
            UiHelper.RemoveAllDrawnEllipses(InitialCanvas, ProcessedCanvas, VectorOfEllipses);
            UiHelper.RemoveAllDrawnPolygons(InitialCanvas, ProcessedCanvas, VectorOfPolygons);
        }

        public void ClearUi(object parameter)
        {
            RemoveAllDrawnElements(parameter);
            ResetUiToInitial(parameter);
        }

        public ICommand AddColorImage
        {
            get
            {
                if (m_loadColorImage == null)
                    m_loadColorImage = new RelayCommand(LoadColorImage);
                return m_loadColorImage;
            }
        }

        public ICommand AddGrayImage
        {
            get
            {
                if (m_loadGrayImage == null)
                    m_loadGrayImage = new RelayCommand(LoadGrayImage);
                return m_loadGrayImage;
            }
        }

        public ICommand Exit
        {
            get
            {
                if (m_exitWindow == null)
                    m_exitWindow = new RelayCommand(ExitWindow);
                return m_exitWindow;
            }

        }

        public ICommand Reset
        {
            get
            {
                if (m_resetButton == null)
                    m_resetButton = new RelayCommand(ResetZoom);
                return m_resetButton;
            }
        }

        public ICommand SaveAsOriginalImage
        {
            get
            {
                if (m_saveAsOriginalImage == null)
                    m_saveAsOriginalImage = new RelayCommand(SaveAsOriginal);
                return m_saveAsOriginalImage;
            }
        }

        public ICommand RemoveAllElements
        {
            get
            {
                if (m_removeAllElements == null)
                    m_removeAllElements = new RelayCommand(RemoveAllDrawnElements);
                return m_removeAllElements;
            }
        }

        public ICommand Clear
        {
            get
            {
                if (m_clearUi == null)
                    m_clearUi = new RelayCommand(ClearUi);
                return m_clearUi;
            }
        }

        public ICommand Invert
        {
            get
            {
                if (m_invertImage == null)
                    m_invertImage = new RelayCommand(InvertImage);
                return m_invertImage;
            }
        }

        public ICommand Copy
        {
            get
            {
                if (m_copyImage == null)
                    m_copyImage = new RelayCommand(CopyImage);
                return m_copyImage;
            }
        }

        public ICommand Save
        {
            get
            {
                if (m_saveImage == null)
                    m_saveImage = new RelayCommand(SaveImage);
                return m_saveImage;
            }
        }

        public ICommand ConvertToGrayImage
        {
            get
            {
                if (m_convertToGrayImage == null)
                    m_convertToGrayImage = new RelayCommand(ConvertToGray);
                return m_convertToGrayImage;
            }
        }

        public ICommand RowDisplay
        {
            get
            {
                if (m_rowDisplay == null)
                    m_rowDisplay = new RelayCommand(GrayLevelsRow);
                return m_rowDisplay;
            }
        }

        public ICommand Magnifier
        {
            get
            {
                if (m_magnifier == null)
                    m_magnifier = new RelayCommand(MagnifierShow);
                return m_magnifier;
            }
        }

        public ICommand Thresholding
        {
            get
            {
                if (m_thresholding == null)
                    m_thresholding = new RelayCommand(ThresholdingImage);
                return m_thresholding;
            }
        }

        public ICommand Crop
        {
            get
            {
                if (m_cropImage == null)
                    m_cropImage = new RelayCommand(CropImage);
                return m_cropImage;
            }
        }

        public ICommand MirrorVertically
        {
            get
            {
                if (m_mirrorVertically == null)
                    m_mirrorVertically = new RelayCommand(MirrorImageVertically);
                return m_mirrorVertically;
            }
        }

        public ICommand MirrorHorizontally
        {
            get
            {
                if (m_mirrorHorizontally == null)
                    m_mirrorHorizontally = new RelayCommand(MirrorImageHorizontally);
                return m_mirrorHorizontally;
            }
        }

        public ICommand RotateClockwise
        {
            get
            {
                if (m_rotateClockwise == null)
                    m_rotateClockwise = new RelayCommand(RotateImageClockwise);
                return m_rotateClockwise;
            }
        }

        public ICommand RotateAntiClockwise
        {
            get
            {
                if (m_rotateAntiClockwise == null)
                    m_rotateAntiClockwise = new RelayCommand(RotateImageAntiClockwise);
                return m_rotateAntiClockwise;
            }
        }

        public ICommand HermiteSplineDisplay
        {
            get
            {
                if (m_cubicHermiteSpline == null)
                    m_cubicHermiteSpline = new RelayCommand(HermiteSplineShow);
                return m_cubicHermiteSpline;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}