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
using System.Collections.Generic;

namespace ImageProcessingFramework.ViewModel
{
    class MainCommands : BaseVM
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
                NotifyPropertyChanged("InitialImage");
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
                NotifyPropertyChanged("ProcessedImage");
            }
        }

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
        }

        public void UpdateZoom(object parameter)
        {
            if (parameter is Slider slider)
            {
                if (slider.Value == slider.Minimum)
                {
                    slider.Value += 0.01;
                    slider.Value -= 0.01;
                }
                else
                {
                    slider.Value -= 0.01;
                    slider.Value += 0.01;
                }
            }
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
            RemoveAllDrawnShapes(parameter);

            if (ColorProcessedImage == null && GrayProcessedImage == null)
            {
                MessageBox.Show("Doesn't exist a processed image.");
                return;
            }

            ClearInitialCanvas(parameter);

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

        #region File

        #region Load grayscale image
        private ICommand m_loadGrayImage;
        public ICommand LoadGrayImage
        {
            get
            {
                if (m_loadGrayImage == null)
                    m_loadGrayImage = new RelayCommand(LoadingGrayImage);
                return m_loadGrayImage;
            }
        }

        public void LoadingGrayImage(object parameter)
        {
            ClearUi(parameter);

            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a gray picture",
                Filter = "Image files (*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName.CompareTo("") == 0)
                return;

            GrayInitialImage = new Image<Gray, byte>(openFileDialog.FileName);
            InitialImage = ImageConverter.Convert(GrayInitialImage);
        }
        #endregion

        #region Load color image
        private ICommand m_loadColorImage;
        public ICommand LoadColorImage
        {
            get
            {
                if (m_loadColorImage == null)
                    m_loadColorImage = new RelayCommand(LoadingColorImage);
                return m_loadColorImage;
            }
        }

        public void LoadingColorImage(object parameter)
        {
            ClearUi(parameter);

            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a color picture",
                Filter = "Image files (*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName.CompareTo("") == 0)
                return;

            ColorInitialImage = new Image<Bgr, byte>(openFileDialog.FileName);
            InitialImage = ImageConverter.Convert(ColorInitialImage);
        }
        #endregion

        #region Save processed image
        private ICommand m_saveImage;
        public ICommand SaveImage
        {
            get
            {
                if (m_saveImage == null)
                    m_saveImage = new RelayCommand(SavingImage);
                return m_saveImage;
            }
        }

        public void SavingImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, please load and process an image first!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "image.jpg",
                Filter = "Image files(*.jpg, *.jpeg, *.jfif, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jfif; *.jpe; *.bmp; *.png"
            };

            saveFileDialog.ShowDialog();

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                Encoder.Quality,
                (long)100
            );

            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == "image/jpeg"
                             select codec).Single();

            if (GrayProcessedImage != null)
                GrayProcessedImage.Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);

            if (ColorProcessedImage != null)
                ColorProcessedImage.Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);
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

        private static void CloseAllWindows()
        {
            MagnifierOn = false;
            RowLevelsOn = false;
            ColumnLevelsOn = false;
            SliderOn = false;

            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();
        }
        #endregion

        #endregion

        #region Edit

        #region Reset initial canvas
        private ICommand m_resetInitialCanvas;
        public ICommand ResetInitialCanvas
        {
            get
            {
                if (m_resetInitialCanvas == null)
                    m_resetInitialCanvas = new RelayCommand(ClearInitialCanvas);
                return m_resetInitialCanvas;
            }
        }

        public void ClearInitialCanvas(object parameter)
        {
            RemoveAllDrawnShapes(parameter);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Reset processed canvas
        private ICommand m_resetProcessedCanvas;
        public ICommand ResetProcessedCanvas
        {
            get
            {
                if (m_resetProcessedCanvas == null)
                    m_resetProcessedCanvas = new RelayCommand(ClearProcessedCanvas);
                return m_resetProcessedCanvas;
            }
        }

        public void ClearProcessedCanvas(object parameter)
        {
            RemoveAllDrawnShapes(parameter);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Remove drawn elements
        private ICommand m_removeDrawnShapes;
        public ICommand RemoveDrawnShapes
        {
            get
            {
                if (m_removeDrawnShapes == null)
                    m_removeDrawnShapes = new RelayCommand(RemoveAllDrawnShapes);
                return m_removeDrawnShapes;
            }
        }

        public void RemoveAllDrawnShapes(object parameter)
        {
            UiHelper.RemoveAllDrawnShapes();
        }
        #endregion

        #region Clear all
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

        public void ClearUi(object parameter)
        {
            CloseAllWindows();
            UiHelper.RemoveAllUiElements();

            RemoveAllDrawnShapes(parameter);
            ClearInitialCanvas(parameter);
            ClearProcessedCanvas(parameter);

            ResetZoom(parameter);
        }
        #endregion

        #endregion

        #region Tools

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

        #region Gray/Color levels

        #region On row
        private ICommand m_rowDisplay;
        public ICommand RowDisplay
        {
            get
            {
                if (m_rowDisplay == null)
                    m_rowDisplay = new RelayCommand(RowLevels);
                return m_rowDisplay;
            }
        }

        public void RowLevels(object parameter)
        {
            if (RowLevelsOn == true) return;

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

            RowLevelsWindow rowLevelsWindow = new RowLevelsWindow();
            rowLevelsWindow.Show();
            RowLevelsOn = true;
        }
        #endregion

        #region On column
        private ICommand m_columnDisplay;
        public ICommand ColumnDisplay
        {
            get
            {
                if (m_columnDisplay == null)
                    m_columnDisplay = new RelayCommand(ColumnLevels);
                return m_columnDisplay;
            }
        }

        public void ColumnLevels(object parameter)
        {
            if (ColumnLevelsOn == true) return;

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

            ColumnLevelsWindow columnLevelsWindow = new ColumnLevelsWindow();
            columnLevelsWindow.Show();
            ColumnLevelsOn = true;
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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

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
            ClearProcessedCanvas(parameter);

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
                MessageBox.Show("Please select an area first!");
                return;
            }

            System.Windows.Point firstPosition = VectorOfMousePosition[VectorOfMousePosition.Count - 2];

            double leftTopX = System.Math.Min(firstPosition.X, LastPosition.X);
            double leftTopY = System.Math.Min(firstPosition.Y, LastPosition.Y);
            double rightBottomX = System.Math.Max(firstPosition.X, LastPosition.X);
            double rightBottomY = System.Math.Max(firstPosition.Y, LastPosition.Y);

            ClearProcessedCanvas(parameter);

            VectorOfRectangles.Add(DrawingHelper.DrawRectangle(InitialCanvas, leftTopX, leftTopY, rightBottomX, rightBottomY, 1, Brushes.Red));
            UpdateZoom(parameter);

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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
            {
                "Thickness value"
            };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
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

        #endregion

        #region Pointwise operations

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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "b value"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int b = (int)response[0];
                if (b >= 0 && b <= 255)
                {
                    int[] lookUpTable = PointwiseOperations.IncreaseBrightnessPlus(b);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "a value (a > 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (a > 1)
                {
                    int[] lookUpTable = PointwiseOperations.IncreaseBrightnessKeepBlack(a);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "a value (0 < a < 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (0 < a && a < 1)
                {
                    int[] lookUpTable = PointwiseOperations.IncreaseBrightnessKeepWhite(a);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "b value"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int b = (int)response[0];
                if (b >= 0 && b <= 255)
                {
                    int[] lookUpTable = PointwiseOperations.DecreaseBrightnessMinus(b);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "a value (0 < a < 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (0 < a && a < 1)
                {
                    int[] lookUpTable = PointwiseOperations.DecreaseBrightnessKeepBlack(a);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "a value (a > 1):"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double a = response[0];
                if (a > 1)
                {
                    int[] lookUpTable = PointwiseOperations.DecreaseBrightnessKeepWhite(a);
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

            ClearProcessedCanvas(parameter);

            int[] lookUpTable = PointwiseOperations.LogarithmicOperator();
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

            ClearProcessedCanvas(parameter);

            int[] lookUpTable = PointwiseOperations.ExponentialOperator();
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Gamma value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double gamma = response[0];
                if (gamma > 0)
                {
                    int[] lookUpTable = PointwiseOperations.GammaCorrection(gamma);
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "r1:",
                    "s1:",
                    "r2:",
                    "s2:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int r1 = (int)response[0];
                int s1 = (int)response[1];
                int r2 = (int)response[2];
                int s2 = (int)response[3];

                if (0 <= r1 && r1 <= r2 && r2 <= 255 &&
                    0 <= s1 && s1 <= s2 && s2 <= 255)
                {
                    int[] lookUpTable = PointwiseOperations.PiecewiseLinearContrast(r1, s1, r2, s2);
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

            ClearProcessedCanvas(parameter);

            int[] lookUpTable = PointwiseOperations.SinusoidalOperator();
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

            ClearProcessedCanvas(parameter);

            int[] lookUpTable = PointwiseOperations.PolynomialOperator();
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "m value:",
                    "E value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                double m = response[0];
                double E = response[1];
                if (m != 0 && E != 0)
                {
                    int[] lookUpTable = PointwiseOperations.EmOperator(m, E);
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

            ClearProcessedCanvas(parameter);

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

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                int[] lookUpTable = PointwiseOperations.HistogramEqualization(GrayInitialImage);
                GrayProcessedImage = Helper.AdjustBrightnessAndContrast(GrayInitialImage, lookUpTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else
            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);

                int[] lookUpTable = PointwiseOperations.HistogramEqualization(GrayProcessedImage);
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

            ClearProcessedCanvas(parameter);

            ColorProcessedImage = PointwiseOperations.ColorHistogramEqualization(ColorInitialImage);
            ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
        }
        #endregion

        #endregion

        #region Thresholding

        #region Threshold is given as input
        private ICommand m_inputThresholding;
        public ICommand InputThresholding
        {
            get
            {
                if (m_inputThresholding == null)
                    m_inputThresholding = new RelayCommand(ThresholdingImage);
                return m_inputThresholding;
            }
        }

        public void ThresholdingImage(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Threshold value: ");
            sliderWindow.ConfigureSlider();

            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Thresholding.InputThresholding);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                sliderWindow.SetAlgorithmToApply(GrayProcessedImage, Thresholding.InputThresholding);
            }

            sliderWindow.Show();
            SliderOn = true;
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
                List<string> prop = new List<string>
                {
                    "Background pixels percent (0 <= p <= 1):"
                };

                dialogBox.CreateDialogBox(prop);
                dialogBox.ShowDialog();

                List<double> response = dialogBox.GetResponseTexts();
                if (response != null)
                {
                    double percent = response[0];
                    if (0 <= percent && percent <= 1)
                    {
                        int threshold = Thresholding.QuantileThreshold(GrayInitialImage, percent);
                        GrayProcessedImage = Thresholding.InputThresholding(GrayInitialImage, threshold);
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
                int threshold = Thresholding.MedianThreshold(GrayInitialImage);
                GrayProcessedImage = Thresholding.InputThresholding(GrayInitialImage, threshold);
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
                int threshold = Thresholding.IntermeansThreshold(GrayInitialImage);
                GrayProcessedImage = Thresholding.InputThresholding(GrayInitialImage, threshold);
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

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Thresholding.OtsuTwoThreshold(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Thresholding.OtsuTwoThreshold(GrayProcessedImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }

        #endregion

        #region Adaptive threshold
        private ICommand m_adaptiveThreshold;
        public ICommand AdaptiveThreshold
        {
            get
            {
                if (m_adaptiveThreshold == null)
                    m_adaptiveThreshold = new RelayCommand(AdaptiveThresholding);
                return m_adaptiveThreshold;
            }
        }

        public void AdaptiveThresholding(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("Please add a gray image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Mask value:",
                    "b value:"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                double b = response[1];
                if (maskSize > 1 && maskSize % 2 == 1 && 0.8 <= b && b <= 0.9)
                {
                    GrayProcessedImage = Thresholding.AdaptiveThresholding(GrayInitialImage, maskSize, b);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("Please add valid values first.");
            }
        }
        #endregion

        #endregion

        #region Low-pass filters

        #region Mean
        private ICommand m_mean;
        public ICommand Mean
        {
            get
            {
                if (m_mean == null)
                    m_mean = new RelayCommand(MeanFiltering);
                return m_mean;
            }
        }

        public void MeanFiltering(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Mask size: ");
            sliderWindow.ConfigureSlider(1, 101, 1, 2);

            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.MeanFiltering);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.MeanFiltering);
            }

            sliderWindow.Show();
            SliderOn = true;
        }
        #endregion

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
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a gray image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Mask size: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.MedianFiltering);
            }

            sliderWindow.ConfigureSlider(1, 101, 1, 2);
            sliderWindow.Show();
            SliderOn = true;
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
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a gray image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Mask size: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.FastMedianFiltering);
            }

            sliderWindow.ConfigureSlider(1, 101, 1, 2);
            sliderWindow.Show();
            SliderOn = true;
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
            if (SliderOn == true) return;
            if (GrayInitialImage != null)
            {
                MessageBox.Show("Please add a color image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Mask size: ");
            if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.VectorMedianFiltering);
            }

            sliderWindow.ConfigureSlider(1, 7, 1, 2);
            sliderWindow.Show();
            SliderOn = true;
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

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Variance value: ");
            sliderWindow.ConfigureSlider(0, 10, 0, 0.5);

            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.GaussianFiltering);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.GaussianFiltering);
            }

            sliderWindow.Show();
            SliderOn = true;
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Variance d",
                    "Variance r"
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
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
        private ICommand m_prewitt;
        public ICommand Prewitt
        {
            get
            {
                if (m_prewitt == null)
                    m_prewitt = new RelayCommand(PrewittOperator);
                return m_prewitt;
            }
        }

        public void PrewittOperator(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.Prewitt);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                sliderWindow.SetAlgorithmToApply(GrayProcessedImage, Filters.Prewitt);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
        }
        #endregion

        #region Sobel
        private ICommand m_sobel;
        public ICommand Sobel
        {
            get
            {
                if (m_sobel == null)
                    m_sobel = new RelayCommand(SobelOperator);
                return m_sobel;
            }
        }

        public void SobelOperator(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.Sobel);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.Sobel);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
        }
        #endregion

        #region Roberts
        private ICommand m_roberts;
        public ICommand Roberts
        {
            get
            {
                if (m_roberts == null)
                    m_roberts = new RelayCommand(RobertsOperator);
                return m_roberts;
            }
        }

        public void RobertsOperator(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.Roberts);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                sliderWindow.SetAlgorithmToApply(GrayProcessedImage, Filters.Roberts);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
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
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Low threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.CannyGradientForGray);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.CannyGradientForColor);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
        }
        #endregion

        #region Gradient direction image
        private ICommand m_cannyDirection;
        public ICommand CannyDirection
        {
            get
            {
                if (m_cannyDirection == null)
                    m_cannyDirection = new RelayCommand(CannyDirectionImage);
                return m_cannyDirection;
            }
        }

        public void CannyDirectionImage(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Low threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.CannyGradientDirectionForGray);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.CannyGradientDirectionForColor);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
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
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Low threshold value: ");
            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.CannyNonmaxSuppressionForGray);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.CannyNonmaxSuppressionForColor);
            }

            sliderWindow.ConfigureSlider();
            sliderWindow.Show();
            SliderOn = true;
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

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Low threshold: ",
                    "High threshold: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
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
        private ICommand m_canny;
        public ICommand Canny
        {
            get
            {
                if (m_canny == null)
                    m_canny = new RelayCommand(CannyOperator);
                return m_canny;
            }
        }

        public void CannyOperator(object parameter)
        {
            if (CannyWindowOn == true) return;

            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image.");
                return;
            }

            ClearProcessedCanvas(parameter);

            CannySliders cannySliders = new CannySliders(this);
            cannySliders.Show();
            CannyWindowOn = true;
        }
        #endregion

        #endregion

        #endregion

        #region Morphological operations

        #region On binary images

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

            ClearProcessedCanvas(parameter);

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
            List<string> prop = new List<string>
                {
                    "Dilation mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.DilationOnBinary(GrayInitialImage, maskSize);
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

            ClearProcessedCanvas(parameter);

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
            List<string> prop = new List<string>
                {
                    "Erosion mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.ErosionOnBinary(GrayInitialImage, maskSize);
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

            ClearProcessedCanvas(parameter);

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
            List<string> prop = new List<string>
                {
                    "Opening mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.OpeningOnBinary(GrayInitialImage, maskSize);
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

            ClearProcessedCanvas(parameter);

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
            List<string> prop = new List<string>
                {
                    "Closing mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.ClosingOnBinary(GrayInitialImage, maskSize);
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

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                if (!Helper.IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }

                ColorProcessedImage = MorphologicalOperations.ConnectedComponents(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #endregion

        #region On grayscale images

        #region Dilation
        private ICommand m_dilationGrayOp;
        public ICommand DilationGrayOp
        {
            get
            {
                if (m_dilationGrayOp == null)
                    m_dilationGrayOp = new RelayCommand(DilationOnGrayscale);
                return m_dilationGrayOp;
            }
        }

        public void DilationOnGrayscale(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Dilation mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.DilationOnGrayscale(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Erosion
        private ICommand m_erosionGrayOp;
        public ICommand ErosionGrayOp
        {
            get
            {
                if (m_erosionGrayOp == null)
                    m_erosionGrayOp = new RelayCommand(ErosionOnGrayscale);
                return m_erosionGrayOp;
            }
        }

        public void ErosionOnGrayscale(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Erosion mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.ErosionOnGrayscale(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Opening
        private ICommand m_openingGrayOp;
        public ICommand OpeningGrayOp
        {
            get
            {
                if (m_openingGrayOp == null)
                    m_openingGrayOp = new RelayCommand(OpeningOnGrayscale);
                return m_openingGrayOp;
            }
        }

        public void OpeningOnGrayscale(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Opening mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.OpeningOnGrayscale(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Closing
        private ICommand m_closingGrayOp;
        public ICommand ClosingGrayOp
        {
            get
            {
                if (m_closingGrayOp == null)
                    m_closingGrayOp = new RelayCommand(ClosingOnGrayscale);
                return m_closingGrayOp;
            }
        }

        public void ClosingOnGrayscale(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Closing mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.ClosingOnGrayscale(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Smoothing
        private ICommand m_morfologicSmooth;
        public ICommand MorfologicSmooth
        {
            get
            {
                if (m_morfologicSmooth == null)
                    m_morfologicSmooth = new RelayCommand(MorfologicSmoothing);
                return m_morfologicSmooth;
            }
        }

        public void MorfologicSmoothing(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.MorfologicSmoothing(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #region Gradient
        private ICommand m_morfologicGradient;
        public ICommand MorfologicGrad
        {
            get
            {
                if (m_morfologicGradient == null)
                    m_morfologicGradient = new RelayCommand(MorfologicGradient);
                return m_morfologicGradient;
            }
        }

        public void MorfologicGradient(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("No grayscale image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Mask size: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            if (response != null)
            {
                int maskSize = (int)response[0];
                if (maskSize > 0 && maskSize % 2 == 1)
                {
                    GrayProcessedImage = MorphologicalOperations.MorfologicGradient(GrayInitialImage, maskSize);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else MessageBox.Show("The mask size is not valid!");
            }
        }
        #endregion

        #endregion

        #endregion

        #region Geometric transformations

        #region Scale transformation
        private ICommand m_scaleTransform;
        public ICommand ScaleTransform
        {
            get
            {
                if (m_scaleTransform == null)
                    m_scaleTransform = new RelayCommand(Scale);
                return m_scaleTransform;
            }
        }

        public void Scale(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
            {
                "Sy:",
                "Sx:"
            };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double Sy = response[0];
            double Sx = response[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.Scale(GrayInitialImage, Sy, Sx);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.Scale(ColorInitialImage, Sy, Sx);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate transformation
        private ICommand m_rotateTransform;
        public ICommand RotateTransform
        {
            get
            {
                if (m_rotateTransform == null)
                    m_rotateTransform = new RelayCommand(Rotate);
                return m_rotateTransform;
            }
        }

        public void Rotate(object parameter)
        {
            if (SliderOn == true) return;
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            SliderWindow sliderWindow = new SliderWindow(this, "Rotation angle: ");
            sliderWindow.ConfigureSlider(0, 360, 0, 5);

            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, GeometricTransformations.Rotate);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, GeometricTransformations.Rotate);
            }

            sliderWindow.Show();
            SliderOn = true;
        }
        #endregion

        #region Twirl transformation
        private ICommand m_twirlTransform;
        public ICommand TwirlTransform
        {
            get
            {
                if (m_twirlTransform == null)
                    m_twirlTransform = new RelayCommand(TwirlTransformation);
                return m_twirlTransform;
            }
        }

        public void TwirlTransformation(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Rotation angle: ",
                    "Maximum radius: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double rotationAngle = response[0];
            double maximumRadius = response[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.TwirlTransformation(GrayInitialImage, rotationAngle, maximumRadius);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.TwirlTransformation(ColorInitialImage, rotationAngle, maximumRadius);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Ripple transformation
        private ICommand m_rippleTransform;
        public ICommand RippleTransform
        {
            get
            {
                if (m_rippleTransform == null)
                    m_rippleTransform = new RelayCommand(RippleTransformation);
                return m_rippleTransform;
            }
        }

        public void RippleTransformation(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "tx: ",
                    "ty: ",
                    "ax: ",
                    "ay: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double tx = response[0];
            double ty = response[1];
            double ax = response[2];
            double ay = response[3];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.RippleTransformation(
                    GrayInitialImage, tx, ty, ax, ay);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.RippleTransformation(
                    ColorInitialImage, tx, ty, ax, ay);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

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

            RemoveAllDrawnShapes(parameter);
            ClearProcessedCanvas(parameter);

            System.Windows.Media.PointCollection pointCollection = new System.Windows.Media.PointCollection()
            {
                sourceP1, sourceP2, sourceP3, sourceP4
            };

            VectorOfPolygons.Add(DrawingHelper.DrawPolygon(InitialCanvas, pointCollection, 1, Brushes.Red));
            UpdateZoom(parameter);

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

            ClearProcessedCanvas(parameter);


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

            ClearProcessedCanvas(parameter);

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