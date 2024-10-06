using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jComboBox : ComboBox, JControl, JBroadcastHandler<JControl>, JSelectable, JChildContainer
{
    public int ID = 0;
    protected override Type StyleKeyOverride => typeof(ComboBox);

    
    
    public mBorder selectionBorder { get; set; }

    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
    private string controlType => jElementType.ComboBox.ToString();
    public string Type => controlType;
    public int XAMLRating { get; set; }
    [field: NonSerialized] public List<string> XAMLPiece { get; set; }

       public new bool IsDropDownOpen
       {
           get => base.IsDropDownOpen;
           set => SetValue(IsDropDownOpenProperty, value);
       }

    public new string? Name
    {
        get => base.Name;
        set => SetValue(NameProperty, value);
    }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;


    

    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }

    public List<JControl> jChildren { get; set; }
    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
        Items.Add(child);
    }

    public void RemoveChild(JControl child)
    {
        Broadcast.OnBroadcast -= (child as JBroadcastHandler<JControl>).HandleBroadcast;
        jChildren.Remove(child);
        Items.Remove(child);
        
    }

    static jComboBox()
    {
       ContainerSetProperties = new(); 
    }
    public void InitContainerProperties()
    {
        ContainerProperties = new();
    }


    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}