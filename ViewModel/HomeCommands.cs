using Emgu.CV;
using Emgu.CV.Structure;
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
    class HomeCommands : NotifyPropertyChanged
    {
        public ImageSource InitialImage { get; set; }
        public ImageSource ProcessedImage { get; set; }

        private bool m_isColorImage;
        private bool m_isPressedConvertButton;

        #region Load grayscale image
        private ICommand m_loadGrayImage;
        public ICommand AddGrayImage
        {
            get
            {
                if (m_loadGrayImage == null)
                    m_loadGrayImage = new RelayCommand(LoadGrayImage);
                return m_loadGrayImage;
            }
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
        #endregion

        #region Load color image
        private ICommand m_loadColorImage;
        public ICommand AddColorImage
        {
            get
            {
                if (m_loadColorImage == null)
                    m_loadColorImage = new RelayCommand(LoadColorImage);
                return m_loadColorImage;
            }
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
        #endregion

        #region Save processed image
        private ICommand m_saveImage;
        public ICommand Save
        {
            get
            {
                if (m_saveImage == null)
                    m_saveImage = new RelayCommand(SaveImage);
                return m_saveImage;
            }
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
        #endregion

        #region Exit
        private ICommand m_exitWindow;
        public ICommand Exit
        {
            get
            {
                if (m_exitWindow == null)
                    m_exitWindow = new RelayCommand(ExitWindow);
                return m_exitWindow;
            }
        }

        public void ExitWindow(object parameter)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region Reset zoom
        private ICommand m_resetZoom;
        public ICommand Reset
        {
            get
            {
                if (m_resetZoom == null)
                    m_resetZoom = new RelayCommand(ResetZoom);
                return m_resetZoom;
            }
        }

        public void ResetZoom(object parameter)
        {
            if (parameter is Slider slider)
                slider.Value = 1;
            OnPropertyChanged("buttonResetZoom");
        }
        #endregion

        #region Save processed image as original image
        private ICommand m_saveAsOriginalImage;
        public ICommand SaveAsOriginalImage
        {
            get
            {
                if (m_saveAsOriginalImage == null)
                    m_saveAsOriginalImage = new RelayCommand(SaveAsOriginal);
                return m_saveAsOriginalImage;
            }
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
        #endregion

        #region Remove drawn elements
        private ICommand m_removeAllElements;
        public ICommand RemoveAllElements
        {
            get
            {
                if (m_removeAllElements == null)
                    m_removeAllElements = new RelayCommand(RemoveAllDrawnElements);
                return m_removeAllElements;
            }
        }

        public void RemoveAllDrawnElements(object parameter)
        {
            UiHelper.RemoveAllDrawnLines(InitialCanvas, ProcessedCanvas, VectorOfLines);
            UiHelper.RemoveAllDrawnRectangles(InitialCanvas, ProcessedCanvas, VectorOfRectangles);
            UiHelper.RemoveAllDrawnEllipses(InitialCanvas, ProcessedCanvas, VectorOfEllipses);
            UiHelper.RemoveAllDrawnPolygons(InitialCanvas, ProcessedCanvas, VectorOfPolygons);
        }
        #endregion

        #region Remove all elements
        private ICommand m_clearUi;
        public ICommand Clear
        {
            get
            {
                if (m_clearUi == null)
                    m_clearUi = new RelayCommand(ClearUi);
                return m_clearUi;
            }
        }

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

        private static void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();
        }

        private void ResetUiToInitial(object parameter)
        {
            RemoveAllDrawnElements(parameter);
            ResetInitialCanvas();
            ResetProcessedCanvas();

            m_isColorImage = false;
            m_isPressedConvertButton = false;
            MagnifierOn = false;
            GLevelsRowOn = false;
            HermiteSplineOn = false;

            ResetZoom(parameter);

            CloseAllWindows();
        }

        public void ClearUi(object parameter)
        {
            ResetUiToInitial(parameter);
            HermiteSplineLookUpTable = null;
        }
        #endregion

        #region Magnifier
        private ICommand m_magnifier;
        public ICommand Magnifier
        {
            get
            {
                if (m_magnifier == null)
                    m_magnifier = new RelayCommand(MagnifierShow);
                return m_magnifier;
            }
        }

        public void MagnifierShow(object parameter)
        {
            if (MagnifierOn == true) return;

            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
            MagnifierOn = true;
        }
        #endregion

        #region Gray levels
        #region On row
        private ICommand m_rowDisplay;
        public ICommand RowDisplay
        {
            get
            {
                if (m_rowDisplay == null)
                    m_rowDisplay = new RelayCommand(GrayLevelsRow);
                return m_rowDisplay;
            }
        }

        public void GrayLevelsRow(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (GLevelsRowOn == true) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            RowDisplayWindow rowDisplayWindow = new RowDisplayWindow();
            rowDisplayWindow.Show();
            GLevelsRowOn = true;
        }
        #endregion

        #region On column
        private ICommand m_columnDisplay;
        public ICommand ColumnDisplay
        {
            get
            {
                if (m_columnDisplay == null)
                    m_columnDisplay = new RelayCommand(GrayLevelsColumn);
                return m_columnDisplay;
            }
        }

        public void GrayLevelsColumn(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (GLevelsColumnOn == true) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColumnDisplayWindow columnDisplayWindow = new ColumnDisplayWindow();
            columnDisplayWindow.Show();
            GLevelsColumnOn = true;
        }
        #endregion
        #endregion

        #region Copy
        private ICommand m_copyImage;
        public ICommand Copy
        {
            get
            {
                if (m_copyImage == null)
                    m_copyImage = new RelayCommand(CopyImage);
                return m_copyImage;
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
        #endregion

        #region Invert
        private ICommand m_invertImage;
        public ICommand Invert
        {
            get
            {
                if (m_invertImage == null)
                    m_invertImage = new RelayCommand(InvertImage);
                return m_invertImage;
            }
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
        #endregion

        #region Transform image to grayscale
        private ICommand m_convertToGrayImage;
        public ICommand ConvertToGrayImage
        {
            get
            {
                if (m_convertToGrayImage == null)
                    m_convertToGrayImage = new RelayCommand(ConvertToGray);
                return m_convertToGrayImage;
            }
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
        #endregion

        #region Crop image
        private ICommand m_cropImage;
        public ICommand Crop
        {
            get
            {
                if (m_cropImage == null)
                    m_cropImage = new RelayCommand(CropImage);
                return m_cropImage;
            }
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
        #endregion

        #region Mirror image

        #region Mirror vertically
        private ICommand m_mirrorVertically;
        public ICommand MirrorVertically
        {
            get
            {
                if (m_mirrorVertically == null)
                    m_mirrorVertically = new RelayCommand(MirrorImageVertically);
                return m_mirrorVertically;
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
        #endregion

        #region Mirror horizontally
        private ICommand m_mirrorHorizontally;
        public ICommand MirrorHorizontally
        {
            get
            {
                if (m_mirrorHorizontally == null)
                    m_mirrorHorizontally = new RelayCommand(MirrorImageHorizontally);
                return m_mirrorHorizontally;
            }
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
        #endregion

        #endregion

        #region Rotate image

        #region Rotate clockwise
        private ICommand m_rotateClockwise;
        public ICommand RotateClockwise
        {
            get
            {
                if (m_rotateClockwise == null)
                    m_rotateClockwise = new RelayCommand(RotateImageClockwise);
                return m_rotateClockwise;
            }
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
        #endregion

        #region Rotate anti-clockwise
        private ICommand m_rotateAntiClockwise;
        public ICommand RotateAntiClockwise
        {
            get
            {
                if (m_rotateAntiClockwise == null)
                    m_rotateAntiClockwise = new RelayCommand(RotateImageAntiClockwise);
                return m_rotateAntiClockwise;
            }
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
        #endregion

        #endregion

        #region Adjust brightness and contrast

        #region Cubic Hermite spline

        #region Window
        private ICommand m_cubicHermiteSplineWindow;
        public ICommand HermiteSplineDisplay
        {
            get
            {
                if (m_cubicHermiteSplineWindow == null)
                    m_cubicHermiteSplineWindow = new RelayCommand(HermiteSplineShow);
                return m_cubicHermiteSplineWindow;
            }
        }

        private void HermiteSplineShow(object parameter)
        {
            if (HermiteSplineOn == true) return;
            var splineShow = new SplineWindow();
            splineShow.Show();
            HermiteSplineOn = true;
        }
        #endregion

        #region Interpolation
        private ICommand m_cubicHermiteSplineInterpolation;
        public ICommand HermiteSplineInterpolation
        {
            get
            {
                if (m_cubicHermiteSplineInterpolation == null)
                    m_cubicHermiteSplineInterpolation = new RelayCommand(HermiteSplineInterpolationImage);
                return m_cubicHermiteSplineInterpolation;
            }
        }

        public void HermiteSplineInterpolationImage(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (HermiteSplineLookUpTable != null)
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Tools.HermiteSplineInterpolation(GrayInitialImage, HermiteSplineLookUpTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = Tools.HermiteSplineInterpolation(ColorInitialImage, HermiteSplineLookUpTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
            }
            else MessageBox.Show("No look-up table available! Please compute it first.");
        }
        #endregion

        #endregion

        #endregion

        #region Thresholding

        #region Threshold is given as input
        private ICommand m_thresholding;
        public ICommand Thresholding
        {
            get
            {
                if (m_thresholding == null)
                    m_thresholding = new RelayCommand(ThresholdingImage);
                return m_thresholding;
            }
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
        #endregion

        #region Otsu two-threshold
        private ICommand m_otsuTwoThreshold;
        public ICommand OtsuTwoThreshold
        {
            get
            {
                if (m_otsuTwoThreshold == null)
                    m_otsuTwoThreshold = new RelayCommand(OtsuTwoThresholdSegmentation);
                return m_otsuTwoThreshold;
            }
        }

        public void OtsuTwoThresholdSegmentation(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.OtsuTwoThreshold(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else
            if (ColorInitialImage != null)
            {
                m_isPressedConvertButton = true;
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Tools.OtsuTwoThreshold(GrayProcessedImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else MessageBox.Show("No grayscale image!");
        }

        #endregion

        #endregion

        #region Low-pass filters

        #region Fast Median
        private ICommand m_fastMedian;
        public ICommand FastMedian
        {
            get
            {
                if (m_fastMedian == null)
                    m_fastMedian = new RelayCommand(FastMedianFiltering);
                return m_fastMedian;
            }
        }

        public void FastMedianFiltering(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Mask dimension"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize != 0)
                {
                    if (ColorInitialImage != null)
                    {
                        m_isPressedConvertButton = true;
                        GrayInitialImage = Tools.Convert(ColorInitialImage);
                    }

                    GrayProcessedImage = Tools.FastMedianFiltering(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);

                    OnPropertyChanged("ProcessedImage");
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

        #endregion

        #region High-pass filters

        #region
        #endregion

        #endregion
    }
}