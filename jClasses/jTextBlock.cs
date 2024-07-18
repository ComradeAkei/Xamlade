using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jTextBlock : TextBlock, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public jTextBlock()
    {
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);
    public mBorder selectionBorder { get; set; }
    public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBlock.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
}