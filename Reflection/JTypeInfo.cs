using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamlade.Extensions;

public static class JTypeInfo
{
    public static IEnumerable<string> GetImplementingClasses(Type interfaceType)
    {
        if (!interfaceType.IsInterface)
            throw new ArgumentException($"{interfaceType.FullName} is not an interface.");

        // Получаем все типы из загруженных сборок
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes());

        // Фильтруем только классы, которые реализуют заданный интерфейс
        var implementingTypes = allTypes
            .Where(type => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
            .Select(type => type.FullName);

        return implementingTypes;
    }
}