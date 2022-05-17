using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ImageProcessingAlgorithms.Tools;
using ImageProcessingAlgorithms.Algorithms;
using ImageProcessingAlgorithms.AlgorithmsHelper;
using ImageConverter = ImageProcessingFramework.Model.ImageConverter;
using static ImageProcessingFramework.Model.DataProvider;
using ImageProcessingFramework.View;
using ImageProcessingFramework.Model;

namespace ImageProcessingFramework.ViewModel
{
    class MainCommands : NotifyPropertyChanged
    {
        private ImageSource m_initialImage;
        public ImageSource InitialImage
        {
            get
            {
                return m_initialImage;
            }
            set
            {
                m_initialImage = value;
                OnPropertyChanged("InitialImage");
            }
        }

        private ImageSource m_processedImage;
        public ImageSource ProcessedImage
        {
            get
            {
                return m_processedImage;
            }
            set
            {
                m_processedImage = value;
                OnPropertyChanged("ProcessedImage");
            }
        }

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
                Title = "Select a gray picture",
                Filter = "Image files (*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            GrayInitialImage = new Image<Gray, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(GrayInitialImage);
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
                Title = "Select a color picture",
                Filter = "Image files (*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            ColorInitialImage = new Image<Bgr, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(ColorInitialImage);
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

            ResetInitialCanvas(parameter);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = ImageConverter.Convert(GrayInitialImage);

                GrayProcessedImage = null;
                ProcessedImage = null;
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = ImageConverter.Convert(ColorInitialImage);

                ColorProcessedImage = null;
                ProcessedImage = null;
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
            CannyWindowOn = false;
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
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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

            if (VectorOfMousePosition.Count < 2)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            System.Windows.Point firstPosition = VectorOfMousePosition[VectorOfMousePosition.Count - 2];

            double leftTopX = System.Math.Min(firstPosition.X, LastPosition.X);
            double leftTopY = System.Math.Min(firstPosition.Y, LastPosition.Y);
            double rightBottomX = System.Math.Max(firstPosition.X, LastPosition.X);
            double rightBottomY = System.Math.Max(firstPosition.Y, LastPosition.Y);

            RemoveAllDrawnElements(parameter);
            ResetProcessedCanvas(parameter);

            VectorOfRectangles.Add(DrawHelper.DrawRectangle(InitialCanvas, leftTopX, leftTopY, rightBottomX, rightBottomY, 1, Brushes.Red));

            if (SliderZoom.Value == SliderZoom.Minimum)
            {
                SliderZoom.Value += 0.01;
                SliderZoom.Value -= 0.01;
            }
            else
            {
                SliderZoom.Value -= 0.01;
                SliderZoom.Value += 0.01;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.CropImage(GrayInitialImage,
                    (int)(leftTopX + 0.5),
                    (int)(leftTopY + 0.5),
                    (int)(rightBottomX + 0.5),
                    (int)(rightBottomY + 0.5));

                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.CropImage(ColorInitialImage,
                    (int)(leftTopX + 0.5),
                    (int)(leftTopY + 0.5),
                    (int)(rightBottomX + 0.5),
                    (int)(rightBottomY + 0.5));

                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.MirrorVertically(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.MirrorHorizontally(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.BorderReplicate(GrayInitialImage, thickness);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.IncreaseBrightnessPlus(b);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.IncreaseBrightnessKeepBlack(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.IncreaseBrightnessKeepWhite(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.DecreaseBrightnessMinus(b);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.DecreaseBrightnessKeepBlack(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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
                    int[] lookUpTable = PunctualOperators.DecreaseBrightnessKeepWhite(a);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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

            int[] lookUpTable = PunctualOperators.LogarithmicOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
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

            int[] lookUpTable = PunctualOperators.ExponentialOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Gamma operator
        private ICommand m_gammaOperator;
        public ICommand GammaOperator
        {
            get
            {
                if (m_gammaOperator == null)
                    m_gammaOperator = new RelayCommand(GammaCorrection);
                return m_gammaOperator;
            }
        }

        public void GammaCorrection(object parameter)
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
                    "Gamma value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double gamma = response[0];
                if (gamma > 0)
                {
                    int[] lookUpTable = PunctualOperators.GammaCorrection(gamma);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    }
                }
                else MessageBox.Show("Please add a valid b value first.");
            }
        }
        #endregion

        #region Piecewise linear contrast
        private ICommand m_piecewiseLinearContrast;
        public ICommand PiecewiseLinearContrast
        {
            get
            {
                if (m_piecewiseLinearContrast == null)
                    m_piecewiseLinearContrast = new RelayCommand(PiecewiseLinearOperator);
                return m_piecewiseLinearContrast;
            }
        }

        public void PiecewiseLinearOperator(object parameter)
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
                    "r1:",
                    "s1:",
                    "r2:",
                    "s2"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int r1 = (int)response[0];
                int s1 = (int)response[1];
                int r2 = (int)response[2];
                int s2 = (int)response[3];

                if (0 <= r1 && r1 <= r2 && r2 <= 255 &&
                    0 <= s1 && s1 <= s2 && s2 <= 255)
                {
                    int[] lookUpTable = PunctualOperators.PiecewiseLinearContrast(r1, s1, r2, s2);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    }
                }
                else MessageBox.Show("Please add valid values first " +
                    "(0 <= r1 <= r2 <= 255 and " +
                    "0 <= s1 <= s2 <= 255).");
            }
        }
        #endregion

