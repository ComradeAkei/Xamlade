using System;
using System.Reflection;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.FunctionalAreas;

namespace Xamlade.Extensions;

public static class Utils
{
    
    private static Panel DebugPanel { get; set; }
    public static Timer DebugTimer{ get; set; }

    private static Timer MainTimer;
    
    public static bool isDebugPanelActive
    {
        get => DebugPanel.IsVisible;
        set
        {
            if (value)
                DebugPanel.IsVisible = true;
            else
                DebugPanel.IsVisible = false;
        }
    }
    //Отладочный итератор
    public static int NextgenIterator = 0;

    //Случайное число
    public static Random random;

    public static void Init(Panel debugPanel)
    {
        random = new Random();
        DebugPanel = debugPanel;
        DebugTimer = new Timer(100);
        DebugTimer.AutoReset = true;
        DebugTimer.Enabled = true;
        MainTimer = new Timer(4000);
        MainTimer.AutoReset = true;
        MainTimer.Elapsed += (sender, args) => Tick();
        MainTimer.Start();
    }
    public static void DEBUG(object? sender, RoutedEventArgs e)
    {
        isDebugPanelActive = !isDebugPanelActive;
      
        
        
    }

    public static void PrintDebugMessage(string message)
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ((DebugPanel.Children[0] as TextBlock)!).Text = message;
        });
    }

    private static void Tick()
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
        //    PropertiesControl.ShowProperties();
        });
    }
    
    
}