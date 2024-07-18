using System;
using System.Threading.Tasks;
using Avalonia;

namespace Xamlade;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Console.WriteLine($"Unhandled exception: {e.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Console.WriteLine($"Unobserved task exception: {e.Exception}");
            e.SetObserved(); // Помечаем исключение как обработанное
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in Main: {ex.Message}");
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
