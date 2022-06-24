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
using ImageConverter = ImageProcessingFramework.Model.ImageConverter;
using static ImageProcessingFramework.Model.DataProvider;
using static ImageProcessingAlgorithms.AlgorithmsHelper.Helper;
using ImageProcessingFramework.View;
using ImageProcessingFramework.Model;
using System.Collections.Generic;
using static System.Math;

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

        #region Check if InitialImage is null
        private bool IsInitialImageNull(bool clearProcessedCanvas = true)
        {
            if (clearProcessedCanvas)
                ClearProcessedCanvas(null);

            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return true;
            }
            return false;
        }
        #endregion

        #region Reset zoom
        private ICommand m_resetZoomCommand;
        public ICommand ResetZoomCommand
        {
            get
            {
                if (m_resetZoomCommand == null)
                    m_resetZoomCommand = new RelayCommand(ResetZoom);
                return m_resetZoomCommand;
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
        private ICommand m_saveProcessedImageAsOriginalImageCommand;
        public ICommand SaveProcessedImageAsOriginalImageCommand
        {
            get
            {
                if (m_saveProcessedImageAsOriginalImageCommand == null)
                    m_saveProcessedImageAsOriginalImageCommand = new RelayCommand(SaveProcessedImageAsOriginalImage);
                return m_saveProcessedImageAsOriginalImageCommand;
            }
        }

        public void SaveProcessedImageAsOriginalImage(object parameter)
        {
            RemoveDrawnShapes(parameter);

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
        private ICommand m_loadGrayImageCommand;
        public ICommand LoadGrayImageCommand
        {
            get
            {
                if (m_loadGrayImageCommand == null)
                    m_loadGrayImageCommand = new RelayCommand(LoadGrayImage);
                return m_loadGrayImageCommand;
            }
        }

        public void LoadGrayImage(object parameter)
        {
            ClearAll(parameter);

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
        private ICommand m_loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (m_loadColorImageCommand == null)
                    m_loadColorImageCommand = new RelayCommand(LoadColorImage);
                return m_loadColorImageCommand;
            }
        }

        public void LoadColorImage(object parameter)
        {
            ClearAll(parameter);

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
        private ICommand m_saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (m_saveProcessedImageCommand == null)
                    m_saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return m_saveProcessedImageCommand;
            }
        }

        public void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
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

        #region Save both images
        private ICommand m_saveBothImagesCommand;
        public ICommand SaveBothImagesCommand
        {
            get
            {
                if (m_saveBothImagesCommand == null)
                    m_saveBothImagesCommand = new RelayCommand(SaveBothImages);
                return m_saveBothImagesCommand;
            }
        }

        public void SaveBothImages(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please load and process an image first!");
                return;
            }

            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please process your image!");
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

            if (GrayInitialImage != null && GrayProcessedImage != null)
                Tools.Merging(GrayInitialImage, GrayProcessedImage).
                    Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);

            if (GrayInitialImage != null && ColorProcessedImage != null)
                Tools.Merging(GrayInitialImage, ColorProcessedImage).
                    Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);

            if (ColorInitialImage != null && GrayProcessedImage != null)
                Tools.Merging(ColorInitialImage, GrayProcessedImage).
                    Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);

            if (ColorInitialImage != null && ColorProcessedImage != null)
                Tools.Merging(ColorInitialImage, ColorProcessedImage).
                    Bitmap.Save(saveFileDialog.FileName, jpegCodec, encoderParams);
        }
        #endregion

        #region Exit
        private ICommand m_exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (m_exitCommand == null)
                    m_exitCommand = new RelayCommand(Exit);
                return m_exitCommand;
            }
        }

        public void Exit(object parameter)
        {
            System.Environment.Exit(0);
        }

        private static void CloseAllWindows()
        {
            MagnifierOn = false;
            DisplayRowLevelsOn = false;
            DisplayColumnLevelsOn = false;
            SliderOn = false;

            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();
        }
        #endregion

        #endregion

        #region Edit

        #region Clear initial canvas
        private ICommand m_clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (m_clearInitialCanvasCommand == null)
                    m_clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return m_clearInitialCanvasCommand;
            }
        }

        public void ClearInitialCanvas(object parameter)
        {
            RemoveDrawnShapes(parameter);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand m_clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (m_clearProcessedCanvasCommand == null)
                    m_clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return m_clearProcessedCanvasCommand;
            }
        }

        public void ClearProcessedCanvas(object parameter)
        {
            RemoveDrawnShapes(parameter);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Remove drawn shapes
        private ICommand m_removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (m_removeDrawnShapesCommand == null)
                    m_removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return m_removeDrawnShapesCommand;
            }
        }

        public void RemoveDrawnShapes(object parameter)
        {
            UiHelper.RemoveAllDrawnShapes();
        }
        #endregion

        #region Clear all
        private ICommand m_clearAllCommand;
        public ICommand ClearAllCommand
        {
            get
            {
                if (m_clearAllCommand == null)
                    m_clearAllCommand = new RelayCommand(ClearAll);
                return m_clearAllCommand;
            }
        }

        public void ClearAll(object parameter)
        {
            CloseAllWindows();
            UiHelper.RemoveAllUiElements();

            RemoveDrawnShapes(parameter);
            ClearInitialCanvas(parameter);
            ClearProcessedCanvas(parameter);

            ResetZoom(parameter);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand m_magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (m_magnifierCommand == null)
                    m_magnifierCommand = new RelayCommand(Magnifier);
                return m_magnifierCommand;
            }
        }

        public void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (IsInitialImageNull(false)) return;

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

        #region Display Gray/Color levels

        #region On row
        private ICommand m_displayLevelsOnRowCommand;
        public ICommand DisplayLevelsOnRowCommand
        {
            get
            {
                if (m_displayLevelsOnRowCommand == null)
                    m_displayLevelsOnRowCommand = new RelayCommand(DisplayLevelsOnRow);
                return m_displayLevelsOnRowCommand;
            }
        }

        public void DisplayLevelsOnRow(object parameter)
        {
            if (DisplayRowLevelsOn == true) return;

            if (IsInitialImageNull(false)) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            RowLevelsWindow rowLevelsWindow = new RowLevelsWindow();
            rowLevelsWindow.Show();
            DisplayRowLevelsOn = true;
        }
        #endregion

        #region On column
        private ICommand m_displayLevelsOnColumnCommand;
        public ICommand DisplayLevelsOnColumnCommand
        {
            get
            {
                if (m_displayLevelsOnColumnCommand == null)
                    m_displayLevelsOnColumnCommand = new RelayCommand(DisplayLevelsOnColumn);
                return m_displayLevelsOnColumnCommand;
            }
        }

        public void DisplayLevelsOnColumn(object parameter)
        {
            if (DisplayColumnLevelsOn == true) return;

            if (IsInitialImageNull(false)) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColumnLevelsWindow columnLevelsWindow = new ColumnLevelsWindow();
            columnLevelsWindow.Show();
            DisplayColumnLevelsOn = true;
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand m_copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (m_copyImageCommand == null)
                    m_copyImageCommand = new RelayCommand(CopyImage);
                return m_copyImageCommand;
            }
        }

        public void CopyImage(object parameter)
        {
            if (IsInitialImageNull()) return;

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

        #region Invert image
        private ICommand m_invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (m_invertImageCommand == null)
                    m_invertImageCommand = new RelayCommand(InvertImage);
                return m_invertImageCommand;
            }
        }

        public void InvertImage(object parameter)
        {
            if (IsInitialImageNull()) return;

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

        #region Convert image to grayscale
        private ICommand m_convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (m_convertImageToGrayscaleCommand == null)
                    m_convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return m_convertImageToGrayscaleCommand;
            }
        }

        public void ConvertImageToGrayscale(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else
                MessageBox.Show("It is possible to convert only color images. " +
                    "Please add a color image first.");
        }
        #endregion

        #region Crop image
        private ICommand m_cropImageCommand;
        public ICommand CropImageCommand
        {
            get
            {
                if (m_cropImageCommand == null)
                    m_cropImageCommand = new RelayCommand(CropImage);
                return m_cropImageCommand;
            }
        }

        public void CropImage(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (VectorOfMousePosition.Count < 2)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            System.Windows.Point firstPosition = VectorOfMousePosition[VectorOfMousePosition.Count - 2];

            double leftTopX = Min(firstPosition.X, LastPosition.X);
            double leftTopY = Min(firstPosition.Y, LastPosition.Y);
            double rightBottomX = Max(firstPosition.X, LastPosition.X);
            double rightBottomY = Max(firstPosition.Y, LastPosition.Y);

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
        private ICommand m_mirrorImageVerticallyCommand;
        public ICommand MirrorImageVerticallyCommand
        {
            get
            {
                if (m_mirrorImageVerticallyCommand == null)
                    m_mirrorImageVerticallyCommand = new RelayCommand(MirrorImageVertically);
                return m_mirrorImageVerticallyCommand;
            }
        }

        public void MirrorImageVertically(object parameter)
        {
            if (IsInitialImageNull()) return;

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
        private ICommand m_mirrorImageHorizontallyCommand;
        public ICommand MirrorImageHorizontallyCommand
        {
            get
            {
                if (m_mirrorImageHorizontallyCommand == null)
                    m_mirrorImageHorizontallyCommand = new RelayCommand(MirrorImageHorizontally);
                return m_mirrorImageHorizontallyCommand;
            }
        }

        public void MirrorImageHorizontally(object parameter)
        {
            if (IsInitialImageNull()) return;

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
        private ICommand m_rotateImageClockwiseCommand;
        public ICommand RotateImageClockwiseCommand
        {
            get
            {
                if (m_rotateImageClockwiseCommand == null)
                    m_rotateImageClockwiseCommand = new RelayCommand(RotateImageClockwise);
                return m_rotateImageClockwiseCommand;
            }
        }

        public void RotateImageClockwise(object parameter)
        {
            if (IsInitialImageNull()) return;

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
        private ICommand m_rotateImageAntiClockwiseCommand;
        public ICommand RotateImageAntiClockwiseCommand
        {
            get
            {
                if (m_rotateImageAntiClockwiseCommand == null)
                    m_rotateImageAntiClockwiseCommand = new RelayCommand(RotateImageAntiClockwise);
                return m_rotateImageAntiClockwiseCommand;
            }
        }

        public void RotateImageAntiClockwise(object parameter)
        {
            if (IsInitialImageNull()) return;

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
        private ICommand m_replicatePaddingCommand;
        public ICommand ReplicatePaddingCommand
        {
            get
            {
                if (m_replicatePaddingCommand == null)
                    m_replicatePaddingCommand = new RelayCommand(ReplicatePadding);
                return m_replicatePaddingCommand;
            }
        }

        public void ReplicatePadding(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
            {
                "Thickness value: "
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
        private ICommand m_increaseBrightnessPlusCommand;
        public ICommand IncreaseBrightnessPlusCommand
        {
            get
            {
                if (m_increaseBrightnessPlusCommand == null)
                    m_increaseBrightnessPlusCommand = new RelayCommand(IncreaseBrightnessPlus);
                return m_increaseBrightnessPlusCommand;
            }
        }

        public void IncreaseBrightnessPlus(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Intercept value: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            int intercept = (int)response[0];
            if (intercept >= 0 && intercept <= 255)
            {
                int[] lookupTable = PointwiseOperations.IncreaseBrightnessPlus(intercept);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add a valid intercept value first.");
        }
        #endregion

        #region Operator * (keep black)
        private ICommand m_increaseBrightnessKeepBlackCommand;
        public ICommand IncreaseBrightnessKeepBlackCommand
        {
            get
            {
                if (m_increaseBrightnessKeepBlackCommand == null)
                    m_increaseBrightnessKeepBlackCommand = new RelayCommand(IncreaseBrightnessKeepBlack);
                return m_increaseBrightnessKeepBlackCommand;
            }
        }

        public void IncreaseBrightnessKeepBlack(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Slope value (slope > 1): "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double slope = response[0];
            if (slope > 1)
            {
                int[] lookupTable = PointwiseOperations.IncreaseBrightnessKeepBlack(slope);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add a valid value first.");

        }
        #endregion

        #region Operator * (keep white)
        private ICommand m_increaseBrightnessKeepWhiteCommand;
        public ICommand IncreaseBrightnessKeepWhiteCommand
        {
            get
            {
                if (m_increaseBrightnessKeepWhiteCommand == null)
                    m_increaseBrightnessKeepWhiteCommand = new RelayCommand(IncreaseBrightnessKeepWhite);
                return m_increaseBrightnessKeepWhiteCommand;
            }
        }

        public void IncreaseBrightnessKeepWhite(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Slope value (0 < slope < 1): "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double slope = response[0];
            if (0 < slope && slope < 1)
            {
                int[] lookupTable = PointwiseOperations.IncreaseBrightnessKeepWhite(slope);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add valid values first.");
        }
        #endregion

        #endregion

        #region Decrease brightness

        #region Operator -
        private ICommand m_decreaseBrightnessMinusCommand;
        public ICommand DecreaseBrightnessMinusCommand
        {
            get
            {
                if (m_decreaseBrightnessMinusCommand == null)
                    m_decreaseBrightnessMinusCommand = new RelayCommand(DecreaseBrightnessMinus);
                return m_decreaseBrightnessMinusCommand;
            }
        }

        public void DecreaseBrightnessMinus(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Intercept value: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            int intercept = (int)response[0];
            if (intercept >= 0 && intercept <= 255)
            {
                int[] lookupTable = PointwiseOperations.DecreaseBrightnessMinus(intercept);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add a valid b value first.");
        }
        #endregion

        #region Operator * (keep black)
        private ICommand m_decreaseBrightnessKeepBlackCommand;
        public ICommand DecreaseBrightnessKeepBlackCommand
        {
            get
            {
                if (m_decreaseBrightnessKeepBlackCommand == null)
                    m_decreaseBrightnessKeepBlackCommand = new RelayCommand(DecreaseBrightnessKeepBlack);
                return m_decreaseBrightnessKeepBlackCommand;
            }
        }

        public void DecreaseBrightnessKeepBlack(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Slope value (0 < slope < 1): "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double slope = response[0];
            if (0 < slope && slope < 1)
            {
                int[] lookupTable = PointwiseOperations.DecreaseBrightnessKeepBlack(slope);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add a valid value first.");
        }
        #endregion

        #region Operator * (keep white)
        private ICommand m_decreaseBrightnessKeepWhiteCommand;
        public ICommand DecreaseBrightnessKeepWhiteCommand
        {
            get
            {
                if (m_decreaseBrightnessKeepWhiteCommand == null)
                    m_decreaseBrightnessKeepWhiteCommand = new RelayCommand(DecreaseBrightnessKeepWhite);
                return m_decreaseBrightnessKeepWhiteCommand;
            }
        }

        public void DecreaseBrightnessKeepWhite(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Slope value (slope > 1): "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double slope = response[0];
            if (slope > 1)
            {
                int[] lookupTable = PointwiseOperations.DecreaseBrightnessKeepWhite(slope);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add valid values first.");
        }
        #endregion

        #endregion

        #endregion

        #region Logarithmic operator
        private ICommand m_logarithmicOperatorCommand;
        public ICommand LogarithmicOperatorCommand
        {
            get
            {
                if (m_logarithmicOperatorCommand == null)
                    m_logarithmicOperatorCommand = new RelayCommand(LogarithmicOperator);
                return m_logarithmicOperatorCommand;
            }
        }

        public void LogarithmicOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            int[] lookupTable = PointwiseOperations.LogarithmicOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Exponential operator
        private ICommand m_exponentialOperatorCommand;
        public ICommand ExponentialOperatorCommand
        {
            get
            {
                if (m_exponentialOperatorCommand == null)
                    m_exponentialOperatorCommand = new RelayCommand(ExponentialOperator);
                return m_exponentialOperatorCommand;
            }
        }

        public void ExponentialOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            int[] lookupTable = PointwiseOperations.ExponentialOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Gamma operator
        private ICommand m_gammaOperatorCommand;
        public ICommand GammaOperatorCommand
        {
            get
            {
                if (m_gammaOperatorCommand == null)
                    m_gammaOperatorCommand = new RelayCommand(GammaOperator);
                return m_gammaOperatorCommand;
            }
        }

        public void GammaOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Gamma value: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double gamma = response[0];
            if (gamma > 0)
            {
                int[] lookupTable = PointwiseOperations.GammaCorrection(gamma);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add a valid b value first.");
        }
        #endregion

        #region Piecewise linear operator
        private ICommand m_piecewiseLinearOperatorCommand;
        public ICommand PiecewiseLinearOperatorCommand
        {
            get
            {
                if (m_piecewiseLinearOperatorCommand == null)
                    m_piecewiseLinearOperatorCommand = new RelayCommand(PiecewiseLinearOperator);
                return m_piecewiseLinearOperatorCommand;
            }
        }

        public void PiecewiseLinearOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

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

            int r1 = (int)response[0];
            int s1 = (int)response[1];
            int r2 = (int)response[2];
            int s2 = (int)response[3];

            if (0 <= r1 && r1 <= r2 && r2 <= 255 &&
                0 <= s1 && s1 <= s2 && s2 <= 255)
            {
                int[] lookupTable = PointwiseOperations.PiecewiseLinearContrast(r1, s1, r2, s2);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add valid values first " +
                "(0 <= r1 <= r2 <= 255 and " +
                "0 <= s1 <= s2 <= 255).");
        }
        #endregion

        #region Non-linear operators

        #region Sinusoidal operator
        private ICommand m_sinusoidalOperatorCommand;
        public ICommand SinusoidalOperatorCommand
        {
            get
            {
                if (m_sinusoidalOperatorCommand == null)
                    m_sinusoidalOperatorCommand = new RelayCommand(SinusoidalOperator);
                return m_sinusoidalOperatorCommand;
            }
        }

        public void SinusoidalOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            int[] lookupTable = PointwiseOperations.SinusoidalOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Polynomial operator
        private ICommand m_polynomialOperatorCommand;
        public ICommand PolynomialOperatorCommand
        {
            get
            {
                if (m_polynomialOperatorCommand == null)
                    m_polynomialOperatorCommand = new RelayCommand(PolynomialOperator);
                return m_polynomialOperatorCommand;
            }
        }

        public void PolynomialOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            int[] lookupTable = PointwiseOperations.PolynomialOperator();
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region EM - operator
        private ICommand m_emOperatorCommand;
        public ICommand EmOperatorCommand
        {
            get
            {
                if (m_emOperatorCommand == null)
                    m_emOperatorCommand = new RelayCommand(EmOperator);
                return m_emOperatorCommand;
            }
        }

        public void EmOperator(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "m value: ",
                    "E value: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double m = response[0];
            double E = response[1];
            if (m != 0 && E != 0)
            {
                int[] lookupTable = PointwiseOperations.EmOperator(m, E);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    ColorProcessedImage = AdjustBrightnessAndContrast(ColorInitialImage, lookupTable);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                }
            }
            else MessageBox.Show("Please add valid values first.");
        }
        #endregion

        #endregion

        #region Cubic Hermite spline

        private ICommand m_cubicHermiteSplineCommand;
        public ICommand CubicHermiteSplineCommand
        {
            get
            {
                if (m_cubicHermiteSplineCommand == null)
                    m_cubicHermiteSplineCommand = new RelayCommand(CubicHermiteSpline);
                return m_cubicHermiteSplineCommand;
            }
        }

        public void CubicHermiteSpline(object parameter)
        {
            if (HermiteSplineOn == true) return;

            if (IsInitialImageNull()) return;

            SplineWindow splineWindow = new SplineWindow(this);
            splineWindow.Show();
            HermiteSplineOn = true;
        }
        #endregion

        #region Histogram equalization
        private ICommand m_histogramEqualizationCommand;
        public ICommand HistogramEqualizationCommand
        {
            get
            {
                if (m_histogramEqualizationCommand == null)
                    m_histogramEqualizationCommand = new RelayCommand(HistogramEqualization);
                return m_histogramEqualizationCommand;
            }
        }

        public void HistogramEqualization(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                int[] lookupTable = PointwiseOperations.HistogramEqualization(GrayInitialImage);
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayInitialImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);

                int[] lookupTable = PointwiseOperations.HistogramEqualization(GrayProcessedImage);
                GrayProcessedImage = AdjustBrightnessAndContrast(GrayProcessedImage, lookupTable);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Color histogram equalization
        private ICommand m_colorHistogramEqualizationCommand;
        public ICommand ColorHistogramEqualizationCommand
        {
            get
            {
                if (m_colorHistogramEqualizationCommand == null)
                    m_colorHistogramEqualizationCommand = new RelayCommand(ColorHistogramEqualization);
                return m_colorHistogramEqualizationCommand;
            }
        }

        public void ColorHistogramEqualization(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ColorHistogramEqualization(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
            }
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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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

        #region 3D Color Thresholding
        private ICommand m_thresholding3D;
        public ICommand Thresholding3D
        {
            get
            {
                if (m_thresholding3D == null)
                    m_thresholding3D = new RelayCommand(ColorThresholding3D);
                return m_thresholding3D;
            }
        }

        public void ColorThresholding3D(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select a color first!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
            {
                "Threshold: "
            };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            int threshold = (int)response[0];

            GrayProcessedImage = Thresholding.Thresholding3D(
                ColorInitialImage, LastPosition.X, LastPosition.Y, threshold);
            ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
        }
        #endregion

        #region 2D Color Thresholding
        private ICommand m_thresholding2D;
        public ICommand Thresholding2D
        {
            get
            {
                if (m_thresholding2D == null)
                    m_thresholding2D = new RelayCommand(ColorThresholding2D);
                return m_thresholding2D;
            }
        }

        public void ColorThresholding2D(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select a color first!");
                return;
            }

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
            {
                "Threshold: "
            };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();
            double threshold = response[0];

            GrayProcessedImage = Thresholding.Thresholding2D(
                ColorInitialImage, LastPosition.X, LastPosition.Y, threshold);
            ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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

        #region Kuwahara
        private ICommand m_kuwahara;
        public ICommand Kuwahara
        {
            get
            {
                if (m_kuwahara == null)
                    m_kuwahara = new RelayCommand(KuwaharaFiltering);
                return m_kuwahara;
            }
        }

        public void KuwaharaFiltering(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.Kuwahara(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Filters.Kuwahara(GrayProcessedImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

            CannySliders cannySliders = new CannySliders(this);
            cannySliders.Show();
            CannyWindowOn = true;
        }
        #endregion

        #endregion

        #region Emboss
        private ICommand m_emboss;
        public ICommand Emboss
        {
            get
            {
                if (m_emboss == null)
                    m_emboss = new RelayCommand(EmbossFilter);
                return m_emboss;
            }
        }

        public void EmbossFilter(object parameter)
        {
            if (SliderOn == true) return;
            if (IsInitialImageNull()) return;

            SliderWindow sliderWindow = new SliderWindow(this, "Threshold value: ");

            if (GrayInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(GrayInitialImage, Filters.Emboss);
            }
            else if (ColorInitialImage != null)
            {
                sliderWindow.SetAlgorithmToApply(ColorInitialImage, Filters.Emboss);
            }

            sliderWindow.ConfigureSlider(3, 7, 3, 2);
            sliderWindow.Show();
            SliderOn = true;
        }
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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

        #region Edge detecting
        private ICommand m_edgeDetectingXORCommand;
        public ICommand EdgeDetectingXORCommand
        {
            get
            {
                if (m_edgeDetectingXORCommand == null)
                    m_edgeDetectingXORCommand = new RelayCommand(EdgeDetecting_XOR);
                return m_edgeDetectingXORCommand;
            }
        }

        public void EdgeDetecting_XOR(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }

                GrayProcessedImage = MorphologicalOperations.XOR(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Skeletonization

        #region 8 Masks algorithm
        private ICommand m_masks8Command;
        public ICommand Masks8Command
        {
            get
            {
                if (m_masks8Command == null)
                    m_masks8Command = new RelayCommand(Masks8);
                return m_masks8Command;
            }
        }

        public void Masks8(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }

                GrayProcessedImage = MorphologicalOperations.Masks8(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

        #region Zhang-Suen algorithm
        private ICommand m_zhangSuenCommand;
        public ICommand ZhangSuenCommand
        {
            get
            {
                if (m_zhangSuenCommand == null)
                    m_zhangSuenCommand = new RelayCommand(ZhangSuen);
                return m_zhangSuenCommand;
            }
        }

        public void ZhangSuen(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("The image is not binary!");
                    return;
                }

                GrayProcessedImage = MorphologicalOperations.ZhangSuen(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else MessageBox.Show("No grayscale image!");
        }
        #endregion

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
            if (IsInitialImageNull()) return;

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
                GrayProcessedImage = GeometricTransformations.TwirlTransformation(
                    GrayInitialImage, rotationAngle, maximumRadius);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.TwirlTransformation(
                    ColorInitialImage, rotationAngle, maximumRadius);
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
            if (IsInitialImageNull()) return;

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

        #region Spherical deformation
        private ICommand m_sphericalDeformation;
        public ICommand SphericalDeform
        {
            get
            {
                if (m_sphericalDeformation == null)
                    m_sphericalDeformation = new RelayCommand(SphericalDeformation);
                return m_sphericalDeformation;
            }
        }

        public void SphericalDeformation(object parameter)
        {
            if (IsInitialImageNull()) return;

            DialogBox dialogBox = new DialogBox();
            List<string> prop = new List<string>
                {
                    "Refractive index: ",
                    "Lens radius: "
                };

            dialogBox.CreateDialogBox(prop);
            dialogBox.ShowDialog();

            List<double> response = dialogBox.GetResponseTexts();

            double refractiveIndex = response[0];
            double lensRadius = response[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.SphericalDeformation(
                    GrayInitialImage, refractiveIndex, lensRadius);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.SphericalDeformation(
                    ColorInitialImage, refractiveIndex, lensRadius);
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
            RemoveDrawnShapes(parameter);

            if (IsInitialImageNull()) return;

            if (VectorOfMousePosition.Count < 4)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            System.Windows.Point sourceP1 = VectorOfMousePosition[VectorOfMousePosition.Count - 4];
            System.Windows.Point sourceP2 = VectorOfMousePosition[VectorOfMousePosition.Count - 3];
            System.Windows.Point sourceP3 = VectorOfMousePosition[VectorOfMousePosition.Count - 2];
            System.Windows.Point sourceP4 = VectorOfMousePosition[VectorOfMousePosition.Count - 1];

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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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
            if (IsInitialImageNull()) return;

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
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

        #region Detecting circles
        private ICommand m_houghGivenRadius;
        public ICommand HoughTransformGivenRadius
        {
            get
            {
                if (m_houghGivenRadius == null)
                    m_houghGivenRadius = new RelayCommand(SlowHoughForCircles_GivenRadius);
                return m_houghGivenRadius;
            }
        }

        public void SlowHoughForCircles_GivenRadius(object parameter)
        {
            if (IsInitialImageNull()) return;

            if (VectorOfMousePosition.Count < 2)
            {
                MessageBox.Show("Please select the approximation of the radius");
                return;
            }

            System.Windows.Point firstPosition = VectorOfMousePosition[VectorOfMousePosition.Count - 2];

            double radius = Sqrt(
                Pow(LastPosition.X - firstPosition.X, 2) +
                Pow(LastPosition.Y - firstPosition.Y, 2));

            if (GrayInitialImage != null)
            {
                if (!IsBinaryImage(GrayInitialImage))
                {
                    MessageBox.Show("Please add a binary image.");
                    return;
                }

                GrayProcessedImage = Segmentation.SlowHoughTransform_GivenRadius(GrayInitialImage, radius);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
            }
        }
        #endregion

        #endregion
    }
}