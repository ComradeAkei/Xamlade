using System;
using Avalonia.Controls;
using Xamlade.mClasses;

namespace Xamlade.jClasses;

public class mGenButton:Button, MControl
{
    public string Label { get; set; }
    
    public mGenButton(string label) => Label = label;
    protected override Type StyleKeyOverride => typeof(Button);

    public mGenButton():base()
    {
        this.Label = "";
    }
}