using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jCheckBox : CheckBox, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public jCheckBox()
    {
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(CheckBox);
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.CheckBox.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }
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

    protected override void OnClick() {}
}