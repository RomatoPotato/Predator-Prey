using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeGame.Additional
{
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

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(NumericBox));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, Math.Round(Math.Clamp(value, MinValue, MaxValue), 1));

                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ValueChangedEvent);
                RaiseEvent(routedEventArgs);
            }
        }

        public int MinValue {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public event EventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public NumericBox()
        {
            InitializeComponent();
        }

        private void Numeric_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) Value += Step;
            else Value -= Step;
        }

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
