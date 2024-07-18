using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jTextBox : TextBox, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public jTextBox()
    {
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(TextBox);
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBox.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    protected override void OnPointerPressed(PointerPressedEventArgs e) { }
    protected override void OnPointerMoved(PointerEventArgs e) { }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
}