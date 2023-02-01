using System.Windows.Input;

namespace LifeGame
{
    public static class Commands
    {
        public static readonly RoutedUICommand OpenChart =
            new RoutedUICommand("", "OpenChart", typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Shift | ModifierKeys.Control)
                });

        public static readonly RoutedUICommand ExportToExcel =
            new RoutedUICommand("", "ExportToExcel", typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.E, ModifierKeys.Control)
                });

        public static readonly RoutedUICommand ReturnDefault =
            new RoutedUICommand("", "ReturnDefault", typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.R, ModifierKeys.Control)
                });

        public static readonly RoutedUICommand Info =
            new RoutedUICommand("", "Info", typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F1)
                });
    }
}
