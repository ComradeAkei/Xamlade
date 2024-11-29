using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Xamlade.jClasses;

namespace Xamlade.FunctionalAreas;

public static class WorkspaceManager
{
    public static void CorrectProperties(JControl element)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            JBaseStatic.CorrectProperties(element);
        });
    }

    public static void CorrectDraw(Control element)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            element.InvalidateMeasure();
            element.InvalidateArrange();
            element.InvalidateVisual();
        }, DispatcherPriority.Render);
    }
}