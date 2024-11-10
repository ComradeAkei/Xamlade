using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.Extensions.Atributes;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;


public class jSlider : Slider, JControl, JBroadcastHandler<JControl>, JSelectable
{
    #region Required

    protected override Type StyleKeyOverride => typeof(Slider);

    private string controlType => jElementType.Slider.ToString();
    public string Type => controlType;

    #endregion


    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
    }

    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
    
}