using Avalonia.Media;
using AvaloniaColorPicker;
using Xamlade.Extensions;

namespace Xamlade.ColorPicker;

public class MyColorPickerWindow : ColorPickerWindow
{
    public MyColorPickerWindow() : base()
    {
        IsPaletteVisible = false;
        
    }
    
    public MyColorPickerWindow(Color? previousColor) : base(previousColor)
    {
        IsPaletteVisible = false;
    }
}