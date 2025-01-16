using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Diagnostics.Runtime;
using Xamlade.Extensions.Atributes;
using Xamlade.jClasses;

namespace Xamlade.Extensions;



/// <summary>
/// Освобождение от оков. Или изъяны реализации.
/// Каждый метод, использующий рефлексию, ОБЯЗАТЕЛЬНО помечать атрибутом ReflectionCall 
/// </summary>
public static class Reflector
{
    /// <summary>
    /// Имя действительно является уникальным идентификатором Avalonia UI, но не в этой программе
    /// </summary>
    /// <param name="name"></param>
    /// <param name="element"></param>
    [ReflectionCall]
    public static void SetName(string? name, JControl element)
    {
        var privateField =
            typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField?.SetValue(element, name);
    }
    
    [ReflectionCall]
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
        Console.WriteLine($"{targetType} :  {i}");
    }

    [ReflectionCall]
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
    /// <typeparam name="makeNull"> Обнуление поля (игнор value)</typeparam>
    /// <exception cref="ArgumentException">Исключение: Поле с таким именем не существует</exception>
    [ReflectionCall]
    public static void ForceSet<T>(object obj, string name, T? value, bool makeNull = false)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым", nameof(name));

        // Если требуется установка в null
        if (makeNull)
            value = default;

        Type type = obj.GetType();

        // Проверяем все возможные варианты имени
        string[] possibleNames = { name, "_" + name, name.ToLower(), "_" + name.ToLower() };

        foreach (string n in possibleNames)
        {
            var member = GetMemberRecursive(type, n);
            if (member is FieldInfo field)
            {
                field.SetValue(obj, value);
                return;
            }
            if (member is PropertyInfo property && property.CanWrite)
            {
                property.SetValue(obj, value);
                return;
            }
        }

        throw new ArgumentException($"Поле или свойство '{name}' не найдено в типе '{type.FullName}' и его базовых классах.");
    }

    [ReflectionCall]
    private static MemberInfo GetMemberRecursive(Type type, string name)
    {
        while (type != null)
        {
            // Ищем сначала поле
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            // Затем ищем свойство
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
    [ReflectionCall]
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
    [ReflectionCall]
    public static Type? VerifyExistance(object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();

        // Проверяем наличие свойства
        var property = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
            return property.PropertyType;

        // Проверяем наличие поля
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
            return field.FieldType;

        return null;
    }
}
    
    
