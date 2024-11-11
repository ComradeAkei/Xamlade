using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using ScottPlot.Avalonia;
using Xamlade.Extensions;
using Xamlade.LinkWorkers;

namespace Xamlade.jClasses;

public class jAvaPlot:AvaPlot,JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(AvaPlot);
    
    

    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
   
    public JChildContainer? _jParent { get; set; }

    private string controlType => jElementType.AvaPlot.ToString();
    public string Type => controlType;
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public IBrush? Background { get; set; }


    public new string? Name
    {
        get => base.Name;
        set => SetValue(NameProperty, value);
    }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
        
    public void AddSpecialProperties()
    {
        AddSpecialSetDelegates();
        xPropertiesGroup["specials"] = new();
        xPropertiesGroup["specials"]["Title"] = new Property(this.Plot.Axes.Title.Label.Text, typeof(string),2);
      //  xPropertiesGroup["specials"]["Columns"] = new Property(this.ColumnDefinitions.Count, typeof(int),2);
        this.Beholder.UpdatePropList("Title");
    //    this.Beholder.UpdatePropList("Columns");
    }

    public void AddSpecialSetDelegates()
    {
       SpecialSetDelegates["Title"] = Utils.ConvertSetter<string>(SetPlotTitle);
    //    SpecialSetDelegates["Columns"] = Utils.ConvertSetter<int>(SetColumnsCount);
    }

    void SetPlotTitle(JControl element, string name) => this.Plot.Title(name);
    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

   
}