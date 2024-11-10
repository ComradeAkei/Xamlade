using System;
using Avalonia.Controls;
using Xamlade.mClasses;

namespace Xamlade.jClasses;

public class mGenButton:Button, MControl
{
    private string _label;

    public string Label
    {
        get => _label;
        set => _label = value.ToString();
    }

    public mGenButton(string label) => Label = label;
    protected override Type StyleKeyOverride => typeof(Button);

    public mGenButton():base()
    {
        this.Label = "";
    }
}