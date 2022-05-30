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

namespace ImageProcessingFramework.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MousePosition = new Point(0, 0);
            LastPosition = new Point(0, 0);
            VectorOfMousePosition = new PointCollection();

            InitialCanvas = canvasOriginalImage;
            ProcessedCanvas = canvasProcessedImage;

            VectorOfLines = new Collection<Line>();
            VectorOfRectangles = new Collection<Rectangle>();
            VectorOfEllipses = new Collection<Ellipse>();
            VectorOfPolygons = new Collection<Polygon>();
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
            else if (string.Compare(nameImage, processedImage.Name) == 0)
            {
                var position = e.GetPosition(processedImage);
                SetUiValues(xPos, yPos, grayValue, bValue, gValue, rValue, GrayProcessedImage, ColorProcessedImage,
                    (int)position.X, (int)position.Y);
            }

            if (MagnifierOn == false)
                RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialSquare, ProcessedSquare);

            if (RowLevelsOn == false)
                RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);

            if (ColumnLevelsOn == false)
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
            if (string.Compare(nameImage, processedImage.Name) == 0)
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
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialSquare, ProcessedSquare);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialColumnLine, ProcessedColumnLine);
            RemoveAllDrawnShapes();

            VectorOfMousePosition.Clear();
        }

        private void DrawUiElements_MouseLeftPressed(object sender, MouseButtonEventArgs e)
        {
            DrawUiElements();
        }

        private void DrawUiElements()
        {
            if (MagnifierOn == true)
            {
                DrawSquare();
                DrawRowLine();
                DrawColumnLine();
            }

            if (RowLevelsOn == true)
                DrawRowLine();

            if (ColumnLevelsOn == true)
                DrawColumnLine();
        }

        private void DrawRowLine()
        {
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialRowLine, ProcessedRowLine);
            InitialRowLine = GetRowLine(canvasOriginalImage, initialImage, sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedRowLine = GetRowLine(canvasProcessedImage, processedImage, sliderZoom.Value);
        }

        private void DrawColumnLine()
        {
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialColumnLine, ProcessedColumnLine);
            InitialColumnLine = GetColumnLine(canvasOriginalImage, initialImage, sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedColumnLine = GetColumnLine(canvasProcessedImage, processedImage, sliderZoom.Value);
        }

        private void DrawSquare()
        {
            RemoveUiElements(canvasOriginalImage, canvasProcessedImage, InitialSquare, ProcessedSquare);
            InitialSquare = GetSquare(canvasOriginalImage, LastPosition.X - 0.5, LastPosition.Y - 0.5,
                sliderZoom.Value);
            if (ColorProcessedImage == null && GrayProcessedImage == null) return;
            ProcessedSquare = GetSquare(canvasProcessedImage, LastPosition.X - 0.5, LastPosition.Y - 0.5,
                sliderZoom.Value);
        }

        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            DrawUiElements();

            ResizeCanvas(canvasOriginalImage, sliderZoom.Value);
            ResizeCanvas(canvasProcessedImage, sliderZoom.Value, false);
        }

        private static readonly Dictionary<Shape, (double, double)> shapesProperties = new Dictionary<Shape, (double, double)>();

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

                        shapesProperties.Add(shape, (leftProperty, topProperty));
                    }

                    Canvas.SetLeft(shape, shapesProperties[shape].Item1 * sliderZoom.Value);
                    Canvas.SetTop(shape, shapesProperties[shape].Item2 * sliderZoom.Value);

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