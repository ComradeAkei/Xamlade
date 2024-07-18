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
    
    
    public void AddChild(JControl child)
    {
        
       
        
        var jParent = child.jParent;
        var mTreeItem1 = child.mTreeItem;
        if (jChildren.Any())
            return;
        jParent.AddChild(this);
        if (child.jParent is jCanvas)
        {
            Canvas.SetTop(this,Canvas.GetTop(child as Control));
            Canvas.SetLeft(this,Canvas.GetLeft(child as Control));
        }
        jParent.RemoveChild(child);
        jChildren.Add(child);
        child.jParent = this;
        Child = (Control)child;
     
        (jParent as JControl).mTreeItem.Items.Remove(mTreeItem1);
        this.mTreeItem = new mTreeViewItem(this);
        this.mTreeItem.IsExpanded = true;
        this.mTreeItem.Header = $"{child.Name} border";
        (jParent as JControl).mTreeItem.Items.Add(this.mTreeItem);
        this.mTreeItem.Items.Add(mTreeItem1);
        Child.IsHitTestVisible = false;
        



    }

    public void Remove()
    {
        double top = 0, left = 0;
        var jParent = this.jParent;
        var child = this.jChildren[0];
        var mTreeItem1 = child.mTreeItem;
        (child as Control).IsHitTestVisible = true;
        if (jParent is jCanvas)
        {
            top = Canvas.GetTop(this as Control);
            left = Canvas.GetLeft(this as Control);
        }
        
        jParent.RemoveChild(this);
        RemoveChild(child);
        
        jParent.AddChild(child);
        if (jParent is jCanvas)
        {
            Canvas.SetTop(child as Control, top);
            Canvas.SetLeft(child as Control, left);
        }
        this.mTreeItem.Items.Remove(mTreeItem1);
        (jParent as JControl).mTreeItem.Items.Add(mTreeItem1);
        (jParent as JControl).mTreeItem.Items.Remove(mTreeItem);

    }
    public void RemoveChild(JControl? child = null)
    {
        jChildren.Clear();
        jParent.RemoveChild(this);
        Child = null;
    }
    
    
}