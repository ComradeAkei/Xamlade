using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jToggleButton : ToggleButton, JControl, JBroadcastHandler<JControl>, JSelectable 
{
    protected override Type StyleKeyOverride => typeof(ToggleButton);
    public static  int Iterator { get; set; }
    public static int ReleaseNewElement() => 
        Iterator++;
    public jToggleButton()
    {
        SpecialSetDelegates = new();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
    }
    private string controlType => jElementType.ToggleButton.ToString();
    public string Type => controlType;
    public mBorder selectionBorder { get; set; }
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }

    protected override void OnClick() {}

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}