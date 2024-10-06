using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jStackPanel : StackPanel, JControl, JChildContainer, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(StackPanel);
    public mBorder selectionBorder { get; set; }
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.StackPanel.ToString();
    public string Type => controlType;
   
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }

    public List<JControl> jChildren { get; set; }

    public jStackPanel()
    {
        SpecialSetDelegates = new();
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
   
    }

    static jStackPanel()
    {
        ContainerSetProperties = new();
    }

    public void InitContainerProperties()
    {
        ContainerProperties = new();
    }

    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
        Children.Add((Control)child);
    }

    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}