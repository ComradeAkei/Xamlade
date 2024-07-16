using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jBorder : Border, JChildContainer, JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(Border);
    
    private string controlType => jElementType.Border.ToString();
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
    public List<JControl> jChildren { get; }

    public jBorder()
    {
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }
    
    public void AddChild(JControl child, double top = 0, double left = 0)
    {
        if (jChildren.Any())
            throw new Exception("jBorder can only have one child");
        jChildren.Add(child);
        child.jParent = this;
        Child = (Control)child;
    }

    public void RemoveChild(JControl? child = null)
    {
        jChildren.Clear();
        Child = null;
    }
}