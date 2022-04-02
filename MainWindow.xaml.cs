using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PointCollection = System.Windows.Media.PointCollection;
using static ImageProcessingFramework.Model.DataProvider;
using static ImageProcessingFramework.Model.UiHelper;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

namespace ImageProcessingFramework
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MousePosition = new Point(0, 0);
            VectorOfMousePosition = new PointCollection();

            LastPosition = MousePosition;
            InitialCanvas = canvasOriginalImage;
            ProcessedCanvas = canvasProcessedImage;

            VectorOfLines = new Collection<Line>();
            VectorOfRectangles = new Collection<Rectangle>();
            VectorOfEllipses = new Collection<Ellipse>();
            VectorOfPolygons = new Collection<Polygon>();

            SliderZoom = sliderZoom;
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            string nameImage = (sender as Image).Name;
            if (string.Compare(nameImage, initialImage.Name) == 0)
            {
                var position = e.GetPosition(initialImage);
                SetUiValues(xPos, yPos, grayValue, bValue, gValue, rValue, GrayInitialImage, ColorInitialImage,
                    (int)position.X, (int)position.Y);
            }
            else
            {
                var position = e.GetPosition(processedImage);
                SetUiValues(xPos, yPos, grayValue, bValue, gValue, rValue, GrayProcessedImage, ColorProcessedImage,
                    (int)position.X, (int)position.Y);
            }

            if (MagnifierOn == false)
                RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRectangle, ProcessedRectangle);

            if (GLevelsRowOn == false)
                RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);

            if (GLevelsColumnOn == false)
                RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialColumnLine, ProcessedColumnLine);
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == ScrollViewerInitial)
            {
                ScrollViewerProcessed.ScrollToVerticalOffset(e.VerticalOffset);
                ScrollViewerProcessed.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else
            {
                ScrollViewerInitial.ScrollToVerticalOffset(e.VerticalOffset);
                ScrollViewerInitial.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        private void MouseLeftPressed(object sender, MouseButtonEventArgs e)
        {
            string nameImage = (sender as Image).Name;
            if (string.Compare(nameImage, initialImage.Name) == 0)
                MousePosition = e.GetPosition(initialImage);
            else
                MousePosition = e.GetPosition(processedImage);

            if (LastPosition != MousePosition)
            {
                VectorOfMousePosition.Add(MousePosition);
                LastPosition = MousePosition;
            }
        }

        private void MouseRightPressed(object sender, MouseButtonEventArgs e)
        {
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialPolygon, ProcessedPolygon);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialEllipse, ProcessedEllipse);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRectangle, ProcessedRectangle);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialColumnLine, ProcessedColumnLine);

            VectorOfMousePosition.Clear();
        }

        private void DrawRowLine(object sender, MouseButtonEventArgs e)
        {
            if (GLevelsRowOn == false) return;

            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);
            InitialRowLine = GetRowLine(canvasOriginalImage, initialImage, sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedRowLine = GetRowLine(canvasProcessedImage, processedImage, sliderZoom.Value);
        }

        private void DrawColumnLine(object sender, MouseButtonEventArgs e)
        {
            if (GLevelsColumnOn == false) return;

            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialColumnLine, ProcessedColumnLine);
            InitialColumnLine = GetColumnLine(canvasOriginalImage, initialImage, sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedColumnLine = GetColumnLine(canvasProcessedImage, processedImage, sliderZoom.Value);
        }

        private void DrawRectangle(object sender, MouseButtonEventArgs e)
        {
            if (MagnifierOn == false) return;

            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRectangle, ProcessedRectangle);
            InitialRectangle = GetRectangle(canvasOriginalImage, (int)MousePosition.X - 4, (int)MousePosition.Y - 4,
                sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedRectangle = GetRectangle(canvasProcessedImage, (int)MousePosition.X - 4, (int)MousePosition.Y - 4,
                sliderZoom.Value);
        }

        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            ResizeCanvas(canvasOriginalImage, sliderZoom.Value);
            ResizeCanvas(canvasProcessedImage, sliderZoom.Value);

            DrawRectangle(sender, e as MouseButtonEventArgs);
            DrawRowLine(sender, e as MouseButtonEventArgs);
            DrawColumnLine(sender, e as MouseButtonEventArgs);
        }

        private static readonly Dictionary<Shape, KeyValuePair<double, double>> shapesProperties = new Dictionary<Shape, KeyValuePair<double, double>>();

        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InitialCanvas != null)
            {
                IEnumerable<Shape> shapes = InitialCanvas.Children.OfType<Shape>();
                if (shapes == null)
                {
                    shapesProperties.Clear();
                    return;
                }

                ScaleTransform scaleTransform = new ScaleTransform
                {
                    ScaleX = sliderZoom.Value,
                    ScaleY = sliderZoom.Value
                };

                foreach (Shape shape in shapes)
                {
                    if (!shapesProperties.ContainsKey(shape))
                    {
                        double leftProperty = (double)shape.GetValue(LeftProperty);
                        double topProperty = (double)shape.GetValue(TopProperty);

                        shapesProperties.Add(shape, new KeyValuePair<double, double>(leftProperty, topProperty));
                    }

                    Canvas.SetLeft(shape, shapesProperties[shape].Key * sliderZoom.Value);
                    Canvas.SetTop(shape, shapesProperties[shape].Value * sliderZoom.Value);

                    if (!(shape.LayoutTransform is ScaleTransform))
                    {
                        shape.LayoutTransform = scaleTransform;
                    }
                    else
                    {
                        (shape.LayoutTransform as ScaleTransform).ScaleX = sliderZoom.Value;
                        (shape.LayoutTransform as ScaleTransform).ScaleY = sliderZoom.Value;
                    }
                }
            }
        }
    }
}