﻿using Emgu.CV;
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
        public ImageSource InitialImage
        {
            get;
            set;
        }
        public ImageSource ProcessedImage
        {
            get;
            set;
        }

        private bool m_isInitialImageColor;
        private bool m_isProcessedImageGray;

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
            ClearUi(parameter);

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
            ClearUi(parameter);

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
            m_isInitialImageColor = true;
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

            if (ColorProcessedImage == null && GrayProcessedImage == null)
            {
                MessageBox.Show("Doesn't exist a processed image.");
                return;
            }

            switch (m_isInitialImageColor)
            {
                case true:
                    {
                        if (m_isProcessedImageGray == true)
                        {
                            m_isInitialImageColor = false;
                            m_isProcessedImageGray = false;

                            GrayInitialImage = GrayProcessedImage;
                            InitialImage = ImageConverter.Convert(GrayInitialImage);

                            ColorInitialImage = null;
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
                            MessageBox.Show("Doesn't exist a grayscale processed image.");
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

        #region Reset initial canvas
        private ICommand m_resetInitial;
        public ICommand ResetInitial
        {
            get
            {
                if (m_resetInitial == null)
                    m_resetInitial = new RelayCommand(ResetInitialCanvas);
                return m_resetInitial;
            }
        }

        public void ResetInitialCanvas(object parameter)
        {
            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
            OnPropertyChanged("InitialImage");

            m_isInitialImageColor = false;
        }
        #endregion

        #region Reset processed canvas
        private ICommand m_resetProcessed;
        public ICommand ResetProcessed
        {
            get
            {
                if (m_resetProcessed == null)
                    m_resetProcessed = new RelayCommand(ResetProcessedCanvas);
                return m_resetProcessed;
            }
        }

        public void ResetProcessedCanvas(object parameter)
        {
            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
            OnPropertyChanged("ProcessedImage");

            m_isProcessedImageGray = false;
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

        private static void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();

            MagnifierOn = false;
            GLevelsRowOn = false;
            GLevelsColumnOn = false;
            HermiteSplineOn = false;
        }

        public void ClearUi(object parameter)
        {
            RemoveAllDrawnElements(parameter);
            ResetInitialCanvas(parameter);
            ResetProcessedCanvas(parameter);

            ResetZoom(parameter);

            CloseAllWindows();
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
            if (GLevelsRowOn == true) return;

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
            if (GLevelsColumnOn == true) return;

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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

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
            ResetProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                m_isProcessedImageGray = true;
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
                return;
            }

            MessageBox.Show(ColorInitialImage != null
               ? "It is possible to convert only color images."
               : "Please add a color image first.");
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
            ResetProcessedCanvas(parameter);

            VectorOfRectangles.Add(DrawHelper.DrawRectangle(InitialCanvas, leftTopX, leftTopY, rightBottomX, rightBottomY, 3, Brushes.Red));
            SliderZoom.Value += 0.01; SliderZoom.Value -= 0.01;

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.CropImage(GrayInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.CropImage(ColorInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayInitialImage = Tools.RotateClockwise(GrayInitialImage);
                InitialImage = ImageConverter.Convert(GrayInitialImage);
                OnPropertyChanged("InitialImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorInitialImage = Tools.RotateClockwise(ColorInitialImage);
                InitialImage = ImageConverter.Convert(ColorInitialImage);
                OnPropertyChanged("InitialImage");
            }
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
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayInitialImage = Tools.RotateAntiClockwise(GrayInitialImage);
                InitialImage = ImageConverter.Convert(GrayInitialImage);
                OnPropertyChanged("InitialImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorInitialImage = Tools.RotateAntiClockwise(ColorInitialImage);
                InitialImage = ImageConverter.Convert(ColorInitialImage);
                OnPropertyChanged("InitialImage");
            }
        }
        #endregion

        #endregion

        #region Replicate padding
        private ICommand m_replicatePadding;
        public ICommand ReplicatePadding
        {
            get
            {
                if (m_replicatePadding == null)
                    m_replicatePadding = new RelayCommand(ReplicatePaddingMethod);
                return m_replicatePadding;
            }
        }

        public void ReplicatePaddingMethod(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
            {
                "Thickness value"
            };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int thickness = (int)response[0];
                if (thickness >= 0)
                {
                    if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.BorderReplicate(ColorInitialImage, thickness);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.BorderReplicate(GrayInitialImage, thickness);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add a valid thickness value first.");
            }
        }
        #endregion

        #region Adjust brightness and contrast

        #region Linear operators

        #region Increase brightness

        #region Operator +
        private ICommand m_increaseBrightnessPlus;
        public ICommand IncreaseBrightnessPlus
        {
            get
            {
                if (m_increaseBrightnessPlus == null)
                    m_increaseBrightnessPlus = new RelayCommand(BrightnessPlus);
                return m_increaseBrightnessPlus;
            }
        }

        public void BrightnessPlus(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "b value"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int b = (int)response[0];
                if (b >= 0 && b <= 255)
                {
                    int[] lookUpTable = Tools.IncreaseBrightnessPlus(b);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add a valid b value first.");
            }
        }
        #endregion

        #region Operator * (keep black)
        private ICommand m_increaseBrightnessKeepBlack;
        public ICommand IncreaseBrightnessKeepBlack
        {
            get
            {
                if (m_increaseBrightnessKeepBlack == null)
                    m_increaseBrightnessKeepBlack = new RelayCommand(IncBrightnessKeepBlack);
                return m_increaseBrightnessKeepBlack;
            }
        }

        public void IncBrightnessKeepBlack(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "a value (a > 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (a > 1)
                {
                    int[] lookUpTable = Tools.IncreaseBrightnessKeepBlack(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add a valid value first.");
            }
        }
        #endregion

        #region Operator * (keep white)
        private ICommand m_increaseBrightnessKeepWhite;
        public ICommand IncreaseBrightnessKeepWhite
        {
            get
            {
                if (m_increaseBrightnessKeepWhite == null)
                    m_increaseBrightnessKeepWhite = new RelayCommand(IncBrightnessMultiplicationKeepWhite);
                return m_increaseBrightnessKeepWhite;
            }
        }

        public void IncBrightnessMultiplicationKeepWhite(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "a value (0 < a < 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (0 < a && a < 1)
                {
                    int[] lookUpTable = Tools.IncreaseBrightnessKeepWhite(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add valid values first.");
            }
        }
        #endregion

        #endregion

        #region Decrease brightness

        #region Operator -
        private ICommand m_decreaseBrightnessMinus;
        public ICommand DecreaseBrightnessMinus
        {
            get
            {
                if (m_decreaseBrightnessMinus == null)
                    m_decreaseBrightnessMinus = new RelayCommand(BrightnessMinus);
                return m_decreaseBrightnessMinus;
            }
        }

        public void BrightnessMinus(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "b value"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int b = (int)response[0];
                if (b >= 0 && b <= 255)
                {
                    int[] lookUpTable = Tools.DecreaseBrightnessMinus(b);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add a valid b value first.");
            }
        }
        #endregion

        #region Operator * (keep black)
        private ICommand m_decreaseBrightnessKeepBlack;
        public ICommand DecreaseBrightnessKeepBlack
        {
            get
            {
                if (m_decreaseBrightnessKeepBlack == null)
                    m_decreaseBrightnessKeepBlack = new RelayCommand(DecrBrightnessKeepBlack);
                return m_decreaseBrightnessKeepBlack;
            }
        }

        public void DecrBrightnessKeepBlack(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "a value (0 < a < 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (0 < a && a < 1)
                {
                    int[] lookUpTable = Tools.DecreaseBrightnessKeepBlack(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add a valid value first.");
            }
        }
        #endregion

        #region Operator * (keep white)
        private ICommand m_decreaseBrightnessKeepWhite;
        public ICommand DecreaseBrightnessKeepWhite
        {
            get
            {
                if (m_decreaseBrightnessKeepWhite == null)
                    m_decreaseBrightnessKeepWhite = new RelayCommand(DecrBrightnessKeepWhite);
                return m_decreaseBrightnessKeepWhite;
            }
        }

        public void DecrBrightnessKeepWhite(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "a value (a > 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (a > 1)
                {
                    int[] lookUpTable = Tools.DecreaseBrightnessKeepWhite(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");
                    }
                }
                else MessageBox.Show("Please add valid values first.");
            }
        }
        #endregion

        #endregion

        #endregion

        #region Log operator
        private ICommand m_logOperator;
        public ICommand LogOperator
        {
            get
            {
                if (m_logOperator == null)
                    m_logOperator = new RelayCommand(LogarithmicOperator);
                return m_logOperator;
            }
        }

        public void LogarithmicOperator(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            int[] lookUpTable = Tools.LogarithmicOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
        }
        #endregion

        #region Inverse Log operator
        private ICommand m_expOperator;
        public ICommand ExpOperator
        {
            get
            {
                if (m_expOperator == null)
                    m_expOperator = new RelayCommand(ExponentialOperator);
                return m_expOperator;
            }
        }

        public void ExponentialOperator(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            int[] lookUpTable = Tools.ExponentialOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
        }
        #endregion

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

            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            SplineWindow splineShow = new SplineWindow();
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

            ResetProcessedCanvas(parameter);

            if (HermiteSplineLookUpTable != null)
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Tools.AdjustBrightnessAndContrast(GrayInitialImage, HermiteSplineLookUpTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = Tools.AdjustBrightnessAndContrast(ColorInitialImage, HermiteSplineLookUpTable);
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
                m_isProcessedImageGray = true;
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

        #region Median
        private ICommand m_median;
        public ICommand Median
        {
            get
            {
                if (m_median == null)
                    m_median = new RelayCommand(MedianFiltering);
                return m_median;
            }
        }

        public void MedianFiltering(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

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
                if (maskSize != 0 && maskSize % 2 == 1)
                {
                    if (ColorInitialImage != null)
                    {
                        m_isProcessedImageGray = true;
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Tools.MedianFiltering(GrayProcessedImage, maskSize);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.MedianFiltering(GrayInitialImage, maskSize);
                    }

                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);

                    OnPropertyChanged("ProcessedImage");
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

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

            ResetProcessedCanvas(parameter);

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
                if (maskSize > 2 && maskSize % 2 == 1)
                {
                    if (ColorInitialImage != null)
                    {
                        m_isProcessedImageGray = true;
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Tools.FastMedianFiltering(GrayProcessedImage, maskSize);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.FastMedianFiltering(GrayInitialImage, maskSize);
                    }

                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

        #region Gaussian bilateral
        private ICommand m_gaussianBilateral;
        public ICommand GaussianBilateral
        {
            get
            {
                if (m_gaussianBilateral == null)
                    m_gaussianBilateral = new RelayCommand(GaussianBilateralFiltering);
                return m_gaussianBilateral;
            }
        }

        public void GaussianBilateralFiltering(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Variance d",
                    "Variance r"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double variance_d = response[0];
                double variance_r = response[1];

                if (variance_d > 0 && variance_r > 0)
                {
                    if (ColorInitialImage != null)
                    {
                        m_isProcessedImageGray = true;
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Tools.GaussianBilateralFiltering(GrayProcessedImage, variance_d, variance_r);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.GaussianBilateralFiltering(GrayInitialImage, variance_d, variance_r);
                    }

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