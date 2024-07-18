using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jImage : Image, JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(Image);
    public jImage()
    {
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
        
    }

    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.Image.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    //Понизили доступ к свойству Background, лол. Хотя интерфейс регламентирует public
    IBrush? JControl.Background { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
    public string? jImageSource { get; set; }
    
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLizeElement(this);
        else if (mode == 2) ImportXAML.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) ImportXAML.CorrectTree(this);
        else if (mode == 5) 
            if(selectionBorder is not null)
                selectionBorder.IsVisible = false;
    }
    
}