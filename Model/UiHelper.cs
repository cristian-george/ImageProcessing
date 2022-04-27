﻿using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using static ImageProcessingFramework.Model.DataProvider;
using System.Windows;
using Ellipse = System.Windows.Shapes.Ellipse;
using System.Collections.ObjectModel;

namespace ImageProcessingFramework.Model
{
    class UiHelper
    {
        public static Rectangle InitialRectangle { get; set; }
        public static Rectangle ProcessedRectangle { get; set; }

        public static Line InitialRowLine { get; set; }
        public static Line ProcessedRowLine { get; set; }

        public static Line InitialColumnLine { get; set; }
        public static Line ProcessedColumnLine { get; set; }

        public static Ellipse InitialEllipse { get; set; }
        public static Ellipse ProcessedEllipse { get; set; }

        public static Polygon InitialPolygon { get; set; }
        public static Polygon ProcessedPolygon { get; set; }

        public static void ResizeCanvas(Canvas canvas, double scaleValue, bool initialImage = true)
        {
            if (initialImage == true)
            {
                if (GrayInitialImage != null)
                {
                    canvas.Height = GrayInitialImage.Height * scaleValue;
                    canvas.Width = GrayInitialImage.Width * scaleValue;
                }

                if (ColorInitialImage != null)
                {
                    canvas.Height = ColorInitialImage.Height * scaleValue;
                    canvas.Width = ColorInitialImage.Width * scaleValue;
                }
            }
            else
            {
                if (GrayProcessedImage != null)
                {
                    canvas.Height = GrayProcessedImage.Height * scaleValue;
                    canvas.Width = GrayProcessedImage.Width * scaleValue;
                }

                if (ColorProcessedImage != null)
                {
                    canvas.Height = ColorProcessedImage.Height * scaleValue;
                    canvas.Width = ColorProcessedImage.Width * scaleValue;
                }
            }
        }

        public static void SetUiValues(TextBlock xPos,
            TextBlock yPos,
            TextBlock grayValue,
            TextBlock bValue,
            TextBlock gValue,
            TextBlock rValue,
            Image<Gray, byte> grayImage,
            Image<Bgr, byte> colorImage,
            int x, int y)
        {
            xPos.Text = "X: " + x.ToString() + " ";
            yPos.Text = "Y: " + y.ToString();

            grayValue.Text = (grayImage != null && y >= 0 && y < grayImage.Height && x >= 0 && x < grayImage.Width) ?
                "Gray: " + grayImage.Data[y, x, 0] : "Gray: 0";
            bValue.Text = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "B: " + colorImage.Data[y, x, 0] : "B: 0";
            gValue.Text = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "G: " + colorImage.Data[y, x, 1] : "G: 0";
            rValue.Text = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "R: " + colorImage.Data[y, x, 2] : "R: 0";
        }

        public static Rectangle GetRectangle(Canvas canvas, double x, double y, double scaleValue)
        {
            var rectangle = new Rectangle
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            rectangle.Width = 9 * scaleValue;
            rectangle.Height = 9 * scaleValue;

            Canvas.SetLeft(rectangle, (x - 4) * scaleValue);
            Canvas.SetTop(rectangle, (y - 4) * scaleValue);
            canvas.Children.Add(rectangle);

            return rectangle;
        }

        public static Line GetRowLine(Canvas canvas, Image image, double scaleValue)
        {
            var line = new Line
            {
                X1 = 0,
                Y1 = LastPosition.Y * scaleValue,
                X2 = image.ActualWidth * scaleValue,
                Y2 = LastPosition.Y * scaleValue,
                StrokeThickness = 2,
                Stroke = Brushes.Red
            };
            canvas.Children.Add(line);

            return line;
        }

        public static Line GetColumnLine(Canvas canvas, Image image, double scaleValue)
        {
            var line = new Line
            {
                X1 = LastPosition.X * scaleValue,
                Y1 = 0,
                X2 = LastPosition.X * scaleValue,
                Y2 = image.ActualHeight * scaleValue,
                StrokeThickness = 2,
                Stroke = Brushes.Red
            };
            canvas.Children.Add(line);

            return line;
        }

        public static void RemoveUiElements(Canvas initialCanvas,
            Canvas processedCanvas,
            UIElement initialElement,
            UIElement processedElement)
        {
            initialCanvas.Children.Remove(initialElement);
            processedCanvas.Children.Remove(processedElement);
        }

        public static void RemoveAllDrawnLines(Canvas initialCanvas,
            Canvas processedCanvas,
            Collection<Line> vectorOfLines)
        {
            foreach (Line line in vectorOfLines)
            {
                initialCanvas.Children.Remove(line);
                processedCanvas.Children.Remove(line);
            }
        }

        public static void RemoveAllDrawnRectangles(Canvas initialCanvas,
            Canvas processedCanvas,
            Collection<Rectangle> vectorOfRectangles)
        {
            foreach (Rectangle rectangle in vectorOfRectangles)
            {
                initialCanvas.Children.Remove(rectangle);
                processedCanvas.Children.Remove(rectangle);
            }
        }

        public static void RemoveAllDrawnEllipses(Canvas initialCanvas,
            Canvas processedCanvas,
            Collection<Ellipse> vectorOfEllipses)
        {
            foreach (Ellipse ellipse in vectorOfEllipses)
            {
                initialCanvas.Children.Remove(ellipse);
                processedCanvas.Children.Remove(ellipse);
            }
        }

        public static void RemoveAllDrawnPolygons(Canvas initialCanvas,
            Canvas processedCanvas,
            Collection<Polygon> vectorOfPolygons)
        {
            foreach (Polygon polygon in vectorOfPolygons)
            {
                initialCanvas.Children.Remove(polygon);
                processedCanvas.Children.Remove(polygon);
            }
        }
    }
}