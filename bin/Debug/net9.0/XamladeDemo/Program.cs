using Avalonia;
using System;

namespace XamladeDemo;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        
        // Глобальный обработчик необработанных исключений
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogError(ex);
            }
        };

        // Обработчик ошибок в асинхронных задачах
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            LogError(e.Exception);
            e.SetObserved(); // Предотвращает завершение приложения
        };

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Метод конфигурации Avalonia
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    // Метод для записи ошибок в лог
    private static void LogError(Exception ex)
    {
        string logPath = "errors.log";
        string message = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";

        // Запись в файл
        File.AppendAllText(logPath, message);

        // Опционально: показать сообщение в консоли
        Console.WriteLine($"Ошибка: {ex.Message}\nЛог сохранён в {logPath}");
    }
        

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}