using System;
using System.Reflection;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Xamlade;

public static class Utils
{
    
    private static Panel DebugPanel { get; set; }
    public static Timer DebugTimer{ get; set; } 
    
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
    }
    public static void DEBUG(object? sender, RoutedEventArgs e)
    {
        isDebugPanelActive = !isDebugPanelActive;
        // Предположим, что у вас есть объект Border
        Border border = new Border();

// Пример установки содержимого Border
        border.Child = new Button { Content = "Click me" };

// Используем рефлексию для доступа к приватному полю _child
        FieldInfo childField = typeof(Border).GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);

        if (childField != null)
        {
            var child = childField.GetValue(border) as Control;
            if (child != null)
            {
                // Ваш код для работы с дочерним элементом
                // Например, вывести тип дочернего элемента
                Console.WriteLine(child.GetType().Name);
            }
        }
        
    }

    public static void PrintDebugMessage(string message)
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ((DebugPanel.Children[0] as TextBlock)!).Text = message;
            Console.WriteLine();
        });
    }
        
    
    
    
}