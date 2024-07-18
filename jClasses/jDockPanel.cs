using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jDockPanel: DockPanel, JControl, JChildContainer, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(DockPanel);
    public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.DockPanel.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<JControl> jChildren { get; }

    public jDockPanel()
    {
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
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

    public mBorder selectionBorder { get; set; }
}