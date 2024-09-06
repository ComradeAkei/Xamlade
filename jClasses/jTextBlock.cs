using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jTextBlock : TextBlock, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public jTextBlock()
    {
        SpecialSetDelegates = new();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
   
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);
    public mBorder selectionBorder { get; set; }
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
    //public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBlock.ToString();
    public string Type => controlType;

    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}