using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jCanvas : Canvas, JChildContainer, JControl, JBroadcastHandler<JControl>, JSelectable
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    private string controlType => jElementType.Canvas.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
  
    public List<string> XAMLPiece { get; set; }
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    public List<JControl> jChildren { get; }
    public jCanvas()
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
        //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }
    public void AddChild(JControl child, double top = 0, double left = 0)
    {
        jChildren.Add(child);
        child.jParent = this;
        SetTop((Control)child,top);
        SetLeft((Control)child, left);
        //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }
    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }
    public int XAMLRating { get; set; }
}