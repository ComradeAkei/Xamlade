using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jGrid: Grid, JControl, JChildContainer, JSelectable, JBroadcastHandler<JControl>
{
    protected override Type StyleKeyOverride => typeof(Grid);
    public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.Grid.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<JControl> jChildren { get; }
    
    
    public jGrid()
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
    
    public static void SetRow(JControl element, int value) => 
        Grid.SetRow(element as Control, value);

    public static void SetColumn(JControl element, int value) => 
        Grid.SetColumn(element as Control, value);

    public static void SetRowSpan(JControl element, int value) => 
        Grid.SetRowSpan(element as Control, value);

    public static void SetColumnSpan(JControl element, int value) => 
        Grid.SetColumnSpan(element as Control, value);

    public static int GetRow(JControl element) => 
        Grid.GetRow(element as Control);

    public static int GetColumn(JControl element) => 
        Grid.GetColumn(element as Control);

    public static int GetRowSpan(JControl element) => 
        Grid.GetRowSpan(element as Control);

    public static int GetColumnSpan(JControl element) => 
        Grid.GetColumnSpan(element as Control);

    public mBorder selectionBorder { get; set; }
}