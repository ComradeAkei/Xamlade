using System;
using System.Linq;
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
    public static void ForceSet<T>(object obj, string name, T? value, bool makeNull = false) 
    {
        
        if (makeNull)
        {
            if (typeof(T).IsValueType)
                value = default(T); // Значимый тип
            else
                value = default; // Ссылочный тип (null)
        }
        // Получение типа объекта
        var type = obj.GetType();
    
        // Если нужно установить null, то используем default(T) для значимых типов
        if (makeNull)
            value = default(T); // Присваиваем значение по умолчанию (например, 0 для int)

        // Поиск приватного поля
        var field = GetFieldRecursive(type, name);
        if (field != null)
        {
            field.SetValue(obj, value);
            return;
        }

        // Поиск приватного свойства
        var property = GetPropertyRecursive(type, name);
        if (property != null)
        {
            property.SetValue(obj, value);
            return;
        }

        // Если имя начинается с подчеркивания
        if (name[0] == '_')
            throw new ArgumentException($"No private field or property named '{name}' found in type '{type.FullName}' or its base types.");

        // Пробуем найти поле или свойство с изменённым именем
        ForceSet(obj, $"_{name.ToLower()}", value, makeNull);
    }

    //Ищем поле в вышестоящих классах по цепочке наследования
    private static FieldInfo GetFieldRecursive(Type type, string name)
    {
        while (type != null)
        {
            var field = type.GetField(name, BindingFlags.Public |BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            type = type.BaseType;
        }
        return null;
    }
    
    //Ищем сойство в вышестоящих классах по цепочке наследования
    private static PropertyInfo GetPropertyRecursive(Type type, string name)
    {
        while (type != null)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
                return property;

            type = type.BaseType;
        }
        return null;
    }
    
    /// <summary>
    /// Вызывает статический метод класса объекта с помощью рефлексии.
    /// </summary>
    /// <param name="obj">Объект, тип которого используется для поиска метода.</param>
    /// <param name="methodName">Имя вызываемого статического метода.</param>
    /// <param name="parameters">Параметры, передаваемые в вызываемый метод.</param>
    /// <returns>Результат выполнения статического метода, если метод имеет возвращаемое значение. Иначе — null.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если метод с указанным именем не найден.</exception>

    public static object StaticCall(object obj, string methodName, params object[] parameters)
    {
        // Поиск метода в интерфейсах объекта
        var methodInfo = obj.GetType()
            .GetInterfaces()
            .Select(iface => iface.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .FirstOrDefault(method => method != null);

        // Если метод найден в интерфейсах, вызываем его
        if (methodInfo != null)
        {
            return methodInfo.Invoke(null, parameters);
        }

        // Ищем метод в типе объекта
        methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        return methodInfo?.Invoke(null, parameters)
               ?? throw new ArgumentException($"Метод {methodName} не найден.");
    }
}
    
    
