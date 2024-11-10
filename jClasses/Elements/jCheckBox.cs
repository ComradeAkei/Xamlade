using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Xamlade.Extensions;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jCheckBox : CheckBox, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public jCheckBox()
    {
        SpecialSetDelegates = new();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
    }

    
    protected override Type StyleKeyOverride => typeof(CheckBox);
    
    
    
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.CheckBox.ToString();
    public string Type => controlType;

    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public new bool IsPressed
    {
        get => base.IsPressed;
        set => Reflector.ForceSet(this,"_isPressed", value);
    }

    protected override void OnClick() {}

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}