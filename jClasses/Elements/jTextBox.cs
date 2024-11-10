using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.Extensions.Atributes;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;


public class jTextBox : TextBox, JControl, JBroadcastHandler<JControl>, JSelectable
{


    protected override Type StyleKeyOverride => typeof(TextBox);

    
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
  //  public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBox.ToString();
    public string Type => controlType;

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    protected override void OnPointerPressed(PointerPressedEventArgs e) { }
    protected override void OnPointerMoved(PointerEventArgs e) { }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}