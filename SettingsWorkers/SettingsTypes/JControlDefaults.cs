using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Xamlade.Extensions;
using Xamlade.Extensions.Atributes;
using Xamlade.jClasses;
using Xamlade.SettingsWorkers.FilesystemService;

namespace Xamlade.SettingsWorkers.SettingsTypes;

public class JControlDefaults
{
    public static Dictionary<string,Settings> JDefaultSettings { get; set; } = new Dictionary<string, Settings>();
    private static IEnumerable<string> CurrentTypes;

    
    //Получаем имена классов, для которых нужно задать начальные параметры генерации
    private static IEnumerable<string> Type2Classname(IEnumerable<string> types)
    {
        return types.Select(type =>
        {
            var className = type[(type.LastIndexOf('.') + 1)..]; // Извлекаем имя класса
            return className.StartsWith("j") ? className[1..] : className; // Убираем "j", если есть
        });
    }
    //Готовим словарь для хранения настроек генерации
    static JControlDefaults()
    {
        var knownTypesList = Type2Classname(JTypeInfo.GetImplementingClasses(typeof(jClasses.JControl)));
        //Объявление словаря свойств под каждый тип 
        foreach (var type in knownTypesList)
            JDefaultSettings.Add(type, new Settings());
        //Инициализация словарей свойств из JSON
        foreach (var classSettings in JDefaultSettings)
            JDefaultSettings[classSettings.Key] = JSONBus.SettingsFromFile(classSettings.Key);
    }
    [ReflectionCall]
    public static void InitDefaults(JControl element)
    {
        //Словарь оставшихся свойств
        var unhandledProperties = new Settings();
        //Фаза нативной рефлексии
        var typeName = element.Type;
        var properties = JDefaultSettings[typeName];
        foreach (var prop in properties)
        {
            var targetType = Reflector.VerifyExistance(element, prop.Key);
            if (targetType != null)
            {
                var method = typeof(TypeCast).GetMethod(nameof(TypeCast.CastValue))
                    ?.MakeGenericMethod(targetType); // Создаём CastValue<TargetType>
                var convertedValue = method?.Invoke(null, new object[] { prop.Value }); // Вызываем CastValue<T>()
                
                Reflector.ForceSet(element, prop.Key, convertedValue);
            }
            else
            {
                unhandledProperties.Add(prop.Key, prop.Value);
            }
        }
        Console.WriteLine();
        //Фаза задания особенных свойств десериализации
        //Фаза вызова специальных делегатов
    }
    
    
}