        #region Non-linear operators

        #region Sinusoidal operator
        private ICommand m_sinusoidalOp;
        public ICommand SinusoidalOp
        {
            get
            {
                if (m_sinusoidalOp == null)
                    m_sinusoidalOp = new RelayCommand(SinusoidalOperator);
                return m_sinusoidalOp;
            }
        }

        public void SinusoidalOperator(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            int[] lookUpTable = PunctualOperators.SinusoidalOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Polynomial operator
        private ICommand m_polynomialOp;
        public ICommand PolynomialOp
        {
            get
            {
                if (m_polynomialOp == null)
                    m_polynomialOp = new RelayCommand(PolynomialOperator);
                return m_polynomialOp;
            }
        }

        public void PolynomialOperator(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            int[] lookUpTable = PunctualOperators.PolynomialOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region EM - operator
        private ICommand m_emOp;
        public ICommand EmOp
        {
            get
            {
                if (m_emOp == null)
                    m_emOp = new RelayCommand(EmOperator);
                return m_emOp;
            }
        }

        public void EmOperator(object parameter)
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
                    "m value:",
                    "E value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double m = response[0];
                double E = response[1];
                if (m != 0 && E != 0)
                {
                    int[] lookUpTable = PunctualOperators.EmOperator(m, E);
                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Helper.AdjustBrightnessAndContrast(ColorInitialImage, lookUpTable);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    }
                }
                else MessageBox.Show("Please add valid values first.");
            }
        }
        #endregion

        #endregion

        #region Cubic Hermite spline

        private ICommand m_cubicHermite;
        public ICommand SplineInterpolation
        {
            get
            {
                if (m_cubicHermite == null)
                    m_cubicHermite = new RelayCommand(HermiteSpline);
                return m_cubicHermite;
            }
        }

        public void HermiteSpline(object parameter)
        {
            if (HermiteSplineOn == true) return;

            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            SplineWindow splineShow = new SplineWindow(this);
            splineShow.Show();
            HermiteSplineOn = true;
        }
        #endregion

        #region Histogram equalization
        private ICommand m_histogramEq;
        public ICommand HistogramEq
        {
            get
            {
                if (m_histogramEq == null)
                    m_histogramEq = new RelayCommand(HistogramEqualization);
                return m_histogramEq;
            }
        }

        public void HistogramEqualization(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                int[] lookUpTable = PunctualOperators.HistogramEqualization(GrayInitialImage);
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else
            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);

                int[] lookUpTable = PunctualOperators.HistogramEqualization(GrayProcessedImage);
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayProcessedImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Color histogram equalization
        private ICommand m_colorHistogramEq;
        public ICommand ColorHistogramEq
        {
            get
            {
                if (m_colorHistogramEq == null)
                    m_colorHistogramEq = new RelayCommand(ColorHistogramEqualization);
                return m_colorHistogramEq;
            }
        }

