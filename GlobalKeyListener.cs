using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Xamlade;

public class GlobalKeyListener
{ 
    public static bool MousePressed = false;
    public event Action<KeyEventArgs> KeyPressed;
    public event Action<KeyEventArgs> KeyReleased;

    public GlobalKeyListener(Window window)
    {
        window.AddHandler(InputElement.KeyDownEvent, KeyDownHandler, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.KeyUpEvent, KeyUpHandler, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
    }

    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
    {
        MousePressed = false;
    }

    private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
    {
        MousePressed = true;
        Workspace.InitPremovable();
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
        {
            KeyPressed?.Invoke(e);
            MainWindow.ResizeFlag = true;
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
