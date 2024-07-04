using System;
using Avalonia.Interactivity;

namespace Xamlade;

public static class Utils
{
    //Отладочный итератор
    public static int NextgenIterator = 0;

    //Случайное число
    public static Random random;

    public static void Init()
    {
        random = new Random();
    }
    public static void DEBUG(object? sender, RoutedEventArgs e)
    {
       
    }
}