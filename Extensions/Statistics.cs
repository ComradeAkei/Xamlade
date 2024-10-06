using System.Collections.Generic;

namespace Xamlade.Extensions;

public static class Statistics
{
    /// <summary>
    /// Количество элементов по классам
    /// </summary>
    private static Dictionary<string, int> ElementCount { get; set; }

    static Statistics()
    {
        ElementCount = new Dictionary<string, int>();
    }
    
    /// <summary>
    /// Получить индекс нового элемента
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    public static int GetNewElementIndex(string Type)
    {
        ElementCount.TryAdd(Type, 0);
        return ElementCount[Type]++;
    }
    /// <summary>
    /// Статистика по элементам
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    public static int GetElementCount(string Type)
    {
        ElementCount.TryAdd(Type, 0);
        return ElementCount[Type];
    }
    
}