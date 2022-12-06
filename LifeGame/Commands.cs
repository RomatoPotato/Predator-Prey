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
    }
}
