using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.FunctionalAreas;

namespace Xamlade.Extensions;

public class GlobalKeyListener
{
    public event Action<KeyEventArgs> KeyPressed = null!;
    public event Action<KeyEventArgs> KeyReleased = null!;

    public GlobalKeyListener(Window window)
    {
        window.AddHandler(InputElement.KeyDownEvent, KeyDownHandler!, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.KeyUpEvent, KeyUpHandler!, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
        window.AddHandler(InputElement.PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
    }

    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
    {
    }

    private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
    {
        Workspace.InitPremovable();
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
        {
            KeyPressed.Invoke(e);
            State.ResizeFlag = true;
        }
        else if (e.Key == Key.LeftShift)
        {
            KeyPressed.Invoke(e);
            State.LShiftPressed = true;
        }
    }
    private void KeyUpHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.Delete || e.Key == Key.LeftShift)
        {
            KeyReleased.Invoke(e);
        }
    }
}
