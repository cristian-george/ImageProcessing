using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageProcessingFramework.View
{
    public partial class DialogBox : Window
    {
        private Button OKButton;

        public DialogBox()
        {
            InitializeComponent();
        }

        public List<double> GetResponseTexts()
        {
            IEnumerable<TextBox> textBoxes = mainGrid.Children.OfType<TextBox>();
            List<double> response = new List<double>();
            foreach (TextBox textBox in textBoxes)
            {
                if (textBox.Text.ToString().Trim().Length == 0 || IsNumeric(textBox.Text.ToString()) == false)
                    response.Add(0);
                else
                    response.Add(double.Parse(textBox.Text));
            }

            return response;
        }

        private void CreateTextBoxes(List<string> values, ref int counter)
        {
            foreach (string val in values)
            {
                #region Create TextBlock

                TextBlock textBlock = new TextBlock
                {
                    Text = val,
                    Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF1, 0xF1, 0xF1)),
                    Height = 20
                };

                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                Grid.SetRow(textBlock, counter++);
                mainGrid.Children.Add(textBlock);

                #endregion

                #region Create TextBox

                TextBox textBox = new TextBox { Height = 20 };

                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                Grid.SetRow(textBox, counter++);
                mainGrid.Children.Add(textBox);

                #endregion
            }
        }

        private void CreateOKButton(ref int counter)
        {
            #region Create an empty TextBlock

            TextBlock nullBlock = new TextBlock { Height = 20 };

            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Grid.SetRow(nullBlock, counter++);
            mainGrid.Children.Add(nullBlock);

            #endregion

            #region Create Button

            OKButton = new Button
            {
                Content = "OK",
                Height = 25,
                Width = 50,
                Background = new SolidColorBrush(Colors.AliceBlue),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x62, 0x9A, 0x62)),
            };
            OKButton.Click += OKButton_Click;

            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Grid.SetRow(OKButton, counter++);
            mainGrid.Children.Add(OKButton);

            #endregion
        }

        public void CreateDialogBox(List<string> values)
        {
            dialogBoxWindow.Height = (values.Count + 3) * 40;

            int counter = 1;
            CreateTextBoxes(values, ref counter);
            CreateOKButton(ref counter);
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