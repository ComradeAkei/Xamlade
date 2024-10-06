using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jBorder : Border, JChildContainer, JControl, JBroadcastHandler<JControl>, JSelectable
{
    
    protected override Type StyleKeyOverride => typeof(Border);
    
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    
    
    private string controlType => jElementType.Border.ToString();
    public mBorder selectionBorder { get; set; }


    public JChildContainer? _jParent { get; set; }
    public string Type => controlType;
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;

    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }
    public List<JControl> jChildren { get; set; }
    
    
    
    public void AddChild(JControl child)
    {
        
       
        
        var jParent = child.jParent;
        var mTreeItem1 = child.Beholder.mTreeItem;
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
     
        (jParent as JControl).Beholder.mTreeItem.Items.Remove(mTreeItem1);
        this.Beholder.mTreeItem = new mTreeViewItem(this);
        this.Beholder.mTreeItem.IsExpanded = true;
        this.Beholder.mTreeItem.Header = $"{child.Name} border";
        (jParent as JControl).Beholder.mTreeItem.Items.Add(this.Beholder.mTreeItem);
        this.Beholder.mTreeItem.Items.Add(mTreeItem1);
        Child.IsHitTestVisible = false;
        



    }

    public void Remove()
    {
        double top = 0, left = 0;
        var jParent = (this as JControl).jParent;
        var child = this.jChildren[0];
        var mTreeItem1 = child.Beholder.mTreeItem;
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
        this.Beholder.mTreeItem.Items.Remove(mTreeItem1);
        (jParent as JControl).Beholder.mTreeItem.Items.Add(mTreeItem1);
        (jParent as JControl).Beholder.mTreeItem.Items.Remove(Beholder.mTreeItem);

    }
    public void RemoveChild(JControl? child = null)
    {
        jChildren.Clear();
        
        (this as JControl).jParent.RemoveChild(this);
        Child = null;
    }

    public void InitContainerProperties()
    {
        
    }


    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}