using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace Xamlade.Extensions;

public static class TypeCast
{

    public static IBrush ConvertToBrush(string colorName)
    {
        if (string.IsNullOrWhiteSpace(colorName))
            throw new ArgumentNullException(nameof(colorName));

        // Попробуем получить цвет из Brushes (предопределенные цвета)
        var property = typeof(Brushes).GetProperty(colorName);
        if (property != null && property.GetValue(null) is IBrush brush)
            return brush;

        // Пробуем распарсить строку в Color
        if (Color.TryParse(colorName, out var color))
            return new SolidColorBrush(color);

        throw new ArgumentException($"Не удалось преобразовать '{colorName}' в IBrush.");
    }
    
    public static T CastValue<T>(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var targetType = typeof(T);

        if (targetType == typeof(int))
            return (T)(object)int.Parse(value);
        if (targetType == typeof(double))
            return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(float))
            return (T)(object)float.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(bool))
            return (T)(object)bool.Parse(value);
        if (targetType == typeof(byte))
            return (T)(object)byte.Parse(value);
        if (targetType == typeof(short))
            return (T)(object)short.Parse(value);
        if (targetType == typeof(long))
            return (T)(object)long.Parse(value);
        if (targetType == typeof(decimal))
            return (T)(object)decimal.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(Guid))
            return (T)(object)Guid.Parse(value);
        if (targetType == typeof(DateTime))
            return (T)(object)DateTime.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(TimeSpan))
            return (T)(object)TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(char) && value.Length == 1)
            return (T)(object)value[0];
        if (targetType == typeof(string))
            return (T)(object)value;
        if (targetType == typeof(IBrush))
            return (T)(object)ConvertToBrush(value);
        if(targetType == typeof(Color))
            return (T)(object)Color.Parse(value);
        if (targetType == typeof(object))
            return (T)(object)value;
        if (targetType.IsEnum)
            if (Enum.TryParse(targetType, value, ignoreCase: true, out object result))
                return (T)result;
        throw new NotSupportedException($"Приведение к типу {targetType} не поддерживается");
    }
}