        public void ColorHistogramEqualization(object parameter)
        {
            if (ColorInitialImage == null)
            {
                MessageBox.Show("Please add a color image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            ColorProcessedImage = PunctualOperators.ColorHistogramEqualization(ColorInitialImage);
            ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
        }
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
                        GrayProcessedImage = ThresholdOperators.Thresholding(GrayInitialImage, threshold);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else MessageBox.Show("Please add a threshold value first.");
                }
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Quantile threshold
        private ICommand m_quantileThreshold;
        public ICommand QuantileThreshold
        {
            get
            {
                if (m_quantileThreshold == null)
                    m_quantileThreshold = new RelayCommand(QuantileThresholding);
                return m_quantileThreshold;
            }
        }

        public void QuantileThresholding(object parameter)
        {
            if (GrayInitialImage != null)
            {
                DialogBox dialogBox = new DialogBox();
                System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Background pixels percent (0 <= p <= 1):"
                };

                dialogBox.CreateDialogBox(prop);
                dialogBox.ShowDialog();

                System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
                if (response != null)
                {
                    double percent = response[0];
                    if (0 <= percent && percent <= 1)
                    {
                        int threshold = ThresholdOperators.QuantileThreshold(GrayInitialImage, percent);
                        GrayProcessedImage = ThresholdOperators.Thresholding(GrayInitialImage, threshold);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
                    else MessageBox.Show("Please add a threshold value first.");
                }
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Median threshold
        private ICommand m_medianThreshold;
        public ICommand MedianThreshold
        {
            get
            {
                if (m_medianThreshold == null)
                    m_medianThreshold = new RelayCommand(MedianThresholding);
                return m_medianThreshold;
            }
        }

        public void MedianThresholding(object parameter)
        {
            if (GrayInitialImage != null)
            {
                int threshold = ThresholdOperators.MedianThreshold(GrayInitialImage);
                GrayProcessedImage = ThresholdOperators.Thresholding(GrayInitialImage, threshold);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Intermeans threshold
        private ICommand m_intermeansThreshold;
        public ICommand IntermeansThreshold
        {
            get
            {
                if (m_intermeansThreshold == null)
                    m_intermeansThreshold = new RelayCommand(IntermeansThresholding);
                return m_intermeansThreshold;
            }
        }

        public void IntermeansThresholding(object parameter)
        {
            if (GrayInitialImage != null)
            {
                int threshold = ThresholdOperators.IntermeansThreshold(GrayInitialImage);
                GrayProcessedImage = ThresholdOperators.Thresholding(GrayInitialImage, threshold);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Otsu two-threshold
        private ICommand m_otsuThreshold;
        public ICommand OtsuTwoThreshold
        {
            get
            {
                if (m_otsuThreshold == null)
                    m_otsuThreshold = new RelayCommand(OtsuThreshold);
                return m_otsuThreshold;
            }
        }

        public void OtsuThreshold(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = ThresholdOperators.OtsuTwoThreshold(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = ThresholdOperators.OtsuTwoThreshold(GrayProcessedImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
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
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    if (ColorInitialImage != null)
                    {
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Filters.MedianFiltering(GrayProcessedImage, maskSize);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Filters.MedianFiltering(GrayInitialImage, maskSize);
                    }

                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    if (ColorInitialImage != null)
                    {
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Filters.FastMedianFiltering(GrayProcessedImage, maskSize);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Filters.FastMedianFiltering(GrayInitialImage, maskSize);
                    }

                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

        #region Vector Median
        private ICommand m_vectorMedian;
        public ICommand VectorMedian
        {
            get
            {
                if (m_vectorMedian == null)
                    m_vectorMedian = new RelayCommand(VectorMedianFiltering);
                return m_vectorMedian;
            }
        }

        public void VectorMedianFiltering(object parameter)
        {
            if (ColorInitialImage == null)
            {
                MessageBox.Show("Please add a color image.");
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
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    ColorProcessedImage = Filters.VectorMedianFiltering(ColorInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

        #region Gaussian
        private ICommand m_gaussian;
        public ICommand Gaussian
        {
            get
            {
                if (m_gaussian == null)
                    m_gaussian = new RelayCommand(GaussianFiltering);
                return m_gaussian;
            }
        }

        public void GaussianFiltering(object parameter)
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
                    "Variance value:",
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double variance = response[0];

                if (variance > 0)
                {
                    if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Filters.GaussianFiltering(ColorInitialImage, variance);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Filters.GaussianFiltering(GrayInitialImage, variance);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    }
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
                        GrayProcessedImage = Tools.Convert(ColorInitialImage);
                        GrayProcessedImage = Filters.GaussianBilateralFiltering(GrayProcessedImage, variance_d, variance_r);
                    }
                    else if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Filters.GaussianBilateralFiltering(GrayInitialImage, variance_d, variance_r);
                    }

                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("Please add a valid dimension first.");
            }
        }
        #endregion

        #endregion

        #region High-pass filters

        #region Prewitt
        private ICommand m_prewittOp;
        public ICommand PrewittOp
        {
            get
            {
                if (m_prewittOp == null)
                    m_prewittOp = new RelayCommand(Prewitt);
                return m_prewittOp;
            }
        }

        public void Prewitt(object parameter)
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
                    "Threshold value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int threshold = (int)response[0];
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.Prewitt(GrayInitialImage, threshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Sobel
        private ICommand m_sobelOp;
        public ICommand SobelOp
        {
            get
            {
                if (m_sobelOp == null)
                    m_sobelOp = new RelayCommand(Sobel);
                return m_sobelOp;
            }
        }

        public void Sobel(object parameter)
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
                    "Threshold value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int threshold = (int)response[0];
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.Sobel(GrayInitialImage, threshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    GrayProcessedImage = Filters.Sobel(ColorInitialImage, threshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Roberts
        private ICommand m_robertsOp;
        public ICommand RobertsOp
        {
            get
            {
                if (m_robertsOp == null)
                    m_robertsOp = new RelayCommand(Roberts);
                return m_robertsOp;
            }
        }

        public void Roberts(object parameter)
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
                    "Threshold value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int threshold = (int)response[0];
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.Roberts(GrayInitialImage, threshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Canny

        #region Gradient magnitude image
        private ICommand m_cannyGradient;
        public ICommand CannyGradient
        {
            get
            {
                if (m_cannyGradient == null)
                    m_cannyGradient = new RelayCommand(CannyGradientImage);
                return m_cannyGradient;
            }
        }

        public void CannyGradientImage(object parameter)
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
                    "Low threshold: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int lowThreshold = (int)response[0];

                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyGradientImage(GrayInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyGradientImage(ColorInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Gradient direction image
        private ICommand m_cannyAngle;
        public ICommand CannyAngle
        {
            get
            {
                if (m_cannyAngle == null)
                    m_cannyAngle = new RelayCommand(CannyAngleImage);
                return m_cannyAngle;
            }
        }

        public void CannyAngleImage(object parameter)
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
                    "Low threshold: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int lowThreshold = (int)response[0];

                if (GrayInitialImage != null)
                {
                    ColorProcessedImage = Filters.CannyGradientDirectionImage(GrayInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = Filters.CannyGradientDirectionImage(ColorInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }

            }
        }
        #endregion

        #region Nonmaxima suppression
        private ICommand m_cannyNonmaxSup;
        public ICommand CannyNonmaxSup
        {
            get
            {
                if (m_cannyNonmaxSup == null)
                    m_cannyNonmaxSup = new RelayCommand(CannyNonmaximaSuppression);
                return m_cannyNonmaxSup;
            }
        }

        public void CannyNonmaximaSuppression(object parameter)
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
                    "Low threshold: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int lowThreshold = (int)response[0];

                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyNonmaxSuppressionImage(GrayInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyNonmaxSuppressionImage(ColorInitialImage, lowThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Hysteresis thresholding
        private ICommand m_cannyHysteresisThreshold;
        public ICommand CannyHysteresisThreshold
        {
            get
            {
                if (m_cannyHysteresisThreshold == null)
                    m_cannyHysteresisThreshold = new RelayCommand(CannyHysteresisThresholding);
                return m_cannyHysteresisThreshold;
            }
        }

        public void CannyHysteresisThresholding(object parameter)
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
                    "Low threshold: ",
                    "High threshold: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int lowThreshold = (int)response[0];
                int highThreshold = (int)response[1];

                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyHysteresisThresholdingImage(GrayInitialImage, lowThreshold, highThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    GrayProcessedImage = Filters.CannyHysteresisThresholdingImage(ColorInitialImage, lowThreshold, highThreshold);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #region Canny operator
        private ICommand m_cannyOp;
        public ICommand CannyOp
        {
            get
            {
                if (m_cannyOp == null)
                    m_cannyOp = new RelayCommand(Canny);
                return m_cannyOp;
            }
        }

        public void Canny(object parameter)
        {
            if (CannyWindowOn == true) return;

            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            CannySliders cannySliders = new CannySliders(this);
            cannySliders.Show();
            CannyWindowOn = true;
        }
        #endregion

        #endregion

        #endregion

        #region Morphology operators

        #region Dilation
        private ICommand m_dilationOp;
        public ICommand DilationOp
        {
            get
            {
                if (m_dilationOp == null)
                    m_dilationOp = new RelayCommand(Dilation);
                return m_dilationOp;
            }
        }

        public void Dilation(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Dilation mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = Morphology.Dilation(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Erosion
        private ICommand m_erosionOp;
        public ICommand ErosionOp
        {
            get
            {
                if (m_erosionOp == null)
                    m_erosionOp = new RelayCommand(Erosion);
                return m_erosionOp;
            }
        }

        public void Erosion(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Erosion mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = Morphology.Erosion(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Opening
        private ICommand m_openingOp;
        public ICommand OpeningOp
        {
            get
            {
                if (m_openingOp == null)
                    m_openingOp = new RelayCommand(Opening);
                return m_openingOp;
            }
        }

        public void Opening(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Opening mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = Morphology.Opening(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Closing
        private ICommand m_closingOp;
        public ICommand ClosingOp
        {
            get
            {
                if (m_closingOp == null)
                    m_closingOp = new RelayCommand(Closing);
                return m_closingOp;
            }
        }

        public void Closing(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            System.Collections.Generic.List<string> prop = new System.Collections.Generic.List<string>
                {
                    "Closing mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            System.Collections.Generic.List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = Morphology.Closing(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Connected components
        private ICommand m_connectedComp;
        public ICommand ConnectedComp
        {
            get
            {
                if (m_connectedComp == null)
                    m_connectedComp = new RelayCommand(ConnectedComponents);
                return m_connectedComp;
            }
        }

        public void ConnectedComponents(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }

                ColorProcessedImage = Morphology.ConnectedComponents(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #endregion

        #region Geometric transformations

        #region Projective transformation
        private ICommand m_projTransform;
        public ICommand ProjTransform
        {
            get
            {
                if (m_projTransform == null)
                    m_projTransform = new RelayCommand(ProjectiveTransformation);
                return m_projTransform;
            }
        }

        public void ProjectiveTransformation(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (VectorOfMousePosition.Count < 4)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            System.Windows.Point sourceP1 = VectorOfMousePosition[VectorOfMousePosition.Count - 4];
            System.Windows.Point sourceP2 = VectorOfMousePosition[VectorOfMousePosition.Count - 3];
            System.Windows.Point sourceP3 = VectorOfMousePosition[VectorOfMousePosition.Count - 2];
            System.Windows.Point sourceP4 = VectorOfMousePosition[VectorOfMousePosition.Count - 1];

            RemoveAllDrawnElements(parameter);
            ResetProcessedCanvas(parameter);

            VectorOfLines.Add(DrawHelper.DrawLine(InitialCanvas, sourceP1.X, sourceP1.Y, sourceP2.X, sourceP2.Y, 1, Brushes.Red));
            VectorOfLines.Add(DrawHelper.DrawLine(InitialCanvas, sourceP2.X, sourceP2.Y, sourceP3.X, sourceP3.Y, 1, Brushes.Red));
            VectorOfLines.Add(DrawHelper.DrawLine(InitialCanvas, sourceP3.X, sourceP3.Y, sourceP4.X, sourceP4.Y, 1, Brushes.Red));
            VectorOfLines.Add(DrawHelper.DrawLine(InitialCanvas, sourceP4.X, sourceP4.Y, sourceP1.X, sourceP1.Y, 1, Brushes.Red));

            if (SliderZoom.Value == SliderZoom.Minimum)
            {
                SliderZoom.Value += 0.01;
                SliderZoom.Value -= 0.01;
            }
            else
            {
                SliderZoom.Value -= 0.01;
                SliderZoom.Value += 0.01;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.ProjectiveTransformation(GrayInitialImage,
                    sourceP1.X, sourceP1.Y, sourceP2.X, sourceP2.Y, sourceP3.X, sourceP3.Y, sourceP4.X, sourceP4.Y);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.ProjectiveTransformation(ColorInitialImage,
                    sourceP1.X, sourceP1.Y, sourceP2.X, sourceP2.Y, sourceP3.X, sourceP3.Y, sourceP4.X, sourceP4.Y);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #endregion

        #region Segmentation

        #region Detecting lines

        #region Hough transformation (3 quadrants)
        private ICommand m_houghThreeQuadrants;
        public ICommand HoughTransformThreeQuadrants
        {
            get
            {
                if (m_houghThreeQuadrants == null)
                    m_houghThreeQuadrants = new RelayCommand(HoughThreeQuadrants);
                return m_houghThreeQuadrants;
            }
        }

        public void HoughThreeQuadrants(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);


            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("Please add a binary image.");
                    return;
                }

                GrayProcessedImage = Segmentation.HoughTransformThreeQuadrants(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Hough transformation (2 quadrants)
        private ICommand m_houghTwoQuadrants;
        public ICommand HoughTransformTwoQuadrants
        {
            get
            {
                if (m_houghTwoQuadrants == null)
                    m_houghTwoQuadrants = new RelayCommand(HoughTwoQuadrants);
                return m_houghTwoQuadrants;
            }
        }

        public void HoughTwoQuadrants(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ResetProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("Please add a binary image.");
                    return;
                }

                GrayProcessedImage = Segmentation.HoughTransformTwoQuadrants(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }
        #endregion

        #endregion

        #endregion
    }
}