using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Xamlade;


public enum ContainerType
{
    Border,
    Canvas,
    DockPanel,
    Grid,
    Panel,
    ScrollViewer,
    StackPanel,
    TabControl,
    TabItem
}

public enum ControlType
{
    Button,
    CheckBox,
    ComboBox,
    DatePicker,
    ListBox,
    ListView,
    Menu,
    MenuItem,
    ProgressBar,
    RadioButton,
    Slider,
    TextBox,
    ToggleButton
}


public interface IChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child);
}

public interface JControl
{
    public JControl? jParent { get; set; }
    public object Type { get; }
}

public class jButton : Button, JControl
{
    protected override Type StyleKeyOverride => typeof(Button);
    public JControl? jParent { get; set; }
    private ControlType controlType => ControlType.Button;
    public object Type => controlType;
}


public class jCanvas : Canvas, IChildContainer, JControl
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; } = false;
    private ContainerType containerType => ContainerType.Canvas;
    public object Type => containerType;
    public JControl? jParent { get; set; }
    public List<JControl> jChildren { get; }

    public jCanvas()
    {
        jChildren = new List<JControl>();
    }
    public jCanvas(JControl? jParent)
    {
        this.jParent = jParent;
        jChildren = new List<JControl>();
    }
    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
        Canvas.SetTop((Control)child,0);
        Canvas.SetLeft((Control)child, 0);
        Console.WriteLine(child.GetType().ToString());
        this.Children.Add((Control)child);
    }

}