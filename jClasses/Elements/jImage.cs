using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jImage : Image, JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(Image);

    public jImage()
    {
        SpecialSetDelegates = new();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
       
        
    }

    
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.Image.ToString();
    public string Type => controlType;

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    //Понизили доступ к свойству Background, лол. Хотя интерфейс регламентирует public
    IBrush? JControl.Background { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
    public string? jImageSource { get; set; }

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}