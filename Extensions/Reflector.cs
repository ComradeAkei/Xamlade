using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Diagnostics.Runtime;
using Xamlade.jClasses;

namespace Xamlade.Extensions;



/// <summary>
/// Освобождение от оков
/// </summary>
public static class Reflector
{
    /// <summary>
    /// Имя действительно является уникальным идентификатором Avalonia UI, но не в этой программе
    /// </summary>
    /// <param name="name"></param>
    /// <param name="element"></param>
    public static void SetName(string? name, JControl element)
    {
        var privateField =
            typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField?.SetValue(element, name);
    }
    
    
    public static void PrintFieldsForType(ClrRuntime runtime, string targetType)
    {
        int i = 0;
        ClrHeap heap = runtime.Heap;
        foreach (var ptr in heap.EnumerateObjects())
        {
            ClrType type = heap.GetObjectType(ptr);
            if (type.Name == targetType)
            {
                i++;
            }
        }
        Console.WriteLine(targetType+": "+i);
    }

    
    public static void SetXYBoundsZero(Control control)
    {
        var boundsField = typeof(Visual).GetField("_bounds", BindingFlags.NonPublic | BindingFlags.Instance);
        if (boundsField != null)
        {
            var bounds = (Rect)boundsField.GetValue(control);
            bounds = new Rect(0, 0, bounds.Width, bounds.Height);
            boundsField.SetValue(control, bounds);
        }
    }
    
    
    
    /// <summary>
    /// AVALONIA UI, ВАШИ МОДИФИКАТОРЫ ДОСТУПА МНЕ МЕШАЮТ! Я не преступник,но вы не оставили мне выбора...
    /// </summary>
    /// <param name="obj">Объект с несправедливым сокрытием</param>
    /// <param name="name">Имя приватного поля</param>
    /// <param name="value">Новое значение</param>
    /// <typeparam name="T"> Тип поля</typeparam>
    /// <exception cref="ArgumentException">Исключение: Поле с таким именем не существует</exception>
    public static void ForceSet<T>(object obj, string name, T value)
    {
        // Получение типа объекта
        var type = obj.GetType();
        // Поиск приватного поля
        var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
            return;
        }
        // Поиск приватного свойства
        var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
            return;
        }

        throw new ArgumentException($"No private field or property named '{name}' found in type '{type.FullName}'.");
    }
    
    
}