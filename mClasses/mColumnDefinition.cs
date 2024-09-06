using Avalonia.Controls;
using Xamlade.mClasses;

namespace Xamlade.jClasses;

public class mColumnDefinition:ColumnDefinition, MControl
{
    public jGrid parent;
    public mColumnDefinition(jGrid parent,GridLength height)
    {
        this.parent = parent;
        this.Width = height;
    }
    public mColumnDefinition(jGrid parent,double width, GridUnitType type = GridUnitType.Star)
    {
        this.parent = parent;
        this.Width = new GridLength(width, type);
        
    }
}