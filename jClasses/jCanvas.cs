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

    public static void SetTop(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetTop(element as Control, value);
    }

    public static void SetLeft(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetLeft(element as Control, value);
    }

    public static void SetRight(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetRight(element as Control, value);
    }

    public static void SetBottom(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetBottom(element as Control, value);
    }

    public static double GetLeft(JControl element) => 
        Canvas.GetLeft(element as Control);

    public static double GetRight(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return Canvas.GetRight(element as Control);
    }

    public static double GetTop(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return Canvas.GetTop(element as Control);
    }

    public static double GetBottom(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return Canvas.GetBottom(element as Control);
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
        SetTop(child, top);
        SetLeft(child, left);
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