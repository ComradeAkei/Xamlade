using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jDockPanel: DockPanel, JControl, JChildContainer, JBroadcastHandler<JControl>, JSelectable, JProperties
{
    protected override Type StyleKeyOverride => typeof(DockPanel);

    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
  //  public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.DockPanel.ToString();
    public string Type => controlType;
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }

    public List<JControl> jChildren { get; set; }

    public jDockPanel()
    {
        SpecialSetDelegates = new();
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
       
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
    
    public void InitContainerProperties()
    {
        
        ContainerProperties = new List<(string, JChildContainer.ContainerPropertyDelegate)>
        {
            ("Dock", (JControl element) => new Property(GetDock(element),typeof(Dock)))
        };
    }

    static jDockPanel()
    {
        
       ContainerSetProperties = new()
        {
            { "Dock",  (jControl, value) => SetDock(jControl, (Dock)Enum.Parse(typeof(Dock), (string)value.Value)) }
        }; 
    }

    public static void SetDock(JControl element, Dock value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        DockPanel.SetDock(element as Control, value);
    }

    public static Dock GetDock(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return DockPanel.GetDock(element as Control);
    }
    

    public mBorder selectionBorder { get; set; }

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}