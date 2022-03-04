﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessingFramework.View
{
    public partial class DialogBox : Window
    {
        public DialogBox()
        {
            InitializeComponent();
        }

        public List<double> GetResponseTexts()
        {
            IEnumerable<TextBox> textBoxes = mainGrid.Children.OfType<TextBox>();
            var response = new List<double>();
            foreach (var textBox in textBoxes)
            {
                if (textBox.Text.ToString().Trim().Length == 0 || IsNumeric(textBox.Text.ToString()) == false)
                    response.Add(0);
                else
                    response.Add(double.Parse(textBox.Text));
            }

            return response;
        }

        public void CreateDialogBox(List<string> values)
        {
            dialogBoxWindow.Height = values.Count * 2 * 25 + 75;
            int index = 1;
            foreach (var val in values)
            {
                var textBlock = new TextBlock();
                textBlock.Text = val;
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var textBox = new TextBox();
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                Grid.SetRow(textBlock, index++);
                mainGrid.Children.Add(textBlock);
                Grid.SetRow(textBox, index++);
                mainGrid.Children.Add(textBox);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            dialogBoxWindow.Close();
        }

        private bool IsNumeric(string text)
        {
            double test;
            return double.TryParse(text, out test);
        }
    }
}