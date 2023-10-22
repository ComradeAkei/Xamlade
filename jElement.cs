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

// Для контейнеров
public interface IChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child);
    public void RemoveChild(JControl child);
    
}
//Для простых объектов
public interface JControl
{
    public IChildContainer jParent { get; set; }
    public object Type { get; }
    public string Name { get; set; }
    public mTreeViewItem mTreeItem { get; set; }
}

//Модифицированные элементы дерева со встроенными jControl
public class mTreeViewItem : TreeViewItem
{
    protected override Type StyleKeyOverride => typeof(TreeViewItem); 
    public JControl element { get; set; }
    public mTreeViewItem(JControl element)
    {
        this.element = element;
        this.Header = element.Name;
        //Обратная связь с jElement
        element.mTreeItem = this;
    }
}
public class jButton : Button, JControl
{
    protected override Type StyleKeyOverride => typeof(Button);
    public IChildContainer jParent { get; set; }
    private ControlType controlType => ControlType.Button;
    public object Type => controlType;
    public mTreeViewItem mTreeItem { get; set; }
}


public class jCanvas : Canvas, IChildContainer, JControl
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; } = false;
    private ContainerType containerType => ContainerType.Canvas;
    public object Type => containerType;
    public mTreeViewItem mTreeItem { get; set; }
    public IChildContainer jParent { get; set; }
    public List<JControl> jChildren { get; }

    public jCanvas()
    {
        jChildren = new List<JControl>();
    }
    public jCanvas(IChildContainer jParent)
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
    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        this.Children.Remove((Control)child);
    }

}