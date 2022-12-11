using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeGame.Additional
{
    public partial class NumericBox : UserControl
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set => SetValue(ValueProperty, Math.Clamp(value, MinValue, MaxValue));
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericBox));

        private Regex onlyNumbersRegex = new Regex(@"\D");

        public NumericBox()
        {
            InitializeComponent();

            DataObject.AddPastingHandler(NumericTextBox, NumericBoxPaste);
        }

        private void NumericBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            string clipboard = e.DataObject.GetData(typeof(string)) as string ?? "";
            DataObject dataObject = new DataObject();

            dataObject.SetData(DataFormats.Text, onlyNumbersRegex.Replace(clipboard, string.Empty));
            e.DataObject = dataObject;
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = onlyNumbersRegex.IsMatch(e.Text);
        }

        private void Numeric_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) Value++;
            else Value--;
        }

        private void NumericButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

        private void NumericButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }
    }
}
