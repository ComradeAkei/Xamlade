using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jToggleButton : ToggleButton, JControl, JBroadcastHandler<JControl>, JSelectable 
{
    protected override Type StyleKeyOverride => typeof(ToggleButton);
    public jToggleButton()
    {
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }
    private string controlType => jElementType.ToggleButton.ToString();
    public string Type => controlType;
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }

    protected override void OnClick() {}
}