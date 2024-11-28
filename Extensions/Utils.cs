using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Xamlade.FunctionalAreas;
using Xamlade.jClasses;
using Xamlade.LinkWorkers;
using Xamlade.ProgramWindow;

namespace Xamlade.Extensions;

public static class Utils
{
    private static Panel DebugPanel { get; set; }
    public static Timer DebugTimer { get; set; }

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

        JControl newObj = JCopy.Copy(Workspace.movable,Workspace.movable.jParent);

        var parent = newObj.jParent as JControl;
        jCanvas.SetLeft((JControl)newObj, jCanvas.GetLeft((JControl)newObj) + (int)(parent.Bounds.Width/4.5f));

    }

    public static void PrintDebugMessage(string message)
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ((DebugPanel.Children[0] as TextBlock)!).Text = message;
        });
    }

    //Конвертеры делегатов
    public static JChildContainer.ContainerSetPropertyDelegate ConvertSetter<T>(Action<JControl, T> setter) =>
        (element, prop) =>
        {
            T value = (T)(prop.Value ?? default(T));
            setter(element, value);
        };

    public static JChildContainer.ContainerPropertyDelegate ConvertGetter<T>(Func<JControl, T> getter)=>
         (element) =>
        {
            T value = getter(element);  // Получаем значение типа T
            return new Property(value);  // Заворачиваем в Property
        };
    
    private static void Tick()
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            //    PropertiesControl.ShowProperties();
        });
    }
}