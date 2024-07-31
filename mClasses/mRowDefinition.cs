using System;
using Avalonia.Controls;
using Xamlade.mClasses;

namespace Xamlade.jClasses;

public class mRowDefinition: RowDefinition, MControl
{
    public jGrid parent;
    
    
    public mRowDefinition(jGrid parent,GridLength height)
    {
        this.parent = parent;
        this.Height = height;
    }
    public mRowDefinition(jGrid parent,double height, GridUnitType type = GridUnitType.Star)
    {
        this.parent = parent;
        this.Height = new GridLength(height, type);
    }

}