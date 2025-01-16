using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Xamlade.SettingsWorkers.SettingsTypes;

namespace Xamlade.SettingsWorkers.FilesystemService;

public class JSONBus
{
    static string JSON_PATH = "./Settings";
    public static Settings SettingsFromFile(string fileName)
    {
        var settings = new Settings();
        try
        {
            // Формируем полный путь к JSON-файлу
            var filePath = Path.Combine(JSON_PATH, $"{fileName}.json");

            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {filePath} не найден.");
                return settings; 
            }

            // Считываем содержимое файла
            var jsonContent = File.ReadAllText(filePath);

            // Десериализуем JSON в словарь и заполняем SettingsDictionary
            var jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            if (jsonDictionary != null)
            {
                settings.SetDictionary(jsonDictionary);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении JSON файла: {ex.Message}");
        }

        return settings;
    }
}