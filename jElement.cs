using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

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
    public IChildContainer? jParent { get; set; }
    public object Type { get; }
    public mTreeViewItem? mTreeItem { get; set; }
   
    // Устанавливает фон элемента
    IBrush? Background { get; set; }
    

    // Указывает, активен ли элемент для взаимодействия с пользователем
    bool IsEnabled { get; set; }

    // Указывает, виден ли элемент
    bool IsVisible { get; set; }

    // Устанавливает прозрачность элемента (от 0 до 1)
    double Opacity { get; set; }

    // Устанавливает горизонтальное выравнивание элемента в его родительском контейнере
    HorizontalAlignment HorizontalAlignment { get; set; }

    // Устанавливает вертикальное выравнивание элемента в его родительском контейнере
    VerticalAlignment VerticalAlignment { get; set; }

    // Устанавливает ширину элемента
    double Width { get; set; }

    // Устанавливает высоту элемента
    double Height { get; set; }

    // Устанавливает минимально допустимые значения для ширины и высоты элемента
    double MinWidth { get; set; }
    double MinHeight { get; set; }

    // Устанавливает максимально допустимые значения для ширины и высоты элемента
    double MaxWidth { get; set; }
    double MaxHeight { get; set; }

    // Устанавливает форму указателя мыши, когда она находится над элементом
    Cursor? Cursor { get; set; }

    // Устанавливает контекст данных для элемента, который используется для привязки данных
    object? DataContext { get; set; }

    // Устанавливает уникальное имя элемента
    public string? Name { get; set; }

    // Позволяет присваивать один или несколько CSS-классов элементу
   // Classes Classes { get; set; }
   
    
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
    public IChildContainer? jParent { get; set; }
    private ControlType controlType => ControlType.Button;
    public object Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
}


public class jCanvas : Canvas, IChildContainer, JControl
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; }
    private ContainerType containerType => ContainerType.Canvas;
    public object Type => containerType;
    public mTreeViewItem? mTreeItem { get; set; }
    public IChildContainer? jParent { get; set; }
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