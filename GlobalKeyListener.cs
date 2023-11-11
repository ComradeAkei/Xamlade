using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Xamlade;

public class GlobalKeyListener
{
    public event Action<KeyEventArgs> KeyPressed;
    public event Action<KeyEventArgs> KeyReleased;

    public GlobalKeyListener(Window window)
    {
        window.AddHandler(InputElement.KeyDownEvent, KeyDownHandler, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.KeyUpEvent, KeyUpHandler, RoutingStrategies.Tunnel);
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
        {
            KeyPressed?.Invoke(e);
        }
    }
    private void KeyUpHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.Delete)
        {
            KeyReleased?.Invoke(e);
        }
    }
}
