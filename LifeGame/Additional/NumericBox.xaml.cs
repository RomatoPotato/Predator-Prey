using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeGame.Additional
{
    /*
     *  Этот элемент создан в качестве удобного заменителя TextBox
     */
    public partial class NumericBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(NumericBox),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(int),
            typeof(NumericBox));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(int),
            typeof(NumericBox));

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step",
            typeof(double),
            typeof(NumericBox));

        // Отображаемое в элементе число
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, Math.Round(Math.Clamp(value, MinValue, MaxValue), 1));
            }
        }

        // Минимальное возможное значение
        public int MinValue {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        // Максимальное возможное значение
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        // На сколько будет изменяться число при увеличении/уменьшении
        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public NumericBox()
        {
            InitializeComponent();
        }

        // Изменение значения прокруткой мыши
        private void Numeric_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) Value += Step;
            else Value -= Step;
        }

        // Изменение значения кнопками
        private void NumericButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
        }

        private void NumericButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
        }
    }
